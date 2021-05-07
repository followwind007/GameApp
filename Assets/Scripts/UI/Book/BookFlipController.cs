using System;
using UnityEngine;

namespace GameApp.UI
{
    public class BookFlipController : MonoBehaviour
    {
        private Book _book;

        public float flipDuration = 1f;
        public float maxFlipHeight = 100f;

        private float _pageWidth = 300f;
        private float _pageHeight = 450f;
        private float _k = 100 / 300 * 300;
        private float _h = 100f - 450f / 2;

        public void Init(Book book)
        {
            if (book == null) return;
            
            _book = book;
            _pageWidth = _book.edgeBottomRight.x;
            _pageHeight = -_book.edgeBottomRight.y * 2;
            _k = maxFlipHeight / (_pageWidth * _pageWidth);
            _h = maxFlipHeight - (_pageHeight / 2);
        }

        public void FlipToLeft()
        {
            if (_book.currentPage + 1 > _book.pageCount || !_book.flipAvailable) return;
            
            GenerateActivePoint(_pageWidth - 1, -_pageWidth, _book.UpdateBookRtlToPoint);
        }

        public void FlipToRight()
        {
            if (_book.currentPage - 1 < 0 || !_book.flipAvailable) return;
            GenerateActivePoint(-_pageWidth + 1, _pageWidth, _book.UpdateBookLtrToPoint);
        }

        private bool _isWorking;

        private float _startTime;
        private float _xStart;
        private float _xEnd;
        private Action<Vector2> _tweenCallback;
        private void Update()
        {
            if (!_isWorking) return;
            
            var x = (Time.time - _startTime) / flipDuration * (_xEnd - _xStart) + _xStart;
            var y = GetY(x);
            _tweenCallback?.Invoke(new Vector2(x, y));
            if (_xStart > _xEnd && x <= _xEnd || _xStart <= _xEnd && x >= _xEnd)
            {
                _book.ReleasePage();
                _book.interactable = true;
                _isWorking = false;
            }
        }

        private void GenerateActivePoint(float xStart, float xEnd, Action<Vector2> callback)
        {
            _xStart = xStart;
            _xEnd = xEnd;
            _tweenCallback = callback;

            _startTime = Time.time;
            _book.interactable = false;
            
            if (_xStart > _xEnd)
                _book.DragRightPageToPoint(new Vector2(_xStart, GetY(_xStart)));
            else
                _book.DragLeftPageToPoint(new Vector2(_xStart, GetY(_xStart)));

            _isWorking = true;
        }

        private float GetY(float x)
        {
            return -_k * (x * x) + _h;
        }

    }
}
