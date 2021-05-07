using UnityEditor;
using UnityEditor.UIElements;
using UnityEngine.UIElements;

namespace GameApp.AnimatorBehaviour
{
    [CustomEditor(typeof(LayerClipState))]
    public class LayerClipStateEditor : AnimatorStateEditor
    {
        public class LayerClipStateView : StateView
        {
            public LayerClipStateView(SerializedObject serializedObject) : base(serializedObject)
            {
                Add(new PropertyField(serializedObject.FindProperty("weight")));
                Add(new PropertyField(serializedObject.FindProperty("clip")));
                Add(new PropertyField(serializedObject.FindProperty("mask")));
            }
        }

        public override VisualElement CreateInspectorGUI()
        {
            return new LayerClipStateView(serializedObject);
        }
    }
}