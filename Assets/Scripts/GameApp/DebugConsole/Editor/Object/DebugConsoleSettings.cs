using System.Collections.Generic;
using Tools;
using UnityEditor;
using UnityEngine;

namespace GameApp.DebugConsole
{
    public class DebugConsoleSettings : ScriptableObject
    {
        private static DebugConsoleSettings _instance;

        public static DebugConsoleSettings Instance
        {
            get
            {
                if (_instance == null)
                {
                    _instance = Resources.Load<DebugConsoleSettings>("DebugConsoleSettings");
                }

                return _instance;
            }
        }
        
        [MenuItem("Assets/Create/Custom/DebugConsoleSettings")]
        private static void CreateSettings()
        {
            AssetUtil.CreateAsset<DebugConsoleSettings>("DebugConsoleSettings");
        }
        
        public List<string> observeLuaPaths = new List<string>();
        
        public List<LuaCommand> luaCommands = new List<LuaCommand>();
        
    }

}
