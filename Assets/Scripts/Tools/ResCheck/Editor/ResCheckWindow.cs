using UnityEditor;

namespace Tools.ResCheck
{
    public class ResCheckWindow : EditorWindow
    {
        public static ResCheckView resCheckView;
        
        [MenuItem("Tools/ResCheck/Main Window", false, 100)]
        public static void Open()
        {
            GetWindow<ResCheckWindow>("ResCheck");
        }

        private void OnEnable()
        {
            resCheckView = new ResCheckView();
            rootVisualElement.Add(resCheckView);
        }

        private void OnDisable()
        {
            resCheckView = null;
        }
    }
}