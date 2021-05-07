using System;
using Cinemachine;
using UnityEngine;
using UnityEngine.Playables;

namespace GameApp.Timeline
{
    [Serializable]
    public class PathPlayableBehaviour : PlayableBehaviour
    {
        [HideInInspector]
        public CinemachinePath path;
        
        public float evaluateScale = 1f;
        public float offset;

        public Vector3 EvaluatePosition(float time)
        {
            return path.EvaluatePositionAtUnit((time + offset) * evaluateScale, CinemachinePathBase.PositionUnits.Normalized);
        }

        public Quaternion EvaluateRotation(float time)
        {
            return path.EvaluateOrientationAtUnit((time + offset) * evaluateScale, CinemachinePathBase.PositionUnits.Normalized);
        }

    }

}