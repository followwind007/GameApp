
namespace UnityEngine.UIElements
{
    public static class VisualElementEx
    {
        public static void AddStyleSheetPath(this VisualElement vs, string path)
        {
            vs.styleSheets.Add(Resources.Load<StyleSheet>(path));
        }
    }
}