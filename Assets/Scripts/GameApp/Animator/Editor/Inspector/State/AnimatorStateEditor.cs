using UnityEditor;
using UnityEditor.UIElements;
using UnityEngine.UIElements;

namespace GameApp.AnimatorBehaviour
{
    [CustomEditor(typeof(AnimatorState), true)]
    public class AnimatorStateEditor : Editor
    {
        public class StateView : VisualElement
        {
            public StateView(SerializedObject serializedObject)
            {
                Add(new PropertyField(serializedObject.FindProperty("stateName")));
            }
        }
        
        public override VisualElement CreateInspectorGUI()
        {
            return new StateView(serializedObject);
        }
    }
}