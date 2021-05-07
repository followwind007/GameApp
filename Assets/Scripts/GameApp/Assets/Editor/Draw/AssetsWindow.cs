using UnityEditor;
using UnityEngine;

namespace GameApp.Assets
{
    public class AssetsWindow : EditorWindow
    {
        [MenuItem("Build/Window", false, 101)]
        private static void ShowWindow()
        {
            var window = GetWindow<AssetsWindow>();
            window.titleContent = new GUIContent("Build Window");
            window.Show();
        }

        private void CreateGUI()
        {
            rootVisualElement.Add(new AssetsView());
        }

        private void OnDisable()
        {
            AssetDatabase.SaveAssets();
        }
    }
}