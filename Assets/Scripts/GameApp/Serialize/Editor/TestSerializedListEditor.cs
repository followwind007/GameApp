using UnityEditor;
using UnityEditor.UIElements;
using UnityEngine.UIElements;

namespace GameApp.Serialize
{
    [CustomEditor(typeof(TestSerlizliedList))]
    public class TestSerializedListEditor : Editor
    {
        public override VisualElement CreateInspectorGUI()
        {
            var container = new VisualElement();
            //https://forum.unity.com/threads/property-drawers.595369/ #6
            container.Add(new PropertyField(serializedObject.FindProperty("serialized")));
            return container;
        }
    }
}