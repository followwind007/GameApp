using System.Collections.Generic;
using System.IO;
using GameApp.LuaResolver;

namespace GameApp.DataBinder
{
    public class BindableInfo
    {
        public enum BindType
        {
            Default, Static
        }

        public BindType bindType = BindType.Default;

        public readonly Dictionary<string, ValueWrap> wraps = new Dictionary<string, ValueWrap>();

        public LuaClass cls;

        public Dictionary<string, PropertyInfo> Properties => cls?.Properties;
        public IEnumerable<PropertyInfo> PropertyList => cls?.propertyList;

        public static BindableInfo FetchField(string path)
        {
            var bInfo = new BindableInfo();
            if (string.IsNullOrEmpty(path))
            {
                return bInfo;
            }
            var resolver = ClassResolver.Instance;
            resolver.Resolve(new FileInfo(path));
            if (resolver.classes.Count == 0) return bInfo;
            bInfo.cls = resolver.classes[0];

            foreach (var p in bInfo.Properties.Values)
            {
                var w = new ValueWrap
                {
                    name = p.name,
                    type = GetValueType(p.typeName)
                };
                p.literals = LiteralUtil.GetLiteralValues(p.literalDescs, w.type);
                bInfo.wraps.Add(w.name, w);
            }

            return bInfo;
        }

        private static ValueType GetValueType(string type)
        {
            ValueType t;
            switch (type)
            {
                case "number":
                    t = ValueType.Float;
                    break;
                case "integer":
                    t = ValueType.Int;
                    break;
                case "boolean":
                    t = ValueType.Bool;
                    break;
                case "string":
                    t = ValueType.String;
                    break;
                case "UnityEngine.Vector2":
                    t = ValueType.Vector2;
                    break;
                case "UnityEngine.Vector3":
                    t = ValueType.Vector3;
                    break;
                case "UnityEngine.Vector4":
                    t = ValueType.Vector4;
                    break;
                case "UnityEngine.Bounds":
                    t = ValueType.Bounds;
                    break;
                case "UnityEngine.Rect":
                    t = ValueType.Rect;
                    break;
                case "UnityEngine.Color":
                    t = ValueType.Color;
                    break;
                case "UnityEngine.AnimationCurve":
                    t = ValueType.Curve;
                    break;
                default:
                    t = ValueType.Object;
                    break;
            }

            return t;
        }

    }
}