using GameApp.DataBinder;
using LuaInterface;
using UnityEngine;

namespace GameApp.DebugConsole
{
    public class CommandUtil
    {
        private static LuaState State => BindableTargetImpl.State;
        
        public static void DoString(string command)
        {
            if (!Application.isPlaying)
            {
                return;
            }
            if (string.IsNullOrEmpty(command)) return;
            
            State?.DoString(command);
        }

        public static void ReloadLua(string requirePath, string content)
        {
            var tbl = State.DoString<LuaTable>(content);
            State.Call("Adapter.ReloadContent", requirePath, tbl, true);
        }
        
    }
}