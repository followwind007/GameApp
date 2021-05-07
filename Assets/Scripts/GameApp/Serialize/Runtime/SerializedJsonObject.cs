using System;
using Object = UnityEngine.Object;

namespace GameApp.Serialize
{
    [Serializable]
    public struct SerializedJsonObject
    {
        public string name;
        public string type;
        public string data;
        public Object obj;

        public static string IdName => nameof(name);
        public static string IdType => nameof(type);
        public static string IdData => nameof(data);
        public static string IdObj => nameof(obj);

        public Type T => GetDataType();

        public SerializedJsonObject(object value) : this(null, null, value) { }

        public SerializedJsonObject(Type t, object value = null) : this(null, t, value) { }
        
        public SerializedJsonObject(string name, Type t, object value)
        {
            data = JsonUtilityWrap.ToJson(value);
            if (t != null)
                type = t.FullName;
            else if (value != null)
                type = value.GetType().FullName;
            else
                throw new ArgumentException("can not find type info from t or value");

            obj = value is Object o ? o : null;
            this.name = name;
        }

        public T GetData<T>()
        {
            return string.IsNullOrEmpty(data) ? default : JsonUtilityWrap.FromJson<T>(data);
        }

        public object GetData()
        {
            return T.IsSubclassOf(typeof(Object)) ? obj : JsonUtilityWrap.FromJson(data, T);
        }

        public Type GetDataType()
        {
            return Type.GetType(type);
        }

        public void SaveDataStr(object value)
        {
            type = value.GetType().ToString();
            data = value.ToString();
        }

        public void Save(object value)
        {
            if (value is Object o)
            {
                obj = o;
                return;
            }

            data = JsonUtilityWrap.ToJson(value);
        }

    }
}