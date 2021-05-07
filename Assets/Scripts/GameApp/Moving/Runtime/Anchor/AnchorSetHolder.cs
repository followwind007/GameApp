using System;
using System.Collections.Generic;
using UnityEngine;

namespace GameApp.Moving
{
    public class AnchorSetHolder : MonoBehaviour
    {
        #if UNITY_EDITOR
        public const string PropSet = "set";
        #endif
        
        public AnchorSet set;

        private List<AnchorPoint> _points;
        public List<AnchorPoint> Points {
            get
            {
                if (_points == null)
                {
                    //copy data in editor for friendly preview of 'AnchorSet'
                    #if UNITY_EDITOR
                    _points = new List<AnchorPoint>(set.Points.Count);
                    #else
                    _points = set.Points;
                    #endif

                    // ReSharper disable once ForCanBeConvertedToForeach
                    for (var i = 0; i < set.Points.Count; i++)
                    {
                        var p = set.Points[i];
                        var point = new AnchorPoint(transform.TransformPoint(p.position), p.rotation);
                        #if UNITY_EDITOR
                        _points.Add(point);
                        #else
                        _points[i] = point;
                        #endif
                    }
                }
                return _points;
            }
        }
        
        /// <summary>
        /// get projection vector on plane
        /// </summary>
        /// <param name="vp">projection vector</param>
        /// <param name="vn">normal vector</param>
        /// <returns></returns>
        public Vector3 ProjectOnPlane(Vector3 vp, Vector3 vn)
        {
            return vp - vn * Vector3.Dot(vp, vn) / Vector3.Dot(vn, vn);
        }

        /// <summary>
        /// check if 2 line intersect, return intersect point
        /// https://blog.csdn.net/xdedzl/article/details/86009147
        /// </summary>
        /// <param name="p1">point on line1</param>
        /// <param name="v1">direction of line1</param>
        /// <param name="p2">point on line2</param>
        /// <param name="v2">direction of line2</param>
        /// <param name="intersection">intersect point</param>
        /// <param name="insideLine">cross point inside line</param>
        /// <returns>intersect or not</returns>
        public static bool LineIntersection(Vector3 p1, Vector3 v1, Vector3 p2, Vector3 v2, out Vector3 intersection)
        {
            intersection = Vector3.zero;
            //parallel
            if (Math.Abs(Vector3.Dot(v1, v2) - 1) < 1E-05f) return false;

            var seg = p2 - p1;
            //area1
            var vecS1 = Vector3.Cross(v1, v2);
            //area2
            var vecS2 = Vector3.Cross(seg, v2);
            var num = Vector3.Dot(seg, vecS1);

            //not in same plane
            if (Math.Abs(num) >= 1E-05f) return false;

            //area ratio
            var num2 = Vector3.Dot(vecS2, vecS1) / vecS1.sqrMagnitude;
            intersection = p1 + v1 * num2;
            return true;
        }
        
        public bool GetCrossPoint(Vector3 start, Vector3 direction, out Vector3 cross)
        {
            cross = Vector3.zero;
            
            GetHeadTwo(p => Vector3.Angle(direction, p.position - start), out var idx1, out var idx2);

            if (idx1 < 0 || idx2 < 0) return false;
            
            var p1 = Points[idx1].position;
            var p2 = Points[idx2].position;

            var vn = Vector3.Cross(p1 - start, p2 - start);
            var pj = ProjectOnPlane(direction, vn);

            var isCross = LineIntersection(start, pj, p1, p2 - p1, out cross);

            if (isCross)
            {
                var betweenLine = Vector3.Dot(p2 - cross, p1 - cross) < 0;
                return betweenLine;
            }
            
            return false;
        }

        public bool GetAttachPoint(Vector3 start, Vector3 direction, out Vector3 attach)
        {
            attach = Vector3.zero;
            
            GetHeadTwo(p => Vector3.Distance(start, p.position), out var idx1, out var idx2);

            if (idx1 < 0 || idx2 < 0) return false;
            
            var p1 = Points[idx1].position;
            var p2 = Points[idx2].position;

            var pDir = (p2 - p1).normalized;
            var dist = Vector3.Dot(pDir, direction);

            attach = p1 + pDir * dist;

            return true;
        }

        private void GetHeadTwo(Func<AnchorPoint, float> getVal, out int idx1, out int idx2)
        {
            idx1 = idx2 = -1;
            float val1 = float.MaxValue, val2 = float.MaxValue;
            var count = Points.Count;
            for (var i = 0; i < count; i++)
            {
                var p = Points[i];
                var val = getVal(p);
                if (val < val1)
                {
                    val2 = val1;
                    idx2 = idx1;
                    val1 = val;
                    idx1 = i;
                }
                else if (val < val2)
                {
                    val2 = val;
                    idx2 = i;
                }
            }
        }

        #if UNITY_EDITOR
        private void OnDrawGizmos()
        {
            if (set.Points == null) return;

            var preColor = Gizmos.color;
            
            Gizmos.color = new Color(1, 1, 0, 0.5f);
            foreach (var p in set.Points)
            {
                var pos = transform.TransformPoint(p.position);
                
                Gizmos.DrawCube(pos, Vector3.one * 0.3f);
            }

            Gizmos.color = preColor;
        }
        #endif
    }
}