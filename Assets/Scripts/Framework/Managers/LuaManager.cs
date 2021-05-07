using UnityEngine;
using LuaInterface;

namespace Framework.Manager
{
    public class LuaManager
    {
        private static LuaManager _instance;
        public static LuaManager Instance
        {
            get
            {
                if (_instance == null)
                {
                    _instance = new LuaManager();
                    _instance.Init();
                }
                return _instance;
            }
        }
        private LuaManager() { }

        public LuaState State { get; private set; }
        public LuaState EditorState { get; private set; }

        private void Init()
        {
            if (Application.isPlaying)
            {
                InitRuntime();
            }
            else
            {
                InitEditor();
            }
        }

        private void InitRuntime()
        {
            State = new LuaState();
            State.AddSearchPath(Application.dataPath + "/Lua/App");
            State.Start();
            LuaBinder.Bind(State);
            AddLopper();
            LoadLibs();
            State.DoFile("Main.lua");
        }

        private void LoadLibs()
        {
            State.BeginPreLoad();
            State.RegFunction("pb", LuaDLL.luaopen_pb);
            State.RegFunction("pb.io", LuaDLL.luaopen_pb_io);
            State.RegFunction("pb.conv", LuaDLL.luaopen_pb_conv);
            State.RegFunction("pb.buffer", LuaDLL.luaopen_pb_buffer);
            State.RegFunction("pb.slice", LuaDLL.luaopen_pb_slice);
            State.EndPreLoad();
        }

        private void AddLopper()
        {
            var go = GameObject.Find("_LuaLooper");
            if (go == null)
            {
                go = new GameObject("_LuaLooper");
                Object.DontDestroyOnLoad(go);
                var looper = go.AddComponent<LuaLooper>();
                looper.luaState = State;
            }
        }

        private void InitEditor()
        {
            EditorState = new LuaState();
        }

    }

}