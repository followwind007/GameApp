using GameApp.Util;
using UnityEditor;
using UnityEngine.UIElements;

namespace GameApp.Moving
{
    public class AnchorSetView : VisualElement
    {
        public AnchorSetView(SerializedProperty property)
        {
            var list = new ReorderableListView(property.FindPropertyRelative(AnchorSet.PropPoints));
            Add(list);
        }
    }

    [CustomPropertyDrawer(typeof(AnchorSet))]
    public class AnchorSetDrawer : PropertyDrawer
    {
        public override VisualElement CreatePropertyGUI(SerializedProperty property)
        {
            return new AnchorSetView(property);
        }
    }
}