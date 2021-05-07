using UnityEngine;

namespace GameApp.AnimatorBehaviour
{
    public abstract class AnimatorStateOverride : ScriptableObject
    {
        public string stateName;
        public bool enabled;
    }
}