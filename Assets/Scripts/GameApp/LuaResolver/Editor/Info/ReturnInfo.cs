using System;
using System.Collections.Generic;

namespace GameApp.LuaResolver
{
    [Serializable]
    public class ReturnInfo
    {
        public string desc;
        public List<string> typeNames = new List<string>();
    }
}