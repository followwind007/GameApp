using System.Collections.Generic;
using Tools.Table;
using UnityEngine;

namespace GameApp.AnimationRigging
{
    public class RigBoneRender : MonoBehaviour
    {
#if UNITY_EDITOR
        public enum JointType
        {
            Sphere, Box, None
        }
        
        [ReorderableItem]
        public List<Transform> joints;

        public JointType jointType;
        public bool showAxis;
        
        public float jointRange = 0.3f;
        
        [LineItem("BoneColor")]
        public Color boneColorHead = Color.blue;
        [LineItem("BoneColor")]
        public Color boneColorTail = Color.white;
        
        [LineItem("JointColor")]
        public Color jointColorHead = Color.magenta;
        [LineItem("JointColor")]
        public Color jointColorTail = Color.white;

        private void OnDrawGizmos()
        {
            var oldColor = Gizmos.color;
            
            for (var i = 0; i < joints.Count; i++)
            {
                var jColor = Color.Lerp(jointColorHead, jointColorTail, i / (joints.Count - 1f));
                Gizmos.color = jColor;
                
                var j = joints[i];
                switch (jointType)
                {
                    case JointType.Sphere:
                        Gizmos.DrawWireSphere(j.position, jointRange);
                        break;
                    case JointType.Box:
                        Gizmos.DrawWireCube(j.position, Vector3.one * jointRange);
                        break;
                    case JointType.None:
                        break;
                }
                
                if (showAxis)
                {
                    DrawJointAxis(j);
                }
                
                if (i > 0)
                {
                    var bColor = Color.Lerp(boneColorHead, boneColorTail, i / (joints.Count - 1f));
                    Gizmos.color = bColor;
                    Gizmos.DrawLine(j.position, joints[i-1].position);
                }
            }

            Gizmos.color = oldColor;
        }

        private void DrawJointAxis(Transform t)
        {
            var oldColor = Gizmos.color;
            var start = t.position;
            
            var forwardEnd = start + t.forward * jointRange;
            Gizmos.color = Color.blue;
            Gizmos.DrawLine(start, forwardEnd);
            
            var upEnd = start + t.up * jointRange;
            Gizmos.color = Color.green;
            Gizmos.DrawLine(start, upEnd);
            
            var rightEnd = start + t.right * jointRange;
            Gizmos.color = Color.red;
            Gizmos.DrawLine(start, rightEnd);

            Gizmos.color = oldColor;
        }

#endif
    }
}