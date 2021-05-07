#if UNITY_EDITOR
using System;
using System.Collections.Generic;
using System.Text;
using LuaInterface;

namespace ToolEditor.EvePro
{
    public class TableEntry
    {
        public Dictionary<string, object> fieldDict = new Dictionary<string, object>();

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
            else if (typeof(T) == typeof(string))
                obj = value;
            else if (typeof(T) == typeof(LuaTable))
                //obj = UnityEditorLua.GetLuaData<LuaTable>(value);
                obj = value;

            fieldDict[name] = obj;
        }

        public void AddField<T>(string name, object value)
        {
            fieldDict[name] = value;
        }

    }

    public class TableObject
    {
        public string Name { get; private set; }

        private Dictionary<object, TableEntry> _tableDict = new Dictionary<object, TableEntry>();

        public T GetField<T>(int id, string fieldName)
        {
            if (_tableDict.ContainsKey(id))
            {
                return _tableDict[id].GetField<T>(fieldName);
            }
            return default(T);
        }

        public TableEntry GetEntry(int id)
        {
            if (_tableDict.ContainsKey(id))
            {
                return _tableDict[id];
            }
            return null;
        }

        public void AddEntry(int id, TableEntry entry)
        {
            _tableDict[id] = entry;
        }

        public void RemoveEntry(int id)
        {
            _tableDict.Remove(id);
        }

        public void Clear()
        {
            _tableDict.Clear();
        }

        public override string ToString()
        {
            StringBuilder builder = new StringBuilder();
            builder.AppendFormat("row: {0}\n", _tableDict.Count);
            foreach (var entry in _tableDict)
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
#endif