using UnityEditor;

namespace Tools.ResCheck
{
    public class ResPrefabImportWindow : EditorWindow
    {
        public static ResPrefabImportView view;
        
        [MenuItem("Tools/ResCheck/Prefab Window", false, 100)]
        public static void Open()
        {
            GetWindow<ResPrefabImportWindow>("ResCheck");
        }

        private void OnEnable()
        {
            view = new ResPrefabImportView();
            rootVisualElement.Add(view);
        }

        private void OnDisable()
        {
            view = null;
        }

        
    }
}