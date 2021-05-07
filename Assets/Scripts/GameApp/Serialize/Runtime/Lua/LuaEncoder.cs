using System;
using System.Collections;
using System.Collections.Generic;
using System.Reflection;
using System.Runtime.Serialization;
using System.Text;

namespace GameApp.Serialize
{
    public static class LuaEncoder
    {
        public static string EncodeToLua(this object item)
        {
            var stringBuilder = new StringBuilder();
            AppendValue(stringBuilder, item, "", "");
            return stringBuilder.ToString();
        }

        private static void AppendValue(StringBuilder stringBuilder, object item, string desc, string indent)
        {
            if (item == null)
            {
                stringBuilder.AppendContent($"{desc}nil,", indent);
                return;
            }

            var type = item.GetType();
            var valStr = ValToString(item);
            
            if (!string.IsNullOrEmpty(valStr))
            {
                stringBuilder.AppendContent($"{desc}{valStr},", indent);
            }
            else
            {
                stringBuilder.AppendContent($"{desc}{{", indent);
                
                if (item is IList list)
                {
                    foreach (var t in list) AppendValue(stringBuilder, t, "", NextIndent(indent));
                }
                else if (type.IsGenericType && type.GetGenericTypeDefinition() == typeof(Dictionary<,>))
                {
                    if (item is IDictionary dict)
                    {
                        foreach (var key in dict.Keys)
                        {
                            var keyStr = ValToString(key);
                            if (!string.IsNullOrEmpty(keyStr)) AppendValue(stringBuilder, dict[key], $"[{keyStr}] = ", indent);
                        }
                    }
                }
                else
                {
                    var fieldInfos = type.GetFields(BindingFlags.Instance | BindingFlags.Public | BindingFlags.FlattenHierarchy);
                    foreach (var t in fieldInfos)
                    {
                        CheckMember(t, item, stringBuilder, indent);
                    }
                    var propertyInfo = type.GetProperties(BindingFlags.Instance | BindingFlags.Public | BindingFlags.FlattenHierarchy);
                    foreach (var t in propertyInfo)
                    {
                        CheckMember(t, item, stringBuilder, indent);
                    }
                }
                
                stringBuilder.AppendContent(string.IsNullOrEmpty(indent) ? "}" : "},", indent);
            }
        }

        private static void CheckMember(MemberInfo t, object item, StringBuilder stringBuilder, string indent)
        {
            if (t.IsDefined(typeof(IgnoreDataMemberAttribute), true)) return;
            object value = null;
            
            if (t is FieldInfo f)
                value = f.GetValue(item);
            else if (t is PropertyInfo p)
                value = p.GetValue(item);

            if (value == null) return;
                        
            if (t.IsDefined(typeof(NotExportEmptyAttribute)) || value is IList l && l.Count == 0 || value is IDictionary d && d.Count == 0) return;
                        
            AppendValue(stringBuilder, value, $"{GetMemberName(t)} = ", NextIndent(indent));
        }

        private static string ValToString(object val)
        {
            var type = val.GetType();
            var str = "";
            if (type == typeof(string))
            {
                var stringBuilder = new StringBuilder();
                stringBuilder.Append('"');
                foreach (var t in (string)val)
                {
                    if (t < ' ' || t == '"' || t == '\\')
                    {
                        stringBuilder.Append('\\');
                        var j = "\"\\\n\r\t\b\f".IndexOf(t);
                        if (j >= 0)
                            stringBuilder.Append("\"\\nrtbf"[j]);
                        else
                            stringBuilder.AppendFormat("u{0:X4}", (uint)t);
                    }
                    else
                    {
                        stringBuilder.Append(t);
                    }
                }
                stringBuilder.Append('"');
                str = stringBuilder.ToString();
            }
            else if (type == typeof(byte) || type == typeof(int))
            {
                str = val.ToString();
            }
            else if (type == typeof(float))
            {
                str = ((float)val).ToString(System.Globalization.CultureInfo.InvariantCulture);
            }
            else if (type == typeof(double))
            {
                str = ((double)val).ToString(System.Globalization.CultureInfo.InvariantCulture);
            }
            else if (type == typeof (decimal)) {
                str = ((decimal)val).ToString (System.Globalization.CultureInfo.InvariantCulture);
            }
            else if (type == typeof(bool))
            {
                str = (bool)val ? "true" : "false";
            }
            else if (type.IsEnum)
            {
                var attr = type.GetCustomAttribute<EnumValueAttribute>();
                if (attr != null && attr.saveType == EnumSaveType.Int)
                    str = $"{(int)val}";
                else
                    str = $"\"{val}\"";
            }

            return str;
        }

        private static string GetMemberName(MemberInfo member)
        {
            if (member.IsDefined(typeof(DataMemberAttribute), true))
            {
                var dataMemberAttribute = (DataMemberAttribute)Attribute.GetCustomAttribute(member, typeof(DataMemberAttribute), true);
                if (!string.IsNullOrEmpty(dataMemberAttribute.Name))
                    return dataMemberAttribute.Name;
            }

            return member.Name;
        }

        private static string NextIndent(string indent)
        {
            return $"{indent}    ";
        }

        private static void AppendContent(this StringBuilder builder, string content, string indent)
        {
            builder.AppendLine($"{indent}{content}");
        }
        
    }
}