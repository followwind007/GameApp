using UnityEngine;
using UnityEngine.UI;
using UnityEngine.EventSystems;

namespace Pangu.GUIs
{
    [RequireComponent(typeof(ScrollRect))]
    public class ScrollRectNest : MonoBehaviour, IDragHandler, IBeginDragHandler, IEndDragHandler
    {
        public enum ScrollDirection
        {
            Horizontal = 0,
            Vertical = 1,
        }

        public ScrollRect parentScroll;

        public delegate void OnDragDelegate(PointerEventData data);
        public OnDragDelegate onDrag;

        public delegate void OnBeginDragDelegate(PointerEventData data);
        public OnBeginDragDelegate onBeginDrag;

        public delegate void OnEndDragDelegate(PointerEventData data);
        public OnEndDragDelegate onEndDrag;

        public ScrollDirection scrollDirection = ScrollDirection.Vertical;

        private bool isSelf = false;

        private ScrollRect _childScroll;

        private void Awake()
        {
            _childScroll = GetComponent<ScrollRect>();
            switch (scrollDirection)
            {
                case ScrollDirection.Horizontal:
                    _childScroll.vertical = false;
                    _childScroll.horizontal = true;
                    break;
                case ScrollDirection.Vertical:
                    _childScroll.vertical = true;
                    _childScroll.horizontal = false;
                    break;
                default:
                    break;
            }
        }

        public void OnBeginDrag(PointerEventData eventData)
        {
            Vector2 touchDelta = eventData.delta;
            if (scrollDirection == ScrollDirection.Vertical)
            {
                if (Mathf.Abs(touchDelta.x) < Mathf.Abs(touchDelta.y))
                {
                    isSelf = true;
                    _childScroll.OnBeginDrag(eventData);
                }
                else
                {
                    isSelf = false;
                    parentScroll.OnBeginDrag(eventData);
                }
            }
            else
            {
                if (Mathf.Abs(touchDelta.x) > Mathf.Abs(touchDelta.y))
                {
                    isSelf = true;
                    _childScroll.OnBeginDrag(eventData);
                }
                else
                {
                    isSelf = false;
                    parentScroll.OnBeginDrag(eventData);
                }
            }
            if (onBeginDrag != null)
            {
                onBeginDrag.Invoke(eventData);
            }
            
        }

        public void OnDrag(PointerEventData eventData)
        {
            if (isSelf)
                _childScroll.OnDrag(eventData);
            else
                parentScroll.OnDrag(eventData);
            if (onDrag != null)
            {
                onDrag.Invoke(eventData);
            }
        }

        public void OnEndDrag(PointerEventData eventData)
        {
            if (isSelf)
                _childScroll.OnEndDrag(eventData);
            else
                parentScroll.OnEndDrag(eventData);
            if (onEndDrag != null)
            {
                onEndDrag.Invoke(eventData);
            }
        }

    }

}