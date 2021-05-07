using UnityEditor;
using UnityEditor.UIElements;
using UnityEngine.UIElements;

namespace GameApp.Assets
{
    public class BundleSettingsView : VisualElement
    {
        public BundleSettingsView(SerializedObject serializedObject)
        {
            var iter = serializedObject.GetIterator();
            iter.Next(true);
            while (iter.NextVisible(false))
            {
                Add(new PropertyField(iter));
            }
        }
        
    }
}