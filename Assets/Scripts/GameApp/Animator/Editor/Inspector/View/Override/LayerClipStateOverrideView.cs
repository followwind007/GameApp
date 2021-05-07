using UnityEditor;
using UnityEditor.UIElements;
using UnityEngine;

namespace GameApp.AnimatorBehaviour
{
    [CustomStateOverrideProvider(typeof(LayerClipStateOverride))]
    public class LayerClipStateOverrideView : AnimatorStateOverrideView
    {
        public LayerClipStateOverrideView(LayerClipStateOverride state) : base(state)
        {
            var serilized = new SerializedObject(state);
            
            detail.Add(new PropertyField(serilized.FindProperty("clip")));
            detail.Add(new PropertyField(serilized.FindProperty("weight")));
            detail.Add(new PropertyField(serilized.FindProperty("mask")));

            var weightField = new FloatField("Weight") { value = state.weight };
            weightField.BindProperty(serilized.FindProperty("weight"));
            detail.Add(weightField);
            
            var maskField = new ObjectField("Mask") { objectType = typeof(AvatarMask), value = state.mask };
            maskField.BindProperty(serilized.FindProperty("mask"));
            detail.Add(maskField);
            
            var clipField = new ObjectField("Clip") { objectType = typeof(AnimationClip), value = state.clip };
            clipField.BindProperty(serilized.FindProperty("clip"));
            detail.Add(clipField);
        }
    }
}