using System.Collections.Generic;
using GameApp.DataBinder;
using UnityEngine;

namespace GameApp.LogicGraph
{
    public class LogicGraphInstance : MonoBehaviour, IBindableTarget
    {
        public LogicGraphObject graphObject;

        public BindableValues exposed = new BindableValues();

        public BindableValues Vals { get; set; }
        public string LuaPath => graphObject.exportPath;
        public List<LuaPath> Interfaces { get; } = new List<LuaPath>();

        public HashSet<string> Methods { get; set; }
        public bool InitDone { get; set; }

        public Dictionary<string, object> ValDict => Vals.valDict;

        
        private void Awake()
        {
            if (!graphObject)
            {
                Debug.LogWarning("null graphObject");
                return;
            }

            graphObject = Instantiate(graphObject);
            Vals = graphObject.binds;
            Bind();
            
            this.Init();
  
            this.CallLua("Awake");
        }

        private void Bind()
        {
            var eps = exposed.valDict;
            Vals.Init();
            foreach (var expose in eps)
            {
                var wrap = Vals.GetWrap(expose.Key);
                if (wrap != null)
                {
                    wrap.value = expose.Value;
                }
            }
        }

        private void Start()
        {
            this.CallLua("Start");
        }

        private void Update()
        {
            this.CallLuaCheck("Update");
        }

        private void OnEnable()
        {
            this.CallLua("OnEnable");
        }

        private void OnDisable()
        {
            this.CallLua("OnDisable");
        }

        private void OnDestroy()
        {
            this.CallLua("OnDestroy");
        }

    }
    
}