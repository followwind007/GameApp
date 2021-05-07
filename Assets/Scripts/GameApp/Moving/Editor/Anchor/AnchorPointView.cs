using UnityEditor;
using UnityEditor.UIElements;
using UnityEngine.UIElements;

namespace GameApp.Moving
{
    public class AnchorPointView : VisualElement
    {
        public AnchorPointView(SerializedProperty property)
        {
            Add(new PropertyField(property.FindPropertyRelative(AnchorPoint.PropPosition)));
            Add(new PropertyField(property.FindPropertyRelative(AnchorPoint.PropRotation)));
        }
    }

    [CustomPropertyDrawer(typeof(AnchorPoint))]
    public class AnchorPointDrawer : PropertyDrawer
    {
        public override VisualElement CreatePropertyGUI(SerializedProperty property)
        {
            return new AnchorPointView(property);
        }
    }

}