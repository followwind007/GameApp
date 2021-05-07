using System;
using System.Collections.Generic;
using System.Text.RegularExpressions;
using UnityEngine;

namespace GameApp.DataBinder
{
    public static class LiteralUtil
    {
        private static readonly Regex PackRegex = new Regex(@"\((.+?)\)", RegexOptions.Singleline);
        
        public static List<string> DealPropertyLiteral(string line)
        {
            var literals = new List<string>();
            if (string.IsNullOrEmpty(line)) return literals;

            var start = line.IndexOf('|');
            if (start < 0) return literals;

            var sub = line.Substring(start + 1);
            var parts = sub.Split('|');
            foreach (var p in parts)
            {
                if (string.IsNullOrEmpty(p)) continue;
                var p1 = Regex.Replace(p, "(\"|\\s)*", "");
                if (!string.IsNullOrEmpty(p1)) literals.Add(p1);
            }
            
            return literals;
        }

        public static List<object> GetLiteralValues(List<string> literals, ValueType type)
        {
            var vals = new List<object>();
            if (literals == null || literals.Count < 1) return vals;
            foreach (var literal in literals)
            {
                switch (type)
                {
                    case ValueType.Int:
                        vals.Add(Convert.ToInt32(literal));
                        break;
                    case ValueType.Float:
                        vals.Add(Convert.ToSingle(literal));
                        break;
                    case ValueType.String:
                        vals.Add(GetString(literal));
                        break;
                    case ValueType.Vector2:
                        vals.Add(GetVector2(literal));
                        break;
                    case ValueType.Vector3:
                        vals.Add(GetVector3(literal));
                        break;
                    case ValueType.Vector4:
                        vals.Add(GetVector4(literal));
                        break;
                    case ValueType.Rect:
                        vals.Add(GetRect(literal));
                        break;
                    case ValueType.Bounds:
                        vals.Add(GetBounds(literal));
                        break;
                    case ValueType.Color:
                        vals.Add(GetColor(literal));
                        break;
                    case ValueType.Curve:
                        break;
                    case ValueType.Bool:
                        break;
                    case ValueType.Object:
                        break;
                    default:
                        throw new ArgumentOutOfRangeException();
                }
            }
            return vals;
        }

        private static string GetString(string literal)
        {
            return Regex.Replace(literal, "(\'|\")*", "");
        }

        private static Vector2 GetVector2(string literal)
        {
            var list = UnPack(literal);
            return list.Count >= 2 ? new Vector2(list[0], list[1]) : Vector2.zero;
        }

        private static Vector3 GetVector3(string literal)
        {
            var list = UnPack(literal);
            return list.Count >= 3 ? new Vector3(list[0], list[1], list[2]) : Vector3.zero;
        }
        
        private static Vector4 GetVector4(string literal)
        {
            var list = UnPack(literal);
            return list.Count >= 4 ? new Vector4(list[0], list[1], list[2], list[3]) : Vector4.zero;
        }
        
        private static Color GetColor(string literal)
        {
            var list = UnPack(literal);
            return list.Count >= 3 ? new Color(list[0], list[1], list[2]) : Color.black;
        }
        
        private static Rect GetRect(string literal)
        {
            var list = UnPack(literal);
            return list.Count >= 4 ? new Rect(list[0], list[1], list[2], list[3]) : Rect.zero;
        }
        
        private static Bounds GetBounds(string literal)
        {
            var list = UnPack(literal);
            return list.Count >= 6 ? new Bounds(new Vector3(list[0], list[1], list[2]), new Vector3(list[3], list[4], list[5]) ) 
                : new Bounds(Vector3.zero, Vector3.one);
        }
        
        private static List<float> UnPack(string literal)
        {
            var args = new List<float>();
            var match = PackRegex.Match(literal);
            if (!match.Success) return args;
            var val = match.Value;
            val = val.Substring(1, val.Length - 2);
            var parts = val.Split(',');
            foreach (var p in parts)
            {
                var arg = p.Replace(" ", "");
                if (!string.IsNullOrEmpty(arg)) args.Add(Convert.ToSingle(arg));
            }
            return args;
        }

    }
}