using Tools.Table;
using UnityEngine;

namespace GameApp.AnimationRigging
{
    public class RigAimBehaviour : MonoBehaviour
    {
        public bool playOnAwake = true;
        
        [Range(0f, 1f)][CustomName("权重")]
        public float weight = 1f;

        [Range(0f, 60f)][CustomName("旋转速度")]
        public float speed = 30f;
        
        [CustomName("约束节点")]
        public Transform joint;
        [CustomName("目标偏移")]
        public Vector3 offset;
        [CustomName("目标节点")]
        public Transform target;
        
        public AxisUtil.Axis axis;

        [MinMax(-180f, 180f, true, true)][CustomName("X轴约束")]
        public Vector2 xAxisRange;
        [MinMax(-180f, 180f, true, true)][CustomName("Y轴约束")]
        public Vector2 yAxisRange;
        [MinMax(-180f, 180f, true, true)][CustomName("Z轴约束")]
        public Vector2 zAxisRange;

        [CustomName("自动回正")]
        public bool autoTweenBack;

        public bool lockRotate;
        public bool isTweening = true;

        [HideInInspector]
        public Quaternion lastRotation;
        
        private bool _isValid;

        public void StartProcess()
        {
            if (_isValid) return;
            
            _isValid = true;
            lastRotation = joint.rotation;
        }

        public void StopProcess()
        {
            _isValid = false;
        }
        
        private void OnEnable()
        {
            if (playOnAwake)
            {
                StartProcess();
            }
        }
        
        private void OnDisable()
        {
            StopProcess();
        }

        private void LateUpdate()
        {
            if (_isValid)
            {
                ProcessAnimation();
            }
        }

        private void ProcessAnimation()
        {
            var weightVal = weight;
            var deltaTime = Time.deltaTime;

            if (weightVal <= 0f || deltaTime <= 0f) return;
            
            var jointRotation = lastRotation;
            
            if (isTweening && !lockRotate)
            {
                var fromDir = jointRotation * AxisUtil.GetAxisVector(axis);
                var toDir = target.position + offset - joint.position;

                var newAxis = Vector3.Cross(fromDir, toDir).normalized;

                //rotate formDir to toDir
                var jointToTargetRotation = Quaternion.AngleAxis(Vector3.Angle(fromDir, toDir) * weightVal, newAxis);
                
                jointRotation = jointToTargetRotation * jointRotation;
                var parentRot = joint.parent.rotation;
                var localRot = Quaternion.Inverse(parentRot) * jointRotation;
                var localRotEuler = AxisUtil.GetClampedRotation(localRot.eulerAngles);

                //clamp with axis range
                var rotEuler = new Vector3(
                    Mathf.Clamp(localRotEuler.x, xAxisRange.x, xAxisRange.y),
                    Mathf.Clamp(localRotEuler.y, yAxisRange.x, yAxisRange.y),
                    Mathf.Clamp(localRotEuler.z, zAxisRange.x, zAxisRange.y));
                
                jointRotation = rotEuler != localRotEuler && autoTweenBack ? joint.rotation : parentRot * Quaternion.Euler(rotEuler);
            }
            else
            {
                jointRotation = joint.rotation;
            }

            lastRotation = Quaternion.Lerp(lastRotation, jointRotation, deltaTime * speed);

            joint.rotation = lastRotation;
        }
        
    }
}