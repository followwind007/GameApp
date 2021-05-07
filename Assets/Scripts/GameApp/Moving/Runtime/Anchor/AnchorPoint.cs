using UnityEngine;

namespace GameApp.Moving
{
    [System.Serializable]
    public struct AnchorPoint
    {
        #if UNITY_EDITOR
        public const string PropPosition = "position";
        public const string PropRotation = "rotation";
        #endif
        
        public Vector3 position;
        public Quaternion rotation;

        public AnchorPoint(Vector3 position, Quaternion rotation)
        {
            this.position = position;
            this.rotation = rotation;
        }
    }
}