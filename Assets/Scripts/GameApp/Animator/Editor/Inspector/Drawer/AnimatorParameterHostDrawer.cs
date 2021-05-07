using UnityEditor;
using UnityEditor.UIElements;
using UnityEngine.UIElements;

namespace GameApp.AnimatorBehaviour
{
    [CustomPropertyDrawer(typeof(AnimatorParameterHost))]
    public class AnimatorParameterHostDrawer : PropertyDrawer
    {
        public class ParameterHostView : VisualElement
        {
            public ParameterHostView(SerializedProperty property)
            {
                style.flexDirection = FlexDirection.Row;

                var nameField = new PropertyField(property.FindPropertyRelative("name"));
                {
                    nameField.style.minWidth = 220;
                }
                Add(nameField);
                Add(new PropertyField(property.FindPropertyRelative("obj")));
            }
        }
        
        public override VisualElement CreatePropertyGUI(SerializedProperty property)
        {
            return new ParameterHostView(property);
        }
    }
}