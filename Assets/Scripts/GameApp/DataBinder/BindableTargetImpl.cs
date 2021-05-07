using System.Collections.Generic;
using LuaInterface;
using UnityEngine;

namespace GameApp.DataBinder
{
    [System.Serializable]
    public class LuaPath
    {
        [PathRef(typeof(Object))]
        public string path;
    }
    
    public static class BindableTargetImpl
    {
        private const string LuaPrefix = "Assets/Lua/";
        
        private static LuaState _state;
        
        [NoToLua]
        public static LuaState State
        {
            get
            {
                if (_state == null)
                {
#if !TEMPLATE_MODE
                    _state = LuaManager.Instance ? LuaManager.Instance.State : null;
#else
                    _state = Framework.Manager.LuaManager.Instance.State;
#endif
                }
                return _state;
            }            
        }


        private static LuaFunction _callFunc;
        private static LuaFunction CallFunc
        {
            get
            {
                if (_callFunc == null) _callFunc = State?.GetFunction("Adapter.Call");
                return _callFunc;
            }
        }

        private static LuaFunction _initFunc;
        private static LuaFunction InitFunc
        {
            get
            {
                if (_initFunc == null) _initFunc = State?.GetFunction("Adapter.Init");
                return _initFunc;
            }
        }

        private static LuaFunction _getLuaTableFunc;
        private static LuaFunction GetLuaTableFunc
        {
            get
            {
                if (_getLuaTableFunc == null) _getLuaTableFunc = State?.GetFunction("Adapter.GetLuaTable");
                return _getLuaTableFunc;
            }
        }

        [NoToLua]
        public static LuaTable GetLuaTable(this IBindableTarget target)
        {
            return GetLuaTableFunc?.Invoke<Object, LuaTable>((Object)target);
        }
        
        [NoToLua]
        public static string GetLuaPath(string fullPath)
        {
            return fullPath.Replace(LuaPrefix, "").Replace(".lua", "");
        }
        
        public static void Init(this IBindableTarget target)
        {
            if (target.InitDone || State == null || string.IsNullOrEmpty(target.LuaPath)) return;
            
            InitFunc?.Call(target, GetLuaPath(target.LuaPath), true);
            target.InitDone = true;
        }

        public static void RegisterMethod(this IBindableTarget target, string method)
        {
            if (target.Methods == null)
            {
                target.Methods = new HashSet<string>();
            }

            target.Methods.Add(method);
        }

        [NoToLua]
        public static bool IsMethodValid(this IBindableTarget target, string method)
        {
            return target.Methods != null && target.Methods.Contains(method);
        }

        [NoToLua]
        public static void CallLua(this IBindableTarget target, string method)
        {
            if (!target.InitDone) return;
            CallFunc?.Call(target, method, true);
        }

        [NoToLua]
        public static void CallLua<T>(this IBindableTarget target, string method, T arg0)
        {
            if (!target.InitDone) return;
            CallFunc?.Call(target, method, arg0, true);
        }
        
        [NoToLua]
        public static void CallLua<T, T1>(this IBindableTarget target, string method, T arg0, T1 arg1)
        {
            if (!target.InitDone) return;
            CallFunc?.Call(target, method, arg0, arg1, true);
        }
        
        [NoToLua]
        public static void CallLua<T, T1, T2>(this IBindableTarget target, string method, T arg0, T1 arg1, T2 arg2)
        {
            if (!target.InitDone) return;
            CallFunc?.Call(target, method, arg0, arg1, arg2, true);
        }
        
        [NoToLua]
        public static void CallLuaCheck(this IBindableTarget target, string method)
        {
            if (!target.IsMethodValid(method)) return;
            target.CallLua(method);
        }
        
        [NoToLua]
        public static void CallLuaCheck<T>(this IBindableTarget target, string method, T arg0)
        {
            if (!target.IsMethodValid(method)) return;
            target.CallLua(method, arg0);
        }

    }
}