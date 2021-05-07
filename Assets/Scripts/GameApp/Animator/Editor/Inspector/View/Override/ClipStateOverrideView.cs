using UnityEditor;
using UnityEditor.UIElements;
using UnityEngine;
using UnityEngine.UIElements;

namespace GameApp.AnimatorBehaviour
{
    [CustomStateOverrideProvider(typeof(ClipStateOverride))]
    public class ClipStateOverrideView : AnimatorStateOverrideView
    {
        public ClipStateOverrideView(ClipStateOverride state) : base(state)
        {
            var clipField = new ObjectField
            {
                value = state.clip,
                objectType = typeof(AnimationClip)
            };
            
            clipField.style.width = 170;
            clipField.RegisterValueChangedCallback(e =>
            {
                state.clip = e.newValue as AnimationClip;
                Save(state);
            });
            
            head.Add(clipField);
        }

    }
}