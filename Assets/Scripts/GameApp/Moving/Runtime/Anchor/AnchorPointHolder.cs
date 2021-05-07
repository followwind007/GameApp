using UnityEngine;
#if UNITY_EDITOR
using UnityEditor;
#endif

namespace GameApp.Moving
{
    public class AnchorPointHolder : MonoBehaviour
    {
        #if UNITY_EDITOR
        public const string PropPoint = "point";
        #endif
        public AnchorPoint point;

        #if UNITY_EDITOR
        private void OnDrawGizmos()
        {
            var isSelected = Selection.activeObject is GameObject g && g == gameObject;
            
            var preColor = Gizmos.color;
            var preHandleColor = Handles.color;

            Handles.color = new Color(0, 1, 0, isSelected ? 1f : 0.3f);
            Handles.SphereHandleCap(0, transform.position, Quaternion.identity, 0.1f, EventType.Repaint);
            
            Gizmos.color = preColor;
            Handles.color = preHandleColor;
        }
        #endif
    }
}