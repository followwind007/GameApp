using System;
using System.Collections.Generic;
using UnityEngine;
using UnityEditor;

namespace Tools.Table
{
    [Serializable]
    public class TableColumn
    {
        [Serializable]
        public class Value
        {
            public int index;
            public string value;
            public object Val { get; set; }
        }
        
        public string name;
        public ValueType type;
        public List<string> enums = new List<string>();
        public List<Value> values = new List<Value>();
        
        public object this[int index]
        {
            get
            {
                if (!_inited) Init();
                return _dict.ContainsKey(index) ? _dict[index] : null;
            }
        }

        private bool _inited;
        private Dictionary<int, object> _dict;

        public void Init()
        {
            _inited = true;
            _dict = new Dictionary<int, object>();
            foreach (var v in values)
            {
                v.Val = v.value;
                switch (type)
                {
                    case ValueType.Int:
                        v.Val = Convert.ToInt32(v.value);
                        break;
                    case ValueType.Float:
                        v.Val = Convert.ToSingle(v);
                        break;
                    case ValueType.Bool:
                        v.Val = Convert.ToBoolean(v.value);
                        break;
                }
                _dict[v.index] = v.Val;
            }
        }
        
    }
    
    public class TableAsset : ScriptableObject
    {
        [MenuItem("Assets/Create/Custom/TableAsset")]
        public static void CreateAsset()
        {
            AssetUtil.CreateAsset<TableAsset>("New Table");
        }

        public string tableName;
        public List<TableColumn> columns = new List<TableColumn>();

    }
}