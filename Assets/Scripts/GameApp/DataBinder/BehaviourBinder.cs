using System.Collections.Generic;
using GameApp.Pool;
using UnityEngine;
using UnityEngine.UI;

namespace GameApp.DataBinder
{
    public class BehaviourBinder : MonoBehaviour, IBindableTarget, IPoolItem, ISerializationCallbackReceiver
    {
        [PathRef(typeof(Object))] public string luaPath;
        public List<LuaPath> interfaceLuaPaths = new List<LuaPath>();
        public BindableValues vals = new BindableValues();
        
        public bool InitDone { get; set; }
        public HashSet<string> Methods { get; set; }
        public BindableValues Vals => vals;
        public List<LuaPath> Interfaces => interfaceLuaPaths;
        public string LuaPath => luaPath;
        
        public object this[string key]
        {
            get
            {
                var obj = vals[key];
                return obj;
            }
        }

        public void OnBeforeSerialize() { }

        public void OnAfterDeserialize()
        {
            Vals.OnAfterDeserialize();
        }

        private void Awake()
        {
            this.Init();
            this.CallLua("Awake");
        }

        private void Start()
        {
            this.CallLua("Start");
        }

        private void OnEnable()
        {
            this.CallLua("OnEnable");
        }

        private void OnDisable()
        {
            this.CallLua("OnDisable");
        }

        private void Update()
        {
            this.CallLuaCheck("Update");
        }

        private void OnDestroy()
        {
            this.CallLua("OnDestroy");
            UIEventCleaner.ClearBind(Vals);
        }
        
        public void OnRecycle()
        {
            this.CallLua("OnRecycle");
        }
        
        private void OnApplicationPause(bool pauseStatus)
        {
            this.CallLuaCheck("OnApplicationPause", pauseStatus);
        }

        private void OnApplicationFocus(bool focusStatus)
        {
            this.CallLuaCheck("OnApplicationFocus", focusStatus);
        }
        
    }
}
