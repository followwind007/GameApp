
using System;
using System.Collections.Generic;
using System.Text;
using LuaInterface;

namespace Tools.Table
{
    public enum ValueType
    {
        Int = 0,
        Float = 1,
        Bool = 2,
        String = 3,
        Elua = 4,
    }

    public class TableEntry
    {
        public readonly Dictionary<string, object> fieldDict = new Dictionary<string, object>();

        public T GetField<T>(string fieldName)
        {
            if (fieldDict.ContainsKey(fieldName))
            {
                return (T)fieldDict[fieldName];
            }
            return default(T);
        }

        public void AddFieldWithString<T>(string name, string value)
        {
            if (string.IsNullOrEmpty(name)) return;
            object obj = null;

            if (typeof(T) == typeof(int))
                obj = Convert.ToInt32(value);
            else if (typeof(T) == typeof(float))
                obj = Convert.ToDouble(value);
            else if (typeof(T) == typeof(bool))
                obj = Convert.ToBoolean(value);
            else if (typeof(T) == typeof(string))
                obj = value;
            else if (typeof(T) == typeof(LuaTable))
                obj = value;

            fieldDict[name] = obj;
        }

        public void AddField(string name, object value)
        {
            fieldDict[name] = value;
        }

    }

    public class TableObject
    {
        //public string Name { get; private set; }

        public Dictionary<object, TableEntry> TableDict { get; private set; }

        private Dictionary<string, ValueType> _colmnType = new Dictionary<string, ValueType>();

        public TableObject()
        {
            TableDict = new Dictionary<object, TableEntry>();
        }

        public T GetField<T>(int id, string fieldName)
        {
            if (TableDict.ContainsKey(id))
            {
                return TableDict[id].GetField<T>(fieldName);
            }
            return default(T);
        }

        public TableEntry GetEntry(object id)
        {
            if (TableDict.ContainsKey(id))
            {
                return TableDict[id];
            }
            return null;
        }

        public void AddEntry(object id, TableEntry entry)
        {
            TableDict[id] = entry;
        }

        public void RemoveEntry(object id)
        {
            TableDict.Remove(id);
        }

        public void Clear()
        {
            TableDict.Clear();
        }

        public ValueType GetColumnType(string name)
        {
            ValueType type;
            _colmnType.TryGetValue(name, out type);
            return type;
        }

        public void SetColumnType(Dictionary<string, ValueType> map)
        {
            _colmnType = map;
        }

        public override string ToString()
        {
            var builder = new StringBuilder();
            builder.AppendFormat("row: {0}\n", TableDict.Count);
            foreach (var entry in TableDict)
            {
                foreach (var field in entry.Value.fieldDict)
                {
                    builder.AppendFormat("{0}: {1};\t", field.Key, field.Value);
                }
                builder.Append("\n");
            }
            return builder.ToString();
        }

    }
}