using System;
using UnityEngine;
using UnityEngine.EventSystems;
using GameApp.Util;

namespace GameApp.UI
{
    public enum FlipMode
    {
        RightToLeft,
        LeftToRight
    }

    public class Book : MonoBehaviour, IBeginDragHandler, IEndDragHandler, IDragHandler
    {
        public Action<int, RectTransform> onRefreshPage;
        public Action onFlip;

        public bool interactable = true;
        //public bool enableShadowEffect = true;
        public float activeDistance = 200;
        //index of the right page
        public int currentPage;
        public int spineWidth = 1;

        public float tweenDuration = 0.3f;

        private RectTransform _bookPanel;
        private RectTransform _turnPageClip;
        private RectTransform _nextPageClip;
        //private Image shadowRTL;
        //private Image shadowLTR;
        private RectTransform _left;
        private RectTransform _leftNext;
        private RectTransform _right;
        private RectTransform _rightNext;

        public int pageCount { get; private set; }

        public Vector2 edgeBottomLeft { get; private set; }
        public Vector2 edgeBottomRight { get; private set; }
        public bool isDragging { get; private set; }
        public bool isTweening { get; private set; }
        public bool flipAvailable { get { return !isDragging && !isTweening; } }

        private Canvas _canvas;
        private float _radiusShort, _radiusLong;

        private Vector2 _spineTop;
        private Vector2 _spineBottom;
        //corner of the page
        private Vector2 _cornor;
        private Vector2 _followPoint;
        private Vector2 _activePoint;

        private FlipMode _mode;

        private Vector3 _leftNextPosition;
        private Vector3 _rightNextPosition;

        private void Awake()
        {
            _bookPanel = gameObject.GetComponent<RectTransform>();
            _turnPageClip = gameObject.GetComponent<RectTransform>("TurnPageClip");
            _nextPageClip = gameObject.GetComponent<RectTransform>("NextPageClip");
            _left = gameObject.GetComponent<RectTransform>("Left");
            _right = gameObject.GetComponent<RectTransform>("Right");
            _leftNext = gameObject.GetComponent<RectTransform>("LeftNext");
            _rightNext = gameObject.GetComponent<RectTransform>("RightNext");

            _canvas = transform.root.GetComponent<Canvas>();
            _left.gameObject.SetActive(true);
            _left.gameObject.SetVisible(false);
            _right.gameObject.SetActive(true);
            _right.gameObject.SetVisible(false);
            /*
            1  .  2  .  3       1:                  2:spine top         3:
            .     .     .
            4     5     6       4:                  5:book position     6:
            .     .     .
            7  .  8  .  9       7:edge bottom left  8:spine bottom      9:edge bottom right
            */
            var rect = _bookPanel.rect;
            var pageWidth = (rect.width - spineWidth) / 2;
            var pageHeight = rect.height;

            _spineTop = new Vector2(0, pageHeight / 2);
            _spineBottom = new Vector2(0, -pageHeight / 2);
            edgeBottomLeft = new Vector2(-rect.width / 2, -pageHeight / 2);
            edgeBottomRight = new Vector2(rect.width / 2, -pageHeight / 2);
            //7--8
            _radiusShort = Vector2.Distance(_spineBottom, edgeBottomLeft);
            //7--2
            _radiusLong = Vector2.Distance(_spineTop, edgeBottomLeft);

            _turnPageClip.sizeDelta = new Vector2(pageWidth * 2, pageHeight + pageWidth * 2);
            //shadowRTL.rectTransform.sizeDelta = new Vector2(pageWidth, pageHeight + pageWidth * 0.6f);
            //shadowLTR.rectTransform.sizeDelta = new Vector2(pageWidth, pageHeight + pageWidth * 0.6f);
            _nextPageClip.sizeDelta = new Vector2(pageWidth, pageHeight + pageWidth * 0.6f);

            _leftNextPosition = _leftNext.transform.position;
            _rightNextPosition = _rightNext.transform.position;
        }

        public void Init(int count, int startPageIndex)
        {
            pageCount = count;
            SetPage(startPageIndex);
        }

        public void SetPage(int pageIndex)
        {
            currentPage = pageIndex;
            RefreshPage(currentPage - 1, _leftNext);
            RefreshPage(currentPage, _rightNext);
        }

