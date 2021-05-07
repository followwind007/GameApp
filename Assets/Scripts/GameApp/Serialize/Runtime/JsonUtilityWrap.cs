using System;
using System.Text;
using UnityEngine;

namespace GameApp.Serialize
{
    public static class JsonUtilityWrap
    {
        public static readonly Converter<AnimationCurve, string> CurveToStr = c => "";
        
        public static string ToJson(object obj)
        {
            if (obj == null) return null;
            string str = null;
            
            if (obj is int || obj is float || obj is bool || obj is string)
            {
                str = obj.ToString();
            }
            else if (obj is Color color)
            {
                var fs = Shrink(new [] { color.r, color.g, color.b, color.a });
                str = Assemble(fs);
            }
            else if (obj is AnimationCurve _)
            {
            }
            else
            {
                str = JsonUtility.ToJson(obj);
            }
            return str;
        }

        public static T FromJson<T>(string json)
        {
            if (string.IsNullOrEmpty(json))
            {
                return default;
            }
            return (T) FromJson(json, typeof(T));
        }

        public static object FromJson(string json, Type type)
        {
            if (string.IsNullOrEmpty(json))
            {
                return null;
            }
            
            object obj;

            if (type == typeof(int))
            {
                obj = Convert.ToInt32(json);
            }
            else if (type == typeof(float))
            {
                obj = Convert.ToSingle(json);
            }
            else if (type == typeof(bool))
            {
                obj = Convert.ToBoolean(json);
            }
            else if (type == typeof(string))
            {
                obj = Convert.ToString(json);
            }
            else if (type == typeof(Color))
            {
                var fs = DessembleFloat(json);
                obj = new Color(fs[0], fs[1], fs[2], fs[3]);
            }
            else
            {
                obj = JsonUtility.FromJson(json, type);
            }

            return obj;
        }

        public static float Shrink(float f)
        {
            return (int) (f * 10000) / 10000f;
        }

        public static float[] Shrink(float[] fs)
        {
            for (var i = 0; i < fs.Length; i++)
            {
                fs[i] = Shrink(fs[i]);
            }

            return fs;
        }

        public static string Assemble<T>(T[] values)
        {
            var sb = new StringBuilder();
            for (var i = 0; i < values.Length; i++)
            {
                sb.Append(i == values.Length - 1 ? $"{values[i]}" : $"{values[i]},");
            }
            return sb.ToString();
        }

        public static int[] DessembleInt(string str)
        {
            var parts = GetParts(str);
            var values = new int[parts.Length];

            for (var i = 0; i < parts.Length; i++)
            {
                var p = parts[i];
                values[i] = Convert.ToInt32(p);
            }

            return values;
        }
        
        private static float[] DessembleFloat(string str)
        {
            var parts = GetParts(str);
            var values = new float[parts.Length];

            for (var i = 0; i < parts.Length; i++)
            {
                var p = parts[i];
                values[i] = Convert.ToSingle(p);
            }

            return values;
        }

        private static string[] GetParts(string str)
        {
            return str.Split(',');
        }
        
    }
}