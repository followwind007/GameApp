using System.Collections;
using UnityEngine;
using System.Collections.Generic;

namespace GameApp.DataBinder
{
    public enum ValueType
    {
        Int = 0,
        Float = 1,
        String = 2,
        Vector2 = 3,
        Vector3 = 4,
        Vector4 = 5,
        Rect = 6,
        Bounds = 7,
        Color = 8,
        Curve = 9,
        Bool = 10,
        Object = 11,
    }

    [System.Serializable]
    public class ValueWrap
    {
        public ValueType type = ValueType.Int;
        public string name;
        public int index;
        [System.NonSerialized]
        public object value;
    }

    [System.Serializable]
    public class BindableValues : ISerializationCallbackReceiver
    {
        [SerializeField] private List<int> ints = new List<int>();

        [SerializeField] private List<float> floats = new List<float>();

        [SerializeField] private List<string> strings = new List<string>();

        [SerializeField] private List<Vector2> vector2 = new List<Vector2>();

        [SerializeField] private List<Vector3> vector3 = new List<Vector3>();

        [SerializeField] private List<Vector4> vector4 = new List<Vector4>();

        [SerializeField] private List<Rect> rects = new List<Rect>();

        [SerializeField] private List<Bounds> bounds = new List<Bounds>();

        [SerializeField] private List<Color> colors = new List<Color>();

        [SerializeField] private List<AnimationCurve> curves = new List<AnimationCurve>();

        [SerializeField] private List<bool> bools = new List<bool>();

        [SerializeField] private List<Object> objects = new List<Object>();

        [SerializeField] public List<ValueWrap> wraps = new List<ValueWrap>();
        
        public readonly Dictionary<string, object> valDict = new Dictionary<string, object>();
        private IEnumerator _enumerator;

        public object this[string key] => valDict[key];

        public object GetData(string key) => valDict[key];
        public T GetData<T>(string key) => (T)valDict[key];

        public IEnumerator GetDataEnumerator()
        {
             _enumerator = valDict.GetEnumerator();
             return _enumerator;
        }

        public void Init()
        {
            valDict.Clear();
            foreach (var v in wraps)
            {
                switch (v.type)
                {
                    case ValueType.Int:
                        v.value = ints.Count > v.index ? ints[v.index] : default;
                        break;
                    case ValueType.Float:
                        v.value = floats.Count > v.index ? floats[v.index] : default;
                        break;
                    case ValueType.String:
                        v.value = strings.Count > v.index ? strings[v.index] : default;
                        break;
                    case ValueType.Vector2:
                        v.value = vector2.Count > v.index ? vector2[v.index] : default;
                        break;
                    case ValueType.Vector3:
                        v.value = vector3.Count > v.index ? vector3[v.index] : default;
                        break;
                    case ValueType.Vector4:
                        v.value = vector4.Count > v.index ? vector4[v.index] : default;
                        break;
                    case ValueType.Rect:
                        v.value = rects.Count > v.index ? rects[v.index] : default;
                        break;
                    case ValueType.Bounds:
                        v.value = bounds.Count > v.index ? bounds[v.index] : default;
                        break;
                    case ValueType.Color:
                        v.value = colors.Count > v.index ? colors[v.index] : default;
                        break;
                    case ValueType.Curve:
                        v.value = curves.Count > v.index ? curves[v.index] : default;
                        break;
                    case ValueType.Bool:
                        v.value = bools.Count > v.index && bools[v.index];
                        break;
                    case ValueType.Object:
                        v.value = objects.Count > v.index ? objects[v.index] : null;
                        break;
                }
                valDict[v.name] = v.value;
            }
        }

        public static object GetDefault(ValueType type)
        {
            switch (type)
            {
                case ValueType.Int:
                    return default(int);
                case ValueType.Float:
                    return default(float);
                case ValueType.String:
                    return default(string);
                case ValueType.Vector2:
                    return default(Vector2);
                case ValueType.Vector3:
                    return default(Vector3);
                case ValueType.Vector4:
                    return default(Vector4);
                case ValueType.Rect:
                    return default(Rect);
                case ValueType.Bounds:
                    return default(Bounds);
                case ValueType.Color:
                    return default(Color);
                case ValueType.Curve:
                    return new AnimationCurve();
                case ValueType.Bool:
                    return default(bool);
                case ValueType.Object:
                    return null;
                default: return null;
            }
        }
        
        public void ClearAll()
        {
            ints?.Clear();
            bools?.Clear();
            rects?.Clear();
            bounds?.Clear();
            colors?.Clear();
            curves?.Clear();
            floats?.Clear();
            objects?.Clear();
            strings?.Clear();
            vector2?.Clear();
            vector3?.Clear();
            vector4?.Clear();
        }
        
        public void Regenerate(IEnumerable<ValueWrap> newWraps = null)
        {
            if (newWraps == null)
            {
                newWraps = wraps;
            }
            ClearAll();
            foreach (var wrap in newWraps)
            {
                var count = 0;
                switch (wrap.type)
                {
                    case ValueType.Int:
                        ints.Add((int?) wrap.value ?? default);
                        count = ints.Count;
                        break;
                    case ValueType.Float:
                        floats.Add((float?) wrap.value ?? default);
                        count = floats.Count;
                        break;
                    case ValueType.String:
                        strings.Add(wrap.value as string);
                        count = strings.Count;
                        break;
                    case ValueType.Vector2:
                        vector2.Add((Vector2?) wrap.value ?? default);
                        count = vector2.Count;
                        break;
                    case ValueType.Vector3:
                        vector3.Add((Vector3?) wrap.value ?? default);
                        count = vector3.Count;
                        break;
                    case ValueType.Vector4:
                        vector4.Add((Vector4?) wrap.value ?? default);
                        count = vector4.Count;
                        break;
                    case ValueType.Rect:
                        rects.Add((Rect?) wrap.value ?? default);
                        count = rects.Count;
                        break;
                    case ValueType.Bounds:
                        bounds.Add((Bounds?) wrap.value ?? default);
                        count = bounds.Count;
                        break;
                    case ValueType.Color:
                        colors.Add((Color?) wrap.value ?? default);
                        count = colors.Count;
                        break;
                    case ValueType.Curve:
                        curves.Add(wrap.value as AnimationCurve);
                        count = curves.Count;
                        break;
                    case ValueType.Bool:
                        bools.Add((bool?) wrap.value ?? default);
                        count = bools.Count;
                        break;
                    case ValueType.Object:
                        objects.Add(wrap.value as Object);
                        count = objects.Count;
                        break;
                }
                wrap.index = count - 1;
            }
        }
        
        public void OnBeforeSerialize() { }

        public void OnAfterDeserialize()
        {
            Init();
        }
        
    }
}