        public void OnBeginDrag(PointerEventData eventData)
        {
            if (isTweening) return;

            var pos = ScreenToTransformPoint(eventData.position);
            if (!IsPointInBookPanel(pos) || !interactable) return;

            var distToBottomLeft = Vector2.Distance(pos, edgeBottomLeft);
            var distToBottomRight = Vector2.Distance(pos, edgeBottomRight);

            if (distToBottomLeft < distToBottomRight)
            {
                if (distToBottomLeft < activeDistance)
                {
                    DragLeftPageToPoint(pos);
                }
            }
            else
            {
                if (distToBottomRight < activeDistance)
                {
                    DragRightPageToPoint(pos);
                }
            }
        }

        public void OnDrag(PointerEventData eventData)
        {
            _activePoint = eventData.position;
        }

        public void OnEndDrag(PointerEventData eventData)
        {
            if (interactable)
                ReleasePage();
        }

        public void DragRightPageToPoint(Vector2 point)
        {
            if (currentPage >= pageCount) return;
            isDragging = true;

            _nextPageClip.pivot = new Vector2(0, 0.12f);
            _turnPageClip.pivot = new Vector2(1, 0.35f);

            _leftNext.transform.SetSiblingIndex(0);

            _left.gameObject.SetVisible(true);
            _left.pivot = Vector2.zero;
            var transform1 = _left.transform;
            var position = _rightNext.transform.position;
            transform1.position = position;
            transform1.eulerAngles = Vector2.zero;

            _right.gameObject.SetVisible(true);
            var transform2 = _right.transform;
            transform2.position = position;
            transform2.eulerAngles = Vector2.zero;
            _right.transform.SetParent(_turnPageClip.transform, true);

            RefreshPage(currentPage, _left);
            RefreshPage(currentPage + 1, _right);
            RefreshPage(currentPage + 2, _rightNext);

            //if (enableShadowEffect) shadowRTL.gameObject.SetVisible(true);
            UpdateBookRtlToPoint(point);
        }

        public void UpdateBookRtlToPoint(Vector2 followLocation)
        {
            _mode = FlipMode.RightToLeft;
            _followPoint = followLocation;

            //shadowRTL.transform.SetParent(turnPageClip.transform, true);
            //shadowRTL.transform.localPosition = Vector2.zero;
            //shadowRTL.transform.localEulerAngles = Vector2.zero;
            _left.transform.SetParent(_bookPanel.transform, true);
            _rightNext.transform.SetParent(_bookPanel.transform, true);

            _cornor = CalculateCornorPosition();
            var t0T1Angle = Calc_T0_T1_Angle(_cornor, edgeBottomRight, out var t1);
            if (t0T1Angle >= -90) t0T1Angle -= 180;
            //Debug.Log(string.Format("mouse:{0},cornor:{1},angle:{2}",followLocation,_cornor,T0_T1_Angle));
            _turnPageClip.pivot = new Vector2(1, 0.35f);
            _turnPageClip.transform.eulerAngles = new Vector3(0, 0, t0T1Angle + 90);
            _turnPageClip.transform.position = _bookPanel.TransformPoint(t1);

            _right.transform.position = _bookPanel.TransformPoint(_cornor);
            var cT1Dy = t1.y - _cornor.y;
            var cT1Dx = t1.x - _cornor.x;
            var cT1Angle = Mathf.Atan2(cT1Dy, cT1Dx) * Mathf.Rad2Deg;
            _right.transform.eulerAngles = new Vector3(0, 0, cT1Angle);

            _nextPageClip.transform.eulerAngles = new Vector3(0, 0, t0T1Angle + 90);
            _nextPageClip.transform.position = _bookPanel.TransformPoint(t1);
            _left.transform.SetParent(_turnPageClip.transform, true);
            _left.transform.SetSiblingIndex(0);
            _rightNext.transform.SetParent(_nextPageClip.transform, true);

            //shadowRTL.rectTransform.SetParent(right, true);
        }

        public void DragLeftPageToPoint(Vector2 point)
        {
            if (currentPage <= 0) return;
            isDragging = true;

            _nextPageClip.pivot = new Vector2(1, 0.12f);
            _turnPageClip.pivot = new Vector2(0, 0.35f);

            _left.gameObject.SetVisible(true);
            _left.pivot = new Vector2(1, 0);
            var transform1 = _left.transform;
            transform1.position = _leftNext.transform.position;
            transform1.eulerAngles = Vector2.zero;
            _left.transform.SetParent(_turnPageClip.transform, true);

            _right.gameObject.SetVisible(true);
            var transform2 = _right.transform;
            transform2.position = _leftNext.transform.position;
            transform2.eulerAngles = Vector2.zero;
            
            _rightNext.transform.SetSiblingIndex(0);

            RefreshPage(currentPage - 3, _leftNext);
            RefreshPage(currentPage - 2, _left);
            RefreshPage(currentPage - 1, _right);

            //if (enableShadowEffect) shadowLTR.gameObject.SetVisible(true);
            UpdateBookLtrToPoint(point);
        }

