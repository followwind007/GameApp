using UnityEditor;
using UnityEditor.UIElements;
using UnityEngine.UIElements;

namespace GameApp.AnimatorBehaviour
{
    [CustomEditor(typeof(ClipState))]
    public class ClipStateEditor : AnimatorStateEditor
    {
        public class ClipStateView : StateView
        {
            public ClipStateView(SerializedObject serializedObject) : base(serializedObject)
            {
                Add(new PropertyField(serializedObject.FindProperty("clip")));
            }
        }
        
        public override VisualElement CreateInspectorGUI()
        {
            return new ClipStateView(serializedObject);
        }
    }
}