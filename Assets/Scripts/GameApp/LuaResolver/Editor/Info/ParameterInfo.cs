using System;
using System.Collections.Generic;

namespace GameApp.LuaResolver
{
    [Serializable]
    public class ParameterInfo
    {
        public string name;
        public string typeName;
        public string desc;
        public List<string> literalDescs = new List<string>();
    }
}