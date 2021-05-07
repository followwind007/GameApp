using System;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

namespace GameApp.UI
{
    public class CustomMesh : BaseMeshEffect
    {
        public enum MeshType
        {
            Outline = 0,
            Skeleton = 1,
            Fill = 2,
        }
        
        public enum VerticeType
        {
            EvenParams = 0,
            CustomPositions = 1,
        }

        private const float FloatTolerance = 0.0001f;

        private static readonly List<UIVertex> TempVertexTriangleStream = new List<UIVertex>();
        private static readonly List<UIVertex> TempInnerVertices = new List<UIVertex>();
        private static readonly List<UIVertex> TempOutterVertices = new List<UIVertex>();

        [SerializeField]
        private MeshType drawType;

        [SerializeField] 
        private VerticeType verticeType = VerticeType.EvenParams;
        public MeshType DrawType
        {
            get { return drawType; }
            set { drawType = value; graphic.SetVerticesDirty(); }
        }

        [SerializeField]
        private float[] parameters;

        public float[] Parameters
        {
            get { return parameters; }
            set { parameters = value; graphic.SetVerticesDirty(); }
        }
        
        [SerializeField]
        private List<Vector2> positions = new List<Vector2>();

        [SerializeField, Range(0f, 360f)]
        private float startAngleDegree;
        public float StartAngleDegree
        {
            get { return startAngleDegree; }
            set { startAngleDegree = value; graphic.SetVerticesDirty(); }
        }

        [SerializeField, Range(-1f, 1f), Tooltip("width related to canvas size")]
        private float lineWidth = 0.1f;
        public float LineWidth
        {
            get { return lineWidth; }
            set { lineWidth = value; graphic.SetVerticesDirty(); }
        }

        [SerializeField]
        private Color outerColor = Color.white;
        public Color OuterColor
        {
            get { return outerColor; }
            set { outerColor = value; graphic.SetVerticesDirty(); }
        }

        [SerializeField]
        private Color innerColor = Color.clear;
        public Color InnerColor
        {
            get { return innerColor; }
            set { innerColor = value; graphic.SetVerticesDirty(); }
        }

        private float? _cacheStartAngleDegree;
        private readonly List<float> _cacheSines = new List<float>();
        private readonly List<float> _cacheCosines = new List<float>();

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

            vh.GetUIVertexStream(TempVertexTriangleStream);

            ModifyVertices(TempVertexTriangleStream);

            vh.Clear();
            vh.AddUIVertexTriangleStream(TempVertexTriangleStream);

            TempVertexTriangleStream.Clear();
        }

        private void ModifyVertices(List<UIVertex> vertices)
        {
            GetVertex(vertices);
            switch (drawType)
            {
                case MeshType.Outline:
                    ModifiyForOutline(vertices);
                    break;
                case MeshType.Skeleton:
                    ModifyForSkeleton(vertices);
                    break;
                case MeshType.Fill:
                    ModifyForFill(vertices);
                    break;
            }
        }

        private void GetVertex(List<UIVertex> vertices)
        {
            if (parameters == null)
                return;

            if (NeedsToUpdateCaches())
                CacheSinesAndCosines();

            TempInnerVertices.Clear();

            _centerPosition = (vertices[0].position + vertices[2].position) / 2f;
            _xUnit = (_centerPosition.x - vertices[0].position.x) * Vector3.right;
            _yUnit = (_centerPosition.y - vertices[0].position.y) * Vector3.up;

            _centerUv = (vertices[0].uv0 + vertices[2].uv0) / 2f;
            Vector2 uUnit = (_centerUv.x - vertices[0].uv0.x) * Vector3.right;
            Vector2 vUnit = (_centerUv.y - vertices[0].uv0.y) * Vector3.up;

            _outterMultipliedColor = GetMultipliedColor(vertices[0].color, outerColor);
            _innerMultipliedColor = GetMultipliedColor(vertices[0].color, innerColor);

            switch (verticeType)
            {
                case VerticeType.EvenParams:
                    for (var i = 0; i < parameters.Length; i++)
                    {
                        var parameter = parameters[i];
                        var cosine = _cacheCosines[i];
                        var sine = _cacheSines[i];
                        var innerVertex = vertices[0];
                        innerVertex.position = _centerPosition + (_xUnit * cosine + _yUnit * sine) * parameter;
                        var offset = (uUnit * cosine + vUnit * sine) * parameter;
                        innerVertex.uv0 = _centerUv + new Vector4(offset.x, offset.y);
                        innerVertex.color = drawType == MeshType.Fill ? _outterMultipliedColor : _innerMultipliedColor;
                        TempInnerVertices.Add(innerVertex);
                    }
                    break;
                case VerticeType.CustomPositions:
                    foreach (var pos in positions)
                    {
                        var vert = vertices[0];
                        vert.uv0 = pos;
                        var offset = new Vector3(pos.x * _xUnit.magnitude * 2, pos.y * _yUnit.magnitude * 2);
                        vert.position = vertices[0].position + offset;
                        vert.color = drawType == MeshType.Fill ? _outterMultipliedColor : _innerMultipliedColor;
                        TempInnerVertices.Add(vert);
                    }
                    break;
            }
        }
        

