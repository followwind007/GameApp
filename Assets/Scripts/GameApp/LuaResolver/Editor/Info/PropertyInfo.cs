using System;
using System.Collections.Generic;

namespace GameApp.LuaResolver
{
    [Serializable]
    public class PropertyInfo
    {
        public string name;
        public string typeName;
        public int index;
        public List<object> literals = new List<object>();
        public List<string> literalDescs = new List<string>();
        public string desc;
        public bool isPublic;
        public string lineStr;
    }
}