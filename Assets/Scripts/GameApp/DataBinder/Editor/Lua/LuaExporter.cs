using System;
using System.Reflection;
using System.Text;
using UnityEditor;
using LuaInterface;
using System.IO;
using UnityEngine;
using System.Collections.Generic;
using System.Linq;
using System.Runtime.CompilerServices;
using System.Text.RegularExpressions;
using GameApp.LuaResolver;
using UnityEngine.Profiling;
using BindType = ToLuaMenu.BindType;
using ParameterInfo = System.Reflection.ParameterInfo;
using PropertyInfo = System.Reflection.PropertyInfo;

namespace GameApp.DataBinder
{
    public static class LuaExporter
    {
        private const string ApiPath = "./Tools/EmmyLuaApi/";
        private const string ClassName = "m";
        private const string ConstrcutorName = "New";

        private static readonly BindType[] Extras =
        {
            CustomSettings._GT(typeof(Bounds)),
            CustomSettings._GT(typeof(Color)),
            CustomSettings._GT(typeof(LayerMask)),
            CustomSettings._GT(typeof(Mathf)),
            CustomSettings._GT(typeof(Plane)),
            CustomSettings._GT(typeof(Profiler)),
            CustomSettings._GT(typeof(Quaternion)),
            CustomSettings._GT(typeof(Ray)),
            CustomSettings._GT(typeof(RaycastHit)),
            CustomSettings._GT(typeof(Time)),
            CustomSettings._GT(typeof(Touch)),
            CustomSettings._GT(typeof(Vector2)),
            CustomSettings._GT(typeof(Vector3)),
            CustomSettings._GT(typeof(Vector4)),
            CustomSettings._GT(typeof(UnityEngine.Object)),
        };

        [MenuItem("Lua/Export EmmyLua API", false, 101)]
        public static void Gen()
        {
            if (Directory.Exists(ApiPath))
            {
                Directory.Delete(ApiPath, true);
            }
            Directory.CreateDirectory(ApiPath);
            
            GenCustom();
            Debug.Log($"Generate API at: {ApiPath}");
        }

        public static void GenCustom()
        {
            var binds = new List<BindType>(Extras);
            binds.AddRange(CustomSettings.customTypeList);
            foreach (var bindType in binds)
            {
                if (bindType.type.IsGenericType) continue;
                GenType(bindType);
            }
        }

        public static void GenType(BindType bindType)
        {
            var t = bindType.type;
            if (!CheckType(t)) return;
            
            var sb = new StringBuilder();
            
            GenTypeField(bindType, sb);

            sb.AppendFormat("---@class {0}", t);
            if (t.BaseType != null)
                sb.Append(CheckType(t.BaseType) ? $": {t.BaseType}\n" : "\n");
            sb.AppendFormat("local {0}={{ }}\n", ClassName);

            GenConstructorMethod(bindType, sb);
            GenTypeMethod(bindType, sb);
            if (bindType.extendList != null && bindType.extendList.Count > 0)
            {
                GenTypeExtendMethod(bindType, sb);
            }

            sb.AppendFormat("{0} = m\n", t);
            File.WriteAllText(ApiPath + t.FullName + ".lua", sb.ToString(), Encoding.UTF8);
        }

        private static bool CheckType(Type t)
        {
            if (t == null)
                return false;
            if (t.IsGenericTypeDefinition)
                return false;
            if (t.IsDefined(typeof(ObsoleteAttribute), false))
                return false;
            if (t == typeof(YieldInstruction))
                return false;
            if (t == typeof(Coroutine))
                return false;
            return !t.IsNested;
        }
        