        private void ModifiyForOutline(List<UIVertex> vertices)
        {
            TempOutterVertices.Clear();
            for (var i = 0; i < parameters.Length; i++)
            {
                var indexPre = i - 1 < 0 ? parameters.Length - 1 : i - 1;
                var indexNext = i + 1 >= parameters.Length ? 0 : i + 1;
                var vertInnerCur = TempInnerVertices[i];
                var vertInnerPre = TempInnerVertices[indexPre];
                var vertInnerNext = TempInnerVertices[indexNext];
                var vecPre = (vertInnerPre.uv0 - vertInnerCur.uv0).normalized;
                var vecNext = (vertInnerNext.uv0 - vertInnerCur.uv0).normalized;

                var angle = Vector2.SignedAngle(vecPre, vecNext);
                var sin = Mathf.Sin(Mathf.Deg2Rad * (Mathf.Abs(angle) / 2f));
                var len = lineWidth * (1f / sin);
                var direct = (vecPre + vecNext).normalized;
                if (angle > 0f) direct = -direct;
                var offset = direct * len;

                var outterVertex = vertices[0];
                outterVertex.uv0 = vertInnerCur.uv0 + offset;
                outterVertex.position = vertInnerCur.position + new Vector3(offset.x * _xUnit.magnitude, offset.y * _yUnit.magnitude, 0) * 2;
                outterVertex.color = _outterMultipliedColor;
                TempOutterVertices.Add(outterVertex);
            }

            if (parameters.Length > 0)
            {
                TempOutterVertices.Add(TempOutterVertices[0]);
                TempInnerVertices.Add(TempInnerVertices[0]);
            }

            vertices.Clear();
            if (Math.Abs(lineWidth) > FloatTolerance)
            {
                for (var i = 0; i < parameters.Length; i++)
                {
                    vertices.Add(TempInnerVertices[i]);
                    vertices.Add(TempOutterVertices[i]);
                    vertices.Add(TempOutterVertices[i + 1]);

                    vertices.Add(TempOutterVertices[i + 1]);
                    vertices.Add(TempInnerVertices[i + 1]);
                    vertices.Add(TempInnerVertices[i]);
                }
            }
        }

        private void ModifyForSkeleton(List<UIVertex> vertices)
        {
            var vert0 = vertices[0];
            vertices.Clear();
            for (var i = 0; i < parameters.Length; i++)
            {
                var vert = TempInnerVertices[i];
                Vector3 direct = vert.uv0 - _centerUv;
                var right3 = -(Quaternion.AngleAxis(90f, Vector3.forward) * direct).normalized;
                Vector4 right = new Vector4(right3.x, right3.y, right3.z);
                var left = -right;
                //four corners
                var halfWidth = lineWidth / 2;
                var bl = _centerUv + left * halfWidth;
                var br = _centerUv + right * halfWidth;
                var tl = vert.uv0 + left * halfWidth;
                var tr = vert.uv0 + right * halfWidth;

                var vbl = vert0;
                vbl.uv0 = bl;
                vbl.position = _centerPosition + new Vector3(left.x * _xUnit.magnitude, left.y * _yUnit.magnitude, 0) * lineWidth;
                vbl.color = _innerMultipliedColor;

                var vbr = vert0;
                vbr.uv0 = br;
                vbr.position = _centerPosition + new Vector3(right.x * _xUnit.magnitude, right.y * _yUnit.magnitude, 0) * lineWidth;
                vbr.color = _innerMultipliedColor;

                var vtl = vert0;
                vtl.uv0 = tl;
                vtl.position = vert.position + new Vector3(left.x * _xUnit.magnitude, left.y * _yUnit.magnitude, 0) * lineWidth;
                vtl.color = _outterMultipliedColor;

                var vtr = vert0;
                vtr.uv0 = tr;
                vtr.position = vert.position + new Vector3(right.x * _xUnit.magnitude, right.y * _yUnit.magnitude, 0) * lineWidth;
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
            for (var i = 0; i < TempInnerVertices.Count; i++)
            {
                var indexNext = i + 1 < TempInnerVertices.Count ? i + 1 : 0;
                vertices.Add(vertCenter);
                vertices.Add(TempInnerVertices[i]);
                vertices.Add(TempInnerVertices[indexNext]);
            }
        }

        private bool NeedsToUpdateCaches()
        {
            return
                !_cacheStartAngleDegree.HasValue ||
                Math.Abs(_cacheStartAngleDegree.Value - startAngleDegree) > FloatTolerance ||
                _cacheSines.Count != parameters.Length;
        }

        private void CacheSinesAndCosines()
        {
            _cacheSines.Clear();
            _cacheCosines.Clear();

            var startAngleRadian = (90f - startAngleDegree) / 180f * (float)Math.PI;
            var unitRadian = -2f * (float)Math.PI / parameters.Length;

            for (var i = 0; i < parameters.Length; i++)
            {
                var radian = startAngleRadian + i * unitRadian;
                _cacheSines.Add(Mathf.Sin(radian));
                _cacheCosines.Add(Mathf.Cos(radian));
            }

            _cacheStartAngleDegree = startAngleDegree;
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