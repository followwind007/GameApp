using System;
using System.Collections.Generic;
using UnityEngine;

namespace GameApp.Moving
{
    [Serializable]
    public struct AnchorSet
    {
        #if UNITY_EDITOR
        public const string PropPoints = "points";
        #endif
        
        [SerializeField]
        private List<AnchorPoint> points;

        public List<AnchorPoint> Points => points;

        public AnchorSet(int size)
        {
            points = new List<AnchorPoint>(size);
        }
    }
}