using System.Collections.Generic;
using UnityEngine;

namespace GameApp.DataBinder
{
    public class StateBinder : StateMachineBehaviour, IBindableTarget
    {        
        [PathRef(typeof(Object))]
        public string luaPath;

        public List<LuaPath> interfaceLuaPaths;

        public BindableValues values;

        public string LuaPath => luaPath;
        public List<LuaPath> Interfaces => interfaceLuaPaths;
        
        public HashSet<string> Methods { get; set; }

        public bool InitDone { get; set; }
        
        public BindableValues Vals 
        {
            get { return values; }
            set { values = value; } 
        }

        public override void OnStateMachineEnter(Animator animator, int stateMachinePathHash)
        {
            base.OnStateMachineEnter(animator, stateMachinePathHash);
            this.CallLua("OnStateMachineEnter", animator, stateMachinePathHash);
        }

        public override void OnStateMachineExit(Animator animator, int stateMachinePathHash)
        {
            base.OnStateMachineExit(animator, stateMachinePathHash);
            this.CallLua("OnStateMachineExit", animator, stateMachinePathHash);
        }

        public override void OnStateEnter(Animator animator, AnimatorStateInfo animatorStateInfo, int layerIndex)
        {
            base.OnStateEnter(animator, animatorStateInfo, layerIndex);
            this.CallLua("OnStateEnter", animator, animatorStateInfo, layerIndex);
        }

        public override void OnStateMove(Animator animator, AnimatorStateInfo stateInfo, int layerIndex)
        {
            base.OnStateMove(animator, stateInfo, layerIndex);
            this.CallLua("OnStateMove", animator, stateInfo, layerIndex);
        }

        public override void OnStateExit(Animator animator, AnimatorStateInfo animatorStateInfo, int layerIndex)
        {
            base.OnStateExit(animator, animatorStateInfo, layerIndex);
            this.CallLua("OnStateExit", animator, animatorStateInfo, layerIndex);
        }

        public override void OnStateIK(Animator animator, AnimatorStateInfo stateInfo, int layerIndex)
        {
            base.OnStateIK(animator, stateInfo, layerIndex);
            this.CallLua("OnStateIK", animator, stateInfo, layerIndex);
        }

        private void Awake()
        {
            this.Init();
            this.CallLua("Awake");
        }

        private void OnDestroy()
        {
            this.CallLua("OnDestroy");
        }

    }
}