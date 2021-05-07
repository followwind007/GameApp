using System;
using System.Collections.Generic;

namespace GameApp.LuaResolver
{
    [Serializable]
    public class TypeInfo
    {
        public string name;
        public string typeName;
        public List<string> literalDescs = new List<string>();
        public string desc;
    }
}