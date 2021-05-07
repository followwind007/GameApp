using UnityEditor;
using UnityEngine.UI;

namespace UnityEngine.UI.Custom
{
    public static class CustomUIEditor
    {
        [MenuItem("GameObject/UI/Custom/ToggleGroup", false, 110)]
        private static void CreateToggleGroup()
        {
            if (Selection.activeGameObject == null) return;
            
            var rectTrans = Selection.activeGameObject.GetComponent<RectTransform>();
            if (rectTrans == null) return;
            
            var go = new GameObject("ToggleGroup");
            var toggleGroupRt = go.AddComponent<RectTransform>();
            SetParentCenter(toggleGroupRt, rectTrans, new Vector2(100f, 100f));
            
            var content = new GameObject("Content");
            var contentRt = content.AddComponent<RectTransform>();
            SetParentCenter(contentRt, toggleGroupRt, new Vector2(100f, 100f));
            
            Selection.activeGameObject = go;

            go.AddComponent<ToggleGroup>();
            var tgh = go.AddComponent<ToggleGroupHelper>();
            tgh.content = contentRt;
        }

        private static Canvas FindCanvas(GameObject target)
        {
            while (target != null)
            {
                var canvas = target.GetComponent<Canvas>();
                if (canvas != null) return canvas;
                var parent = target.transform.parent;
                target = parent != null ? parent.gameObject : null;
            }

            return null;
        }

        private static void SetParentCenter(RectTransform child, RectTransform parent, Vector2 size)
        {
            child.SetParent(parent);
            child.pivot = new Vector2(0.5f, 0.5f);
            child.anchorMin = new Vector2(0.5f, 0.5f);
            child.anchorMax = new Vector2(0.5f, 0.5f);
            child.sizeDelta = size;
            child.anchoredPosition = Vector2.zero;
        }
        
    }

}
