using UnityEngine;
using UnityEngine.UI;

namespace Pangu.GUIs
{
    public class MiniMap : MonoBehaviour
    {
        public enum UpdateFrequency
        {
            RealTime = 0,
            Normal = 1,
            Low = 2,
            Never = 3,
        }

        public float intervalRealTime = 0f;
        public float intervalNoraml = 0.1f;
        public float intervalLow = 1f;

        [SerializeField]
        private RawImage _mapTexture;

        [SerializeField]
        private Vector2 _mapSize = Vector2.one;
        private Vector2 _mapShowSize = Vector2.one;
        private float _mapScale = 0f;

        [SerializeField]
        private float _elementCullWidth = 20f;
        [SerializeField]
        private Vector3 _mapAnchorPosition = Vector3.zero;

        private Vector3 _currentPosition = Vector3.zero;

        [SerializeField]
        private Vector2 _uv = Vector2.one;

        [SerializeField]
        [Range(0f, 1f)]
        private float _scaleFactor = 1f;

        public float ScaleFactor
        {
            get
            {
                if (_scaleFactor <= 0)
                {
                    return 0.01f;
                }
                return _scaleFactor;
            }
            set
            {
                if (value != _scaleFactor)
                {
                    _scaleFactor = value;
                    SetUVSize();
                }
            }
        }

        public RawImage MapTexture
        {
            get
            {
                return _mapTexture;
            }
            set
            {
                if (value != null)
                {
                    _mapTexture = value;
                }
            }
        }

        public void InitMap(Vector2 mapSize, Vector3 mapAnchorPos, RawImage mapTexture, float scaleFactor)
        {
            _mapSize = mapSize;
            _mapAnchorPosition = mapAnchorPos;
            _mapTexture = mapTexture;
            ScaleFactor = scaleFactor;
        }

        public void UpdateMapUV(Vector3 worldPosition)
        {
            //消除偏移
            Vector3 polishedPos = worldPosition - _mapAnchorPosition;
            //地图坐标转化为uv起点
            Vector2 mapPoint = new Vector2(polishedPos.x / _mapSize.x, polishedPos.z / _mapSize.y);
            Vector2 uvHalf = _uv / 2;
            
            Vector2 startPointRaw = mapPoint - uvHalf;
            Vector2 startPoint = GetAdjustedStartPoint(startPointRaw);

            Rect rect = new Rect(startPoint, _uv);
            if (_mapTexture != null)
            {
                _mapTexture.uvRect = rect;
            }
            _currentPosition = worldPosition;
        }

        public void UpdateElementPosition()
        {

        }

        public bool CheckElementVisible(Vector3 worldPostion)
        {
            Vector2 elementPos = GetElementPosition(worldPostion);
            Vector2 halfMapShowSize = _mapShowSize / 2;
            //判断图标是否在圆形区域外, 需要额外加上剔除宽度, 避免穿帮
            return elementPos.magnitude < halfMapShowSize.magnitude - _elementCullWidth;
        }

        public Vector2 GetElementPosition(Vector3 worldPosition)
        {
            Vector3 offsetVec3 = worldPosition - _currentPosition;
            Vector2 offset = new Vector2(offsetVec3.x, offsetVec3.z);
            //世界坐标距离到UI坐标距离转换
            Vector2 uiOffset = offset * _mapScale / ScaleFactor;
            return uiOffset;
        }

        public void AddMapElement(int frequency, bool updateRotation)
        {
            
            
        }

        public void RemoveMapElement(MiniMapElement element)
        {

        }

        private void SetUVSize()
        {
            Vector2 uvRaw;
            float ratio = _mapSize.x / _mapSize.y;
            if (_mapTexture)
            {
                RectTransform rectTrans = _mapTexture.GetComponent<RectTransform>();
                _mapShowSize = rectTrans.sizeDelta;
            }
            //原始uv
            if (_mapSize.x > _mapSize.y)
            {
                uvRaw = new Vector2(1f / ratio, 1f);
                _mapScale = _mapSize.y / _mapShowSize.y;
            }
            else
            {
                uvRaw = new Vector2(1f, 1f / ratio);
                _mapScale = _mapSize.x / _mapShowSize.x;
            }
            //缩放uv
            _uv = uvRaw * _scaleFactor;
        }

        private Vector2 GetAdjustedStartPoint(Vector2 point)
        {
            Vector2 adjPoint = point;
            
            //uvRect起点限定区域
            Vector2 constrainX = new Vector2(0, 1f - _uv.x);
            Vector2 constrainY = new Vector2(0, 1f - _uv.y);

            if (adjPoint.x < constrainX.x) adjPoint.x = constrainX.x;
            if (adjPoint.x > constrainX.y) adjPoint.x = constrainX.y;

            if (adjPoint.y < constrainY.x) adjPoint.y = constrainY.x;
            if (adjPoint.y > constrainY.y) adjPoint.y = constrainY.y;
            
            return adjPoint;
        }

        

    }

}