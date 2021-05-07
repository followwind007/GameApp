
namespace UnityEngine.UI
{
    [RequireComponent(typeof(ScrollRect))]
    public class ScrollRectHelper : MonoBehaviour
    {
        public GameObject indicatorBottom;

        private ScrollRect _scroll;
        private RectTransform _rectTrans;
        private RectTransform _content;

        public void MoveTo(RectTransform target, Vector2 scrollRectPos)
        {
            var pos = target.anchoredPosition;
            var pivot = target.pivot;
            var size = target.rect.size;
            pos = new Vector2(pos.x - pivot.x * size.x, -pos.y - (1 - pivot.y) * size.y);

            var contentSize = _content.rect.size;

            var scrollRectTrans = _scroll.GetComponent<RectTransform>();
            var scrollSize = scrollRectTrans.rect.size;

            if (_scroll.vertical)
            {
                var vertNormal = new Vector2(pos.y, pos.y + scrollSize.y) / (contentSize.y - scrollSize.y);
                vertNormal = new Vector2(Mathf.Clamp(vertNormal.x, 0f, 1f), Mathf.Clamp(vertNormal.y, 0f, 1f));
                _scroll.verticalNormalizedPosition = 1 - vertNormal.x - (vertNormal.y - vertNormal.x) * scrollRectPos.y;
            }

            if (_scroll.horizontal)
            {
                var horiNormal = new Vector2(pos.x, pos.x + scrollSize.x) / (contentSize.x - scrollSize.x);
                horiNormal = new Vector2(Mathf.Clamp(horiNormal.x, 0f, 1f), Mathf.Clamp(horiNormal.y, 0f, 1f));
                _scroll.horizontalNormalizedPosition = horiNormal.x + (horiNormal.y - horiNormal.x) * scrollRectPos.x;
            }
        }

        private void Awake()
        {
            _rectTrans = GetComponent<RectTransform>();
            _scroll = GetComponent<ScrollRect>();
            _content = _scroll.content;
            _scroll.onValueChanged.AddListener(OnValueChanged);
        }
        

        private void Start()
        {
            OnValueChanged(Vector2.zero);
        }

        private void OnDestroy()
        {
            _scroll.onValueChanged.RemoveListener(OnValueChanged);
        }

        private void OnValueChanged(Vector2 val)
        {
            if (_content)
            {
                var scrollHeight = _rectTrans.rect.height;
                if (indicatorBottom != null)
                {
                    indicatorBottom.SetActive(_content.anchoredPosition.y + scrollHeight - _content.rect.height < -1f);
                }
            }
        }
        
        
    }
}