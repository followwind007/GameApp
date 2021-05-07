using System.Collections.Generic;
using System.IO;
using System.Text.RegularExpressions;

namespace GameApp.LuaResolver
{
    public static class ParseUtil
    {
        private static readonly Regex RegClass = new Regex(@"(\w+_*(\w|\d|\.)+)+");
        private static readonly Regex RegClassName = new Regex(@"---@class\s+(\w+_*(\w|\d|\.)+)+\s*");
        private static readonly Regex RegClassParent = new Regex(@":\s*(\w+_*(\w|\d|\.)+)+");
        private static readonly Regex RegFuncSelfName = new Regex(@"function\s+(\w+(_*(\w|\d)+)*)");
        private static readonly Regex RegDefineFunction = new Regex(@"function\s+(\w|\d|_)+(:|\.)(\w|\d|_)+");

        public static string GetClassName(string line, out List<string> parents)
        {
            parents = new List<string>();
            var pm = RegClassParent.Match(line);
            if (pm.Success)
            {
                parents.Add(Regex.Replace(pm.Value, @":\s*", ""));
            }
            
            var match = RegClassName.Match(line);

            var clsName = Regex.Replace(match.Value, @"---@class\s+", "");
            clsName = clsName.Replace(" ", "");
            return clsName;
        }

        public static string GetClassDefineName(string line)
        {
            var endIndex = line.IndexOf('=');
            if (endIndex > 0)
            {
                var pre = line.Substring(0, endIndex);
                pre = pre.Replace(" ", "");
                pre = pre.Replace("local", "");
                return pre;
            }
            return null;
        }

        public static PropertyInfo GetPropertyInfo(string line)
        {
            var ti = GetTypeInfo(line);
            if (ti == null) return null;

            var prop = new PropertyInfo
            {
                name = ti.name,
                typeName = ti.typeName,
                literalDescs = ti.literalDescs,
                desc = ti.desc,
                isPublic = line.Contains(Reserves.FieldPublic),
                lineStr = line
            };
            
            return prop;
        }

        public static TypeInfo GetTypeInfo(string line)
        {
            var isPublic = line.Contains(Reserves.FieldPublic);
            var regex = isPublic ? new Regex(@"---@\w+\s+public\s+") : new Regex(@"---@\w+\s+");
            line = regex.Replace(line, "");
            
            var descStart = line.IndexOf('@');
            var desc = descStart >= 0 ? line.Substring(descStart + 1) : null;
            
            var sub = line.Substring(0, descStart > 0 ? descStart : line.Length);

            var literals = DataBinder.LiteralUtil.DealPropertyLiteral(sub);

            var parts = sub.Split(' ');
            if (parts.Length < 2) return null;

            var ti = new TypeInfo
            {
                name = parts[0],
                typeName = parts[1],
                literalDescs = literals,
                desc = desc
            };
            
            return ti;
        }

        public static string GetFunctionSelfName(string line)
        {
            var match = RegFuncSelfName.Match(line);
            return Regex.Replace(match.Value, @"function\s+", "");
        }
        
        public static string GetFunctionName(string line)
        {
            var mv = Regex.Match(line, @"(\w+_*\d*)\(.*\)").Value;
            return Regex.Replace(mv, @"\(.*\)", "");
        }

        public static ReturnInfo GetReturnInfo(string line)
        {
            line = Regex.Replace(line, @"---@return\s+", "");
            
            var descStart = line.IndexOf('@');
            var desc = descStart >= 0 ? line.Substring(descStart) : null;

            var sub = line.Substring(0, descStart >= 0 ? descStart : line.Length);

            var types = new List<string>();

            var mts = RegClass.Matches(sub);
            foreach (Match mt in mts)
            {
                types.Add(mt.Value);
            }

            var ri = new ReturnInfo
            {
                typeNames = types,
                desc = desc,
            };
            
            return ri;
        }

        public static string GetSummary(string line)
        {
            return Regex.Replace(line, @"---@summary\s+", "");
        }

        public static List<string> GetTags(string line)
        {
            var tagStr = Regex.Replace(line, @"---@tag\s+", "");
            var tags = tagStr.Split(',');
            return new List<string>(tags);
        }
        
        public static List<LineInfo> GetLineInfo(string[] ls)
        {
            var lines = new List<LineInfo>();

            for (var i = 0; i < ls.Length; i++)
            {
                var line = ls[i];
                var l = new LineInfo {str = line, line = i};
                
                if (string.IsNullOrEmpty(line))
                {
                    l.type = LineType.Content;
                }
                else if (line.StartsWith(Reserves.Field))
                {
                    var isPublic = line.Contains(Reserves.FieldPublic);
                    l.type = isPublic ? LineType.PublicProperty : LineType.Property;
                }
                else if (line.StartsWith(Reserves.ClassType))
                {
                    l.type = LineType.ClassType;
                }
                else if (line.StartsWith(Reserves.Class))
                {
                    l.type = LineType.Class;
                }
                else if (Regex.IsMatch(line, @"Class\(.*\)"))
                {
                    l.type = LineType.DefineClass;
                }
                else if (RegDefineFunction.IsMatch(line))
                {
                    l.type = LineType.Function;
                }
                else if (line.StartsWith(Reserves.Tag))
                {
                    l.type = LineType.Tag;
                }
                else if (line.StartsWith(Reserves.Annotation))
                {
                    l.type = LineType.Annotation;
                }
                else
                {
                    l.type = LineType.Content;
                }
                
                lines.Add(l);
            }

            return lines;
        }

        public static List<LineInfo> GetLineInfo(string path)
        {
            var ls = File.ReadAllLines(path);
            return GetLineInfo(ls);
        }

        public static string GetTypeShortName(string t)
        {
            var parts = t.Split('.');
            return parts[parts.Length - 1];
        }

        public static bool IsAnnotated(string s)
        {
            return s.StartsWith("--");
        }

    }
}