        public static void GenTypeField(BindType bindType, StringBuilder sb)
        {
            var t = bindType.type;
            
            var members = new List<MemberInfo>();
            
            var fields = t.GetFields(BindingFlags.Public | BindingFlags.Static | BindingFlags.Instance | BindingFlags.DeclaredOnly);
            members.AddRange(fields);
            var properties = t.GetProperties(BindingFlags.Public | BindingFlags.Static | BindingFlags.Instance | BindingFlags.DeclaredOnly);
            members.AddRange(properties);
            
            foreach (var m in members)
            {
                if (m.IsDefined(typeof(NoToLuaAttribute), false) || 
                    m.IsDefined(typeof(ObsoleteAttribute), false)) 
                    continue;
                
                var typeName = "";
                if (m is FieldInfo f)
                {
                    typeName = GetLuaType(f.FieldType);
                }
                else if (m is PropertyInfo p)
                {
                    typeName = GetLuaType(p.PropertyType);
                }
                
                sb.AppendFormat("---@field public {0} {1}\n", m.Name, typeName);
            }
        }

        public static void GenConstructorMethod(BindType bindType, StringBuilder sb)
        {
            var t = bindType.type;
            var cis = t.GetConstructors();
            foreach (var c in cis)
            {
                var paramStr = new StringBuilder();
                foreach (var p in c.GetParameters())
                {
                    GenParameterInfoLine(p, sb, paramStr);
                }

                sb.AppendLine($"---@return {t.FullName}");
                sb.AppendLine($"function {ClassName}.{ConstrcutorName}({paramStr}) end");
            }
        }

        public static void GenTypeMethod(BindType bindType, StringBuilder sb)
        {
            var t = bindType.type;
            var methods = t.GetMethods(BindingFlags.Public | 
                BindingFlags.Static | 
                BindingFlags.Instance |
                BindingFlags.DeclaredOnly);
            foreach (var method in methods)
            {
                if (method.IsGenericMethod)
                    continue;
                if (method.IsDefined(typeof(NoToLuaAttribute), false))
                    continue;
                if (method.Name.StartsWith("get_") || method.Name.StartsWith("set_"))
                    continue;
                sb.AppendLine("---@public");
                var paramstr = new StringBuilder();
                
                var rts = new List<string>();

                if (method.ReturnType != typeof(void))
                {
                    rts.Add(GetLuaType(method.ReturnType));
                }
                
                foreach (var param in method.GetParameters())
                {
                    if (param.IsOut)
                    {
                        var typeDesc = GetLuaType(param.ParameterType);
                        rts.Add(typeDesc);
                        continue;
                    }
                    GenParameterInfoLine(param, sb, paramstr);
                }

                if (rts.Count > 0)
                {
                    sb.Append("---@return ");
                    for (var i = 0; i < rts.Count; i++)
                    {
                        sb.Append(i != rts.Count - 1 ? $"{rts[i]}," : $"{rts[i]}\n");
                    }
                }
                
                sb.AppendFormat("function {0}{1}{2}({3}) end\n", ClassName, method.IsStatic ? "." : ":", method.Name, paramstr);
            }
        }

        private static void GenParameterInfoLine(ParameterInfo param, StringBuilder sb, StringBuilder paramstr)
        {
            sb.AppendFormat("---@param {0} {1}\n", param.Name, GetLuaType(param.ParameterType));
            paramstr.Append(paramstr.Length != 0 ? $", {param.Name}" : param.Name);
        }

        private static void GenTypeExtendMethod(BindType bindType, StringBuilder sb)
        {
            var t = bindType.type;
            var extMethods = new List<MethodInfo>();
            foreach (var ext in bindType.extendList)
            {
                var mts = ext.GetMethods(BindingFlags.Public | BindingFlags.Static | BindingFlags.DeclaredOnly);
                foreach (var mt in mts)
                {
                    if (!mt.IsDefined(typeof(ExtensionAttribute), false))
                    {
                        continue;
                    }

                    var plist = mt.GetParameters();
                    var firstType = plist[0].ParameterType;

                    if (t.BaseType != null  
                        && (firstType == t || firstType.IsAssignableFrom(t) || IsGenericType(mt, t))
                        && (t == t.BaseType || t.IsSubclassOf(t.BaseType)))
                    {
                        extMethods.Add(mt);
                    }
                }
            }

            foreach (var method in extMethods)
            {
                sb.AppendLine("---@public");
                var paramstr = new StringBuilder();
                var paramList = method.GetParameters();
                
                for (var i = 1; i < paramList.Length; i++)
                {
                    var param = paramList[i];
                    sb.AppendFormat("---@param {0} {1}\n", param.Name, GetLuaType(param.ParameterType));
                    if (paramstr.Length != 0)
                    {
                        paramstr.Append(", ");
                    }
                    paramstr.Append(param.Name);
                }
                sb.AppendFormat("---@return {0}\n", method.ReturnType == typeof(void) ? "void" : GetLuaType(method.ReturnType));
                sb.AppendFormat("function {0}:{1}({2}) end\n", ClassName, method.Name, paramstr);
            }
        }

        private static string GetLuaType(Type t)
        {
            string desc;
            if (t.IsEnum
                || t == typeof(ulong)
                || t == typeof(long)
                || t == typeof(float)
                || t == typeof(double)
                || t == typeof(byte)
                || t == typeof(ushort)
                || t == typeof(short))
                desc = "number";
            else if (t == typeof(int) || t == typeof(uint))
                desc = "integer";
            else if (t == typeof(bool))
                desc = "boolean";
            else if (t == typeof(string))
                desc = "string";
            else if (t == typeof(void))
                desc = "void";
            else
            {
                if (t.IsGenericType)
                {
                    desc = GetGenericLuaType(t);
                }
                else if (t.FullName != null && t.BaseType == typeof(Array))
                {
                    var typeName = t.FullName.Replace("[]", string.Empty);
                    var arrayType = t.Assembly.GetType(typeName);
                    desc = $"{GetLuaType(arrayType)}[]";
                }
                else
                {
                    desc = t.FullName;
                }
                
                if (desc != null) desc = Regex.Replace(desc, "&", "");
            }

            return desc;
        }

        private static string GetGenericLuaType(Type t)
        {
            var gt = t.GetGenericTypeDefinition();
            if (gt == typeof(List<>))
            {
                var ct = t.GenericTypeArguments[0];
                return $"List<{GetLuaType(ct)}>";
            }
            return t.FullName;
        }

        private static bool IsGenericType(MethodInfo mt, Type t)
        {
            return mt.GetGenericArguments().Any(t1 => t1 == t);
        }

        public static void GenerateLuaCode(string path, BindableValues vals)
        {
            var file = new FileInfo(path);
            if (!file.Exists) return;

            var className = file.Name.Replace(".lua", "");

            var builder = new StringBuilder();

            vals.Init();
            var dict = vals.valDict;

            var ps = LuaParser.Instance.GetClass(path)?.Properties;

            foreach (var kv in dict)
            {
                if (kv.Value != null)
                {
                    var type = GetLuaType(kv.Value.GetType());
                    if (ps != null && ps.TryGetValue(kv.Key, out var p) && p.typeName == type)
                        builder.AppendLine(p.lineStr);
                    else
                        builder.AppendFormat("---@field public {0} {1}\n", kv.Key, type);
                }
            }

            var lines = ParseUtil.GetLineInfo(path);
            var sw = new StreamWriter(path, false);
            LineInfo lineClass = null;
            for (var i = 0; i < lines.Count; i++)
            {
                var line = lines[i];
                if (line.type == LineType.Class)
                {
                    lineClass = line;
                }
                else if (line.type == LineType.DefineClass)
                {
                    if (line.str.Contains(className + " =") || line.str.Contains(className + "="))
                    {
                        sw.Write(builder.ToString());
                        if (lineClass == null)
                            sw.WriteLine("---@class {0}", className);
                        else
                            sw.WriteLine(lineClass.str);

                        sw.WriteLine(line.str);
                    }
                    else
                    {
                        if (i - 1 > 0 && lines[i - 1].type == LineType.Class)
                        {
                            sw.WriteLine(lines[i - 1].str);
                        }

                        sw.WriteLine(line.str);
                    }
                }
                else if(line.type != LineType.PublicProperty)
                {
                    sw.WriteLine(line.str);
                }
            }

            sw.Close();
            sw.Dispose();

            Debug.Log($"Generate code at: {path}");
        }

    }
}