        public void UpdateBookLtrToPoint(Vector2 followLocation)
        {
            _mode = FlipMode.LeftToRight;
            _followPoint = followLocation;

            //shadowLTR.transform.SetParent(turnPageClip.transform, true);
            //shadowLTR.transform.localPosition = Vector2.zero;
            //shadowLTR.transform.localEulerAngles = Vector2.zero;

            _right.transform.SetParent(_bookPanel.transform, true);
            _leftNext.transform.SetParent(_bookPanel.transform, true);

            _cornor = CalculateCornorPosition();
            var t0T1Angle = Calc_T0_T1_Angle(_cornor, edgeBottomLeft, out var t1);
            if (t0T1Angle < 0) t0T1Angle += 180;

            _turnPageClip.transform.eulerAngles = new Vector3(0, 0, t0T1Angle - 90);
            _turnPageClip.transform.position = _bookPanel.TransformPoint(t1);

            _left.transform.position = _bookPanel.TransformPoint(_cornor);
            var cT1Dy = t1.y - _cornor.y;
            var cT1Dx = t1.x - _cornor.x;
            var cT1Angle = Mathf.Atan2(cT1Dy, cT1Dx) * Mathf.Rad2Deg;
            _left.transform.eulerAngles = new Vector3(0, 0, cT1Angle - 180);

            _nextPageClip.transform.eulerAngles = new Vector3(0, 0, t0T1Angle - 90);
            _nextPageClip.transform.position = _bookPanel.TransformPoint(t1);
            _right.transform.SetParent(_turnPageClip.transform, true);
            _right.transform.SetSiblingIndex(0);
            _leftNext.transform.SetParent(_nextPageClip.transform, true);

            //shadowLTR.rectTransform.SetParent(left, true);
        }

        public void ReleasePage()
        {
            if (isDragging)
            {
                isDragging = false;
                var distanceToLeft = Vector2.Distance(_cornor, edgeBottomLeft);
                var distanceToRight = Vector2.Distance(_cornor, edgeBottomRight);
                if (distanceToRight < distanceToLeft && _mode == FlipMode.RightToLeft)
                    TweenBack();
                else if (distanceToRight > distanceToLeft && _mode == FlipMode.LeftToRight)
                    TweenBack();
                else
                    TweenForward();
            }
        }

        private void RefreshPage(int index, RectTransform pageTrans)
        {
            if (index < 0 || index >= pageCount)
            {
                pageTrans.gameObject.SetVisible(false);
                return;
            }
            else
                pageTrans.gameObject.SetVisible(true);

            onRefreshPage?.Invoke(index, pageTrans);
        }

        private void Update()
        {
            if (isDragging && interactable)
            {
                UpdateBook();
            }
        }

        private void UpdateBook()
        {
            var lerpPoint = Vector2.Lerp(_followPoint, ScreenToTransformPoint(_activePoint), Time.deltaTime * 10);
            if (_mode == FlipMode.RightToLeft)
                UpdateBookRtlToPoint(lerpPoint);
            else
                UpdateBookLtrToPoint(lerpPoint);
        }

        private float Calc_T0_T1_Angle(Vector2 c, Vector2 bookCorner, out Vector2 t1)
        {
            var t0 = (c + bookCorner) / 2;
            var t0CornerDy = bookCorner.y - t0.y;
            var t0CornerDx = bookCorner.x - t0.x;
            var t0CornerAngle = Mathf.Atan2(t0CornerDy, t0CornerDx);

            var t1X = t0.x - t0CornerDy * Mathf.Tan(t0CornerAngle);
            t1X = NormalizeT1X(t1X, bookCorner, _spineBottom);
            t1 = new Vector2(t1X, _spineBottom.y);

            var t0T1Dy = t1.y - t0.y;
            var t0T1Dx = t1.x - t0.x;
            var t0T1Angle = Mathf.Atan2(t0T1Dy, t0T1Dx) * Mathf.Rad2Deg;
            return t0T1Angle;
        }

        private float NormalizeT1X(float t1, Vector2 corner, Vector2 sb)
        {
            if (t1 > sb.x && sb.x > corner.x)
                return sb.x;
            if (t1 < sb.x && sb.x < corner.x)
                return sb.x;
            return t1;
        }

        private Vector2 CalculateCornorPosition()
        {
            var fSbDy = _followPoint.y - _spineBottom.y;
            var fSbDx = _followPoint.x - _spineBottom.x;
            var fSbAngle = Mathf.Atan2(fSbDy, fSbDx);
            var r1 = new Vector2(_radiusShort * Mathf.Cos(fSbAngle), _radiusShort * Mathf.Sin(fSbAngle)) + _spineBottom;

            var fSbDistance = Vector2.Distance(_followPoint, _spineBottom);
            var c = fSbDistance < _radiusShort ? _followPoint : r1;
            var fStDy = c.y - _spineTop.y;
            var fStDx = c.x - _spineTop.x;
            var fStAngle = Mathf.Atan2(fStDy, fStDx);
            var r2 = new Vector2(_radiusLong * Mathf.Cos(fStAngle),
               _radiusLong * Mathf.Sin(fStAngle)) + _spineTop;
            var cStDistance = Vector2.Distance(c, _spineTop);
            if (cStDistance > _radiusLong)
                c = r2;
            return c;
        }

        private void TweenForward()
        {
            isTweening = true;
            var target = _mode == FlipMode.RightToLeft ? edgeBottomLeft : edgeBottomRight;
            ValueTweener.ValueTo(gameObject, _followPoint, target, GetTweenDuration(target), TweenUpdate,
                OnFinishTweenForward);
        }

        private void OnFinishTweenForward()
        {
            isTweening = false;
            if (_mode == FlipMode.RightToLeft)
            {
                //Debug.Log(currentPage + ":" + (currentPage + 2));
                currentPage += 2;
                RefreshPage(currentPage - 1, _leftNext);
            }
            else
            {
                //Debug.Log(currentPage + ":" + (currentPage - 2));
                currentPage -= 2;
                RefreshPage(currentPage, _rightNext);
            }
            _rightNext.transform.SetParent(_bookPanel.transform, true);
            _leftNext.transform.SetParent(_bookPanel.transform, true);

            _left.gameObject.SetVisible(false);
            _right.gameObject.SetVisible(false);
            //shadowRTL.gameObject.SetVisible(false);
            //shadowLTR.gameObject.SetVisible(false);

            ResetPages();
            onFlip?.Invoke();
        }

        private void TweenBack()
        {
            isTweening = true;
            var target = _mode == FlipMode.RightToLeft ? edgeBottomRight : edgeBottomLeft;
            ValueTweener.ValueTo(gameObject, _followPoint, target, GetTweenDuration(target), TweenUpdate, OnFinishTweenBack);
        }

        private void OnFinishTweenBack()
        {
            isTweening = false;
            if (_mode == FlipMode.RightToLeft)
            {
                RefreshPage(currentPage, _rightNext);
            }
            else
            {
                RefreshPage(currentPage - 1, _leftNext);
            }
            _rightNext.transform.SetParent(_bookPanel.transform);
            _leftNext.transform.SetParent(_bookPanel.transform);

            ResetPages();
            _left.gameObject.SetVisible(false);
            _right.gameObject.SetVisible(false);
        }

        private void TweenUpdate(Vector3 value)
        {
            if (_mode == FlipMode.RightToLeft)
                UpdateBookRtlToPoint(value);
            else
                UpdateBookLtrToPoint(value);
        }

        private void ResetPages()
        {
            var transform1 = _leftNext.transform;
            transform1.rotation = Quaternion.identity;
            transform1.position = _leftNextPosition;
            var transform2 = _rightNext.transform;
            transform2.rotation = Quaternion.identity;
            transform2.position = _rightNextPosition;
        }

        private Vector2 ScreenToTransformPoint(Vector2 global)
        {
            Vector2 localPos;
            if (_canvas.renderMode == RenderMode.ScreenSpaceOverlay)
            {
                localPos = _bookPanel.InverseTransformPoint(global);
            }
            else
            {
                RectTransformUtility.ScreenPointToLocalPointInRectangle(_bookPanel, global, _canvas.worldCamera, out localPos);
            }
            return localPos;
        }

        private bool IsPointInBookPanel(Vector2 point)
        {
            return point.x > edgeBottomLeft.x && point.x < edgeBottomRight.x &&
                   point.y > edgeBottomLeft.y && point.y < -edgeBottomLeft.y;
        }

        private float GetTweenDuration(Vector2 dest)
        {
            var dist = Vector2.Distance(dest, _followPoint);
            return (dist / edgeBottomRight.x) * tweenDuration;
        }

    }

}