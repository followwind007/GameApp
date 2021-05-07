using System;
using System.Collections.Generic;
using UnityEngine;

namespace GameApp.AnimatorBehaviour
{
    [Serializable]
    public abstract class AnimatorState : ScriptableObject
    {
        public string stateName;
        public string StateName => stateName;

        [HideInInspector]
        public List<AnimatorTransfer> transfers = new List<AnimatorTransfer>();
        
        [HideInInspector]
        public Vector2 position;

        public abstract AnimatorStateBehaviour CreateBehaviour(AnimatorRunner runner);
    }
}