namespace UnityEngine.UI
{
    [RequireComponent(typeof(Canvas))]
    public class CanvasHelper : MonoBehaviour
    {
        public bool useSafeArea;
        
        private RectTransform _rectTrans;

        public void AddChild(GameObject child)
        {
            AddChild(child.GetComponent<RectTransform>());
        }

        public void AddChild(RectTransform childRectTrans)
        {
            childRectTrans.SetParent(_rectTrans);
            if (useSafeArea)
            {
                var safeArea = Screen.safeArea;
                childRectTrans.anchorMin = new Vector2(safeArea.min.x / Screen.width, safeArea.min.y / Screen.height);
                childRectTrans.anchorMax = new Vector2(safeArea.max.x / Screen.width, safeArea.max.y / Screen.height);
                childRectTrans.sizeDelta = Vector2.zero;
            }
            else
            {
                childRectTrans.anchorMin = Vector2.zero;
                childRectTrans.anchorMax = Vector2.one;
                childRectTrans.sizeDelta = Vector2.zero;
            }
        }
        
        private void Awake()
        {
            _rectTrans = GetComponent<RectTransform>();
        }
        
        
    }
}