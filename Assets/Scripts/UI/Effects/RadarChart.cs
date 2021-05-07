using System;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

namespace GameApp.UI
{
    public class RadarChart : BaseMeshEffect
    {
        public enum RadarType
        {
            Outline = 0,
            Skeleton = 1,
            Fill = 2,
        }

        private static List<UIVertex> _tempVertexTriangleStream = new List<UIVertex>();
        private static List<UIVertex> _tempInnerVertices = new List<UIVertex>();
        private static List<UIVertex> _tempOutterVertices = new List<UIVertex>();

        [SerializeField]
        private RadarType _drawType;
        public RadarType DrawType
        {
            get { return _drawType; }
            set { _drawType = value; graphic.SetVerticesDirty(); }
        }

        [SerializeField]
        private float[] _parameters;
        public float[] Parameters
        {
            get { return _parameters; }
            set { _parameters = value; graphic.SetVerticesDirty(); }
        }

        [SerializeField, Range(0f, 360f)]
        private float _startAngleDegree;
        public float StartAngleDegree
        {
            get { return _startAngleDegree; }
            set { _startAngleDegree = value; graphic.SetVerticesDirty(); }
        }

        [SerializeField, Range(-1f, 1f), Tooltip("width related to canvas size")]
        private float _lineWidth = 0.1f;
        public float LineWidth
        {
            get { return _lineWidth; }
            set { _lineWidth = value; graphic.SetVerticesDirty(); }
        }

        [SerializeField]
        private Color _outerColor = Color.white;
        public Color OuterColor
        {
            get { return _outerColor; }
            set { _outerColor = value; graphic.SetVerticesDirty(); }
        }

        [SerializeField]
        private Color _innerColor = Color.clear;
        public Color InnerColor
        {
            get { return _innerColor; }
            set { _innerColor = value; graphic.SetVerticesDirty(); }
        }

        private float? _cacheStartAngleDegree;
        private List<float> _cacheSines = new List<float>();
        private List<float> _cacheCosines = new List<float>();

        private Vector4 _centerUv;

        private Vector3 _centerPosition;
        private Vector3 _xUnit;
        private Vector3 _yUnit;

        private Color _innerMultipliedColor;
        private Color _outterMultipliedColor;

        public override void ModifyMesh(VertexHelper vh)
        {
            if (!IsActive())
                return;

            vh.GetUIVertexStream(_tempVertexTriangleStream);

            ModifyVertices(_tempVertexTriangleStream);

            vh.Clear();
            vh.AddUIVertexTriangleStream(_tempVertexTriangleStream);

            _tempVertexTriangleStream.Clear();
        }

        private void ModifyVertices(List<UIVertex> vertices)
        {
            GetVertex(vertices);
            switch (_drawType)
            {
                case RadarType.Outline:
                    ModifiyForOutline(vertices);
                    break;
                case RadarType.Skeleton:
                    ModifyForSkeleton(vertices);
                    break;
                case RadarType.Fill:
                    ModifyForFill(vertices);
                    break;
                default:
                    break;
            }
        }

        private void GetVertex(List<UIVertex> vertices)
        {
            if (_parameters == null)
                return;

            if (NeedsToUpdateCaches())
                CacheSinesAndCosines();

            _tempInnerVertices.Clear();

            _centerPosition = (vertices[0].position + vertices[2].position) / 2f;
            _xUnit = (_centerPosition.x - vertices[0].position.x) * Vector3.right;
            _yUnit = (_centerPosition.y - vertices[0].position.y) * Vector3.up;

            _centerUv = (vertices[0].uv0 + vertices[2].uv0) / 2f;
            Vector2 uUnit = (_centerUv.x - vertices[0].uv0.x) * Vector3.right;
            Vector2 vUnit = (_centerUv.y - vertices[0].uv0.y) * Vector3.up;

            _outterMultipliedColor = GetMultipliedColor(vertices[0].color, _outerColor);
            _innerMultipliedColor = GetMultipliedColor(vertices[0].color, _innerColor);

            for (var i = 0; i < _parameters.Length; i++)
            {
                var parameter = _parameters[i];
                var cosine = _cacheCosines[i];
                var sine = _cacheSines[i];
                var innerVertex = vertices[0];
                innerVertex.position = _centerPosition + (_xUnit * cosine + _yUnit * sine) * parameter;
                var offset = (uUnit * cosine + vUnit * sine) * parameter;
                innerVertex.uv0 = _centerUv + new Vector4(offset.x, offset.y);
                innerVertex.color = _drawType == RadarType.Fill ? _outterMultipliedColor : _innerMultipliedColor;
                _tempInnerVertices.Add(innerVertex);
            }
        }

