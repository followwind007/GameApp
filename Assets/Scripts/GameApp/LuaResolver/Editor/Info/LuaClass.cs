using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text;
using Tools;

namespace GameApp.LuaResolver
{
    [Serializable]
    public class LuaClass
    {
        public List<string> parentNames = new List<string>();
        
        public List<LuaFunction> functions = new List<LuaFunction>();

        private Dictionary<string, LuaFunction> _functionDict;
        public Dictionary<string, LuaFunction> FunctionDict
        {
            get
            {
                if (_functionDict == null)
                {
                    _functionDict = new Dictionary<string, LuaFunction>();
                    functions.ForEach(f => { _functionDict[f.Id] = f; });
                }

                return _functionDict;
            }
        }

        private Dictionary<string, PropertyInfo> _properties;
        
        public List<PropertyInfo> propertyList = new List<PropertyInfo>();
        public Dictionary<string, PropertyInfo> Properties {
            get
            {
                if (_properties == null)
                {
                    _properties = new Dictionary<string, PropertyInfo>();
                    propertyList.ForEach(p => { _properties[p.name] = p; });
                }

                return _properties;
            }
        }
        
        private List<LineInfo> _lines;
        public List<LineInfo> Lines {
            get
            {
                if (_lines == null)
                {
                    var ls = File.ReadAllLines(file);
                    _lines = ParseUtil.GetLineInfo(ls);
                }

                return _lines;
            }
        }

        public string name;

        public string defineName;

        public string summary;
        public List<string> tags = new List<string>();

        public string file;

        public string ShortName => GetShortName();

        public string RelativePath => AssetUtil.GetRelativePath(file);

        public bool IsCsType => IsCSharpeType(name);

        public List<LuaFunction> GetFunction(string funcName)
        {
            return functions.Where(f => f.name == funcName).ToList();
        }
        
        public static bool IsCSharpeType(string type)
        {
            foreach (var assembly in AppDomain.CurrentDomain.GetAssemblies())
            {
                if (assembly.GetType(type) != null)
                {
                    return true;
                }
            }

            return false;
        }

        public IEnumerable<string> NameParts => name.Split('.');

        public override string ToString()
        {
            var sb = new StringBuilder();
            sb.AppendLine($"path: {AssetUtil.GetRelativePath(file)}");
            sb.AppendLine(parentNames.Count == 0 ? $"class: {name} ({defineName})" : $"class: {name} : {parentNames[0]} ({defineName})");
            foreach (var p in Properties.Values)
            {
                sb.AppendLine($"property: {p.typeName} {p.name}");
            }

            foreach (var f in functions)
            {
                sb.AppendLine($"function: {f}");
            }

            return sb.ToString();
        }

        public string GetShortName()
        {
            var parts = name.Split('.');
            return parts[parts.Length - 1];
        }
        
    }
}