using System.Collections.Generic;
using UnityEngine;

namespace GameApp.AnimatorBehaviour
{
    [CreateAssetMenu(fileName = "Animator Data", menuName = "Animator/Data", order = 201)]
    public class AnimatorData : ScriptableObject, ISerializationCallbackReceiver
    {
        public List<AnimatorState> states = new List<AnimatorState>();

        public AnimatorState enterState;

        public List<AnimatorParameterHost> parameters = new List<AnimatorParameterHost>();

        public void Init()
        {
            states.ForEach(s => { s.transfers.ForEach(t => t.Init()); });
        }

        public void OnBeforeSerialize() { }

        public void OnAfterDeserialize()
        {
            //Init();
        }
    }
    
}