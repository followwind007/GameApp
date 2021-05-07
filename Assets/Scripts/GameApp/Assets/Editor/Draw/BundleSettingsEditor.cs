using UnityEditor;
using UnityEngine.UIElements;

namespace GameApp.Assets
{
    //[CustomEditor(typeof(BundleSettings))]
    public class BundleSettingsEditor : Editor
    {
        public override VisualElement CreateInspectorGUI()
        {
            return new BundleSettingsView(serializedObject);
        }
    }
}