using UnityEditor;

namespace UnityEngine.UI
{
    [CustomEditor(typeof(HotArea))]
    public class HotAreaEditor : Editor
    {
        [MenuItem("GameObject/UI/HotArea", false)]
        private static void AddGameObject()
        {
            GameObject child = new GameObject("HotArea");
            RectTransform rectTransform = child.AddComponent<RectTransform>();
            Vector2 size = Vector2.one;
            GameObject select = Selection.activeGameObject;
            if (select)
            {
                child.transform.SetParent(select.transform);
                RectTransform r = select.GetComponent<RectTransform>();
                if (r)
                {
                    size = r.sizeDelta;
                }
            }
            rectTransform.sizeDelta = size;

            rectTransform.anchorMax = Vector2.one;
            rectTransform.anchorMin = Vector2.zero;
            rectTransform.localScale = Vector3.one;
            rectTransform.offsetMax = Vector2.zero;
            rectTransform.offsetMin = Vector2.zero;

            child.AddComponent<HotArea>();
        }

        public override void OnInspectorGUI()
        {
            
        }

    }
}