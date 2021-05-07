using System.Collections.Generic;
using UnityEngine;

namespace GameApp.LuaResolver
{
    [CreateAssetMenu(menuName = "Custom/LuaResolverSettings", fileName = "LuaResolverSettings", order = 501)]
    public class LuaResolverSettings : ScriptableObject
    {
        public static LuaResolverSettings Instance 
        {
            get
            {
                var data = Resources.Load<LuaResolverSettings>("LuaResolverSettings");
                if (data == null)
                {
                    Debug.LogError("can not find LuaResolverSettings in Resources");
                }
                return data;
            }
        }

        public List<string> luaPaths = new List<string>();
        
        public List<LuaClass> luaClasses = new List<LuaClass>();
        
    }
}