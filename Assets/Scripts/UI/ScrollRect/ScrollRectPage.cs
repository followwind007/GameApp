using UnityEngine;
using UnityEngine.UI;
using UnityEngine.EventSystems;
using System;
using DG.Tweening;

namespace Pangu.GUIs
{
    [RequireComponent(typeof(ScrollRect))]
    public class ScrollRectPage : MonoBehaviour, IEndDragHandler, IBeginDragHandler
    {
        public bool isDebug;
        
        public bool vertical;
        public bool horizontal = true;

        public Vector2 pageSize = new Vector2(100f, 100f);

        public Ease ease = Ease.Flash;
        public float tweenDuration = 0.5f;

        [Range(0f, 1f)]
        public float sensitive = 0.5f;

        public bool nestScrollRect;

        public Action<int, int> onEndTween;
        public Action<int, int> onBeginTween;

        private ScrollRect _scrollRect;
        private RectTransform _contentRectTrans;

        private Tweener _tweener;

        private Vector2 _rectValue;
        private Vector2 _destValue;

        private int _indexDestVertical;
        private int _indexDestHorizontal;

        private Vector2 _dragBeginPos;
        private Vector2 _dragEndPos;
        private Vector2 _delta;

        private float _dura = 0.5f;

        public int BlockX { get; private set; }
        public int BlockY { get; private set; }

        public void OnBeginDrag(PointerEventData eventData)
        {
            _dragBeginPos = eventData.position;
        }

        public void OnEndDrag(PointerEventData eventData)
        {
            _dragEndPos = eventData.position;
            _delta = _dragEndPos - _dragBeginPos;

            if (_tweener != null && _tweener.IsActive()) return;
            
            if (vertical && !horizontal)
                _delta.x = 0f;
            else if (!vertical && horizontal)
                _delta.y = 0f;
            
            SetIndexDest();
            onBeginTween?.Invoke(_indexDestHorizontal, _indexDestVertical);

            _tweener = _scrollRect.DONormalizedPos(_destValue, _dura);
            _tweener.SetEase(ease);
            _tweener.onComplete = () =>
            {
                onEndTween?.Invoke(_indexDestHorizontal, _indexDestVertical);
            };
            //Debug.Log("normal dest: " + _destValue + " index: " + _indexDestHorizontal + "," + _indexDestVertical + " delta: " + _delta);
        }

        private void Awake()
        {
            _scrollRect = GetComponent<ScrollRect>();
            _scrollRect.onValueChanged.AddListener(OnValueChange);

            _contentRectTrans = _scrollRect.content;
            if (!_contentRectTrans)
            {
                Debug.LogError("scroll rect need content!");
            }

            if (nestScrollRect)
            {
                var nests = GetComponentsInChildren<ScrollRectNest>();
                foreach (var nest in nests)
                {
                    nest.onBeginDrag = OnBeginDrag;
                    nest.onEndDrag = OnEndDrag;
                }
            }
        }

        private void OnValueChange(Vector2 uv)
        {
            _rectValue = uv;
        }

        private void SetIndexDest()
        {
            var deltaX = _delta.x / pageSize.x;
            var absDeltaX = Math.Abs(deltaX) - Math.Floor(Math.Abs(deltaX));
            if (absDeltaX > sensitive)
                deltaX = deltaX < 0 ? (float)Math.Floor(deltaX) : (float)Math.Ceiling(deltaX);
            _indexDestHorizontal -= (int)deltaX;

            var deltaY = _delta.y / pageSize.y;
            var absDeltaY = Math.Abs(deltaY) - Math.Floor(Math.Abs(deltaY));
            if (absDeltaY > sensitive)
                deltaY = deltaY < 0 ? (float)Math.Floor(deltaY) : (float)Math.Ceiling(deltaY);
            _indexDestVertical -= (int)deltaY;

            var rect = _contentRectTrans.rect;
            BlockX = (int)(rect.size.x / pageSize.x);
            BlockY = (int)(rect.size.y / pageSize.y);

            _indexDestHorizontal = Mathf.Clamp(_indexDestHorizontal, 0, BlockX - 1);
            _indexDestVertical = Mathf.Clamp(_indexDestVertical, 0, BlockY - 1);

            var rect1 = ((RectTransform) transform).rect;
            var rect2 = _contentRectTrans.rect;
            
            var gapX = pageSize.x / (rect2.width - rect1.width + 0.001f);
            var gapY = pageSize.y / (rect2.height - rect1.height + 0.001f);

            _destValue = new Vector2(Mathf.Clamp(gapX * _indexDestHorizontal, 0f, 1f), Mathf.Clamp(gapY * _indexDestVertical, 0f, 1f));
            Debug.Log(_destValue);
            _dura = Math.Abs(_rectValue.magnitude - _destValue.magnitude) / new Vector2(gapX, gapY).magnitude * tweenDuration;
            _dura = Mathf.Clamp(_dura, 0.1f, tweenDuration);
        }
        
        #if UNITY_EDITOR
        private void OnGUI()
        {
            if (!isDebug)
            {
                return;
            }
            var normalizedPosition = _scrollRect.normalizedPosition;
            GUILayout.Label($"{normalizedPosition.x}, {normalizedPosition.y}");
        }
        #endif
    }

}