        private void ModifiyForOutline(List<UIVertex> vertices)
        {
            _tempOutterVertices.Clear();
            for (var i = 0; i < _parameters.Length; i++)
            {
                var indexPre = i - 1 < 0 ? _parameters.Length - 1 : i - 1;
                var indexNext = i + 1 >= _parameters.Length ? 0 : i + 1;
                var vertInnerCur = _tempInnerVertices[i];
                var vertInnerPre = _tempInnerVertices[indexPre];
                var vertInnerNext = _tempInnerVertices[indexNext];
                Vector2 vecPre = (vertInnerPre.uv0 - vertInnerCur.uv0).normalized;
                Vector2 vecNext = (vertInnerNext.uv0 - vertInnerCur.uv0).normalized;

                var angle = Vector2.SignedAngle(vecPre, vecNext);
                var sin = Mathf.Sin(Mathf.Deg2Rad * (Mathf.Abs(angle) / 2f));
                var len = _lineWidth * (1f / sin);
                var direct = (vecPre + vecNext).normalized;
                if (angle > 0f) direct = -direct;
                var offset = direct * len;

                var outterVertex = vertices[0];
                outterVertex.uv0 = vertInnerCur.uv0 + new Vector4(offset.x, offset.y);
                outterVertex.position = vertInnerCur.position + new Vector3(offset.x * _xUnit.magnitude, offset.y * _yUnit.magnitude, 0) * 2;
                outterVertex.color = _outterMultipliedColor;
                _tempOutterVertices.Add(outterVertex);
            }

            if (_parameters.Length > 0)
            {
                _tempOutterVertices.Add(_tempOutterVertices[0]);
                _tempInnerVertices.Add(_tempInnerVertices[0]);
            }

            vertices.Clear();
            if (_lineWidth != 0f)
            {
                for (var i = 0; i < _parameters.Length; i++)
                {
                    vertices.Add(_tempInnerVertices[i]);
                    vertices.Add(_tempOutterVertices[i]);
                    vertices.Add(_tempOutterVertices[i + 1]);

                    vertices.Add(_tempOutterVertices[i + 1]);
                    vertices.Add(_tempInnerVertices[i + 1]);
                    vertices.Add(_tempInnerVertices[i]);
                }
            }
        }

        private void ModifyForSkeleton(List<UIVertex> vertices)
        {
            var vert0 = vertices[0];
            vertices.Clear();
            for (var i = 0; i < _parameters.Length; i++)
            {
                var vert = _tempInnerVertices[i];
                Vector3 direct = vert.uv0 - _centerUv;
                var right3 = -(Quaternion.AngleAxis(90f, Vector3.forward) * direct).normalized;
                var right = new Vector4(right3.x, right3.y, right3.z);
                var left = -right;
                //four corners
                Vector2 bl = _centerUv + left * _lineWidth / 2;
                Vector2 br = _centerUv + right * _lineWidth / 2;
                Vector2 tl = vert.uv0 + left * _lineWidth / 2;
                Vector2 tr = vert.uv0 + right * _lineWidth / 2;

                var vbl = vert0;
                vbl.uv0 = bl;
                vbl.position = _centerPosition + new Vector3(left.x * _xUnit.magnitude, left.y * _yUnit.magnitude, 0) * _lineWidth;
                vbl.color = _innerMultipliedColor;

                var vbr = vert0;
                vbr.uv0 = br;
                vbr.position = _centerPosition + new Vector3(right.x * _xUnit.magnitude, right.y * _yUnit.magnitude, 0) * _lineWidth;
                vbr.color = _innerMultipliedColor;

                var vtl = vert0;
                vtl.uv0 = tl;
                vtl.position = vert.position + new Vector3(left.x * _xUnit.magnitude, left.y * _yUnit.magnitude, 0) * _lineWidth;
                vtl.color = _outterMultipliedColor;

                var vtr = vert0;
                vtr.uv0 = tr;
                vtr.position = vert.position + new Vector3(right.x * _xUnit.magnitude, right.y * _yUnit.magnitude, 0) * _lineWidth;
                vtr.color = _outterMultipliedColor;

                vertices.Add(vbl);
                vertices.Add(vtl);
                vertices.Add(vtr);

                vertices.Add(vtr);
                vertices.Add(vbr);
                vertices.Add(vbl);
            }
        }

        private void ModifyForFill(List<UIVertex> vertices)
        {
            var vertCenter = vertices[0];
            vertCenter.uv0 = _centerUv;
            vertCenter.position = _centerPosition;
            vertCenter.color = _innerMultipliedColor;
            vertices.Clear();
            for (var i = 0; i < _tempInnerVertices.Count; i++)
            {
                var indexNext = i + 1 < _tempInnerVertices.Count ? i + 1 : 0;
                vertices.Add(vertCenter);
                vertices.Add(_tempInnerVertices[i]);
                vertices.Add(_tempInnerVertices[indexNext]);
            }
        }

        private bool NeedsToUpdateCaches()
        {
            return
                !_cacheStartAngleDegree.HasValue ||
                Math.Abs(_cacheStartAngleDegree.Value - _startAngleDegree) > 0.001f ||
                _cacheSines.Count != _parameters.Length;
        }

        private void CacheSinesAndCosines()
        {
            _cacheSines.Clear();
            _cacheCosines.Clear();

            var startAngleRadian = (90f - _startAngleDegree) / 180f * (float)Math.PI;
            var unitRadian = -2f * (float)Math.PI / _parameters.Length;

            for (var i = 0; i < _parameters.Length; i++)
            {
                var radian = startAngleRadian + i * unitRadian;
                _cacheSines.Add(Mathf.Sin(radian));
                _cacheCosines.Add(Mathf.Cos(radian));
            }

            _cacheStartAngleDegree = _startAngleDegree;
        }

        private Color32 GetMultipliedColor(Color32 color1, Color32 color2)
        {
            return new Color32(
                (byte)(color1.r * color2.r / 255),
                (byte)(color1.g * color2.g / 255),
                (byte)(color1.b * color2.b / 255),
                (byte)(color1.a * color2.a / 255));
        }
    }

}