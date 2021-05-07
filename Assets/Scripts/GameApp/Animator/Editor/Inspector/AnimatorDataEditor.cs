
using UnityEditor;
using UnityEditor.UIElements;
using UnityEngine;
using UnityEngine.UIElements;

namespace GameApp.AnimatorBehaviour
{
    [CustomEditor(typeof(AnimatorData))]
    public class AnimatorDataEditor : Editor
    {
        public class DataView : VisualElement
        {
            public readonly SerializedObject serializedObject;
            public readonly AnimatorData data;
            
            public DataView(SerializedObject serializedObject, Object target)
            {
                this.serializedObject = serializedObject;
                data = (AnimatorData) target;

                var parameter = new PropertyField(serializedObject.FindProperty("parameters"));
                parameter.SetEnabled(false);
                Add(parameter);
            }
        }
        
        public override VisualElement CreateInspectorGUI()
        {
            return new DataView(serializedObject, target);
        }
    }
}