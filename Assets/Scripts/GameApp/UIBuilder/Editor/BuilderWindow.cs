using UnityEditor;
using UnityEditor.UIElements;
using UnityEngine;
using UnityEngine.UIElements;

namespace GameApp.UIBuilder
{
    public class BuilderWindow : EditorWindow
    {
        private GameObject _selected;
        
        private VisualElement root
        {
            get { return rootVisualElement; }
        }
        
        [MenuItem("Tools/UIBuilder", false, 301)]
        public static BuilderWindow Open()
        {
            var window = GetWindow<BuilderWindow>();
            window.titleContent = new GUIContent("UIBuilder");
            return window;
        }
        
        private void OnEnable()
        {
            var builder = new BuilderView();
            root.Add(builder);
        }

        private void OnDisable()
        {
            Prefs.SavePrefs();
        }
    }

}
