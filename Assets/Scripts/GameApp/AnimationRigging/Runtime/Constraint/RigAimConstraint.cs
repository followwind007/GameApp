using Tools.Table;
using UnityEngine;
#if UNITY_2019_3_OR_NEWER
using UnityEngine.Animations;
#else
using UnityEngine.Experimental.Animations;
#endif
using UnityEngine.Playables;

namespace GameApp.AnimationRigging
{
    public class RigAimConstraint : RigConstraintBehaviour
    {
        public struct AimJob : IAnimationJob
        {
            public FloatProperty weight;
            public FloatProperty speed;
            public TransformStreamHandle joint;
            public TransformStreamHandle parent;
            public Vector3Property offset;
            public Vector3Property targetPosition;
            public Vector3 axis;

            public Vector2Property xAxisRange;
            public Vector2Property yAxisRange;
            public Vector2Property zAxisRange;

            public Quaternion lastRotation;

            public bool autoTweenBack;

            public void ProcessAnimation(AnimationStream stream)
            {
                var weightVal = weight.Get(stream);
                var deltaTime = stream.deltaTime;
                
                var jointPosition = joint.GetPosition(stream);
                var jointRotation = lastRotation;
                
                if (weightVal <= 0f || deltaTime <= 0f)
                {
                    return;
                }

                var targetPos = targetPosition.Get(stream);

                var fromDir = jointRotation * axis;
                var toDir = targetPos - jointPosition;

                var newAxis = Vector3.Cross(fromDir, toDir).normalized;

                //rotate formDir to toDir
                var jointToTargetRotation = Quaternion.AngleAxis(Vector3.Angle(fromDir, toDir) * weightVal, newAxis);

                jointRotation = jointToTargetRotation * jointRotation;

                var parentRot = parent.GetRotation(stream);
                var localRot = Quaternion.Inverse(parentRot) * jointRotation;
                var localRotEuler = AxisUtil.GetClampedRotation(localRot.eulerAngles);

                var xRange = xAxisRange.Get(stream);
                var yRange = yAxisRange.Get(stream);
                var zRange = zAxisRange.Get(stream);
                
                //clamp with axis range
                var rotEuler = new Vector3(
                    Mathf.Clamp(localRotEuler.x, xRange.x, xRange.y),
                    Mathf.Clamp(localRotEuler.y, yRange.x, yRange.y),
                    Mathf.Clamp(localRotEuler.z, zRange.x, zRange.y));
                
                jointRotation = rotEuler != localRotEuler && autoTweenBack ? joint.GetRotation(stream) : parentRot * Quaternion.Euler(rotEuler);

                //apply offset rotation
                var offsetVal = offset.Get(stream);
                if (offsetVal.magnitude > 0f) jointRotation *= Quaternion.Euler(offsetVal);
                
                lastRotation = Quaternion.Lerp(lastRotation, jointRotation, deltaTime * speed.Get(stream));

                joint.SetRotation(stream, lastRotation);
            }

            public void ProcessRootMotion(AnimationStream stream)
            {
                
            }
        }
        
        [SyncToStream][Range(0f, 1f)][CustomName("权重")]
        public float weight = 1f;

        [SyncToStream][Range(0f, 60f)][CustomName("旋转速度")]
        public float speed = 30f;
        
        [CustomName("约束节点")]
        public Transform joint;
        [SyncToStream][CustomName("旋转偏移")]
        public Vector3 offset;
        [CustomName("目标节点")]
        public Transform target;
        
        public AxisUtil.Axis axis;

        [SyncToStream] [MinMax(-180f, 180f, true, true)][CustomName("X轴约束")]
        public Vector2 xAxisRange;
        [SyncToStream] [MinMax(-180f, 180f, true, true)][CustomName("Y轴约束")]
        public Vector2 yAxisRange;
        [SyncToStream] [MinMax(-180f, 180f, true, true)][CustomName("Z轴约束")]
        public Vector2 zAxisRange;

        [SyncToStream][HideInInspector]
        public Vector3 targetPosition;

        [CustomName("自动回正")]
        public bool autoTweenBack;
        
        public override IAnimationJob CreateJob(Animator animator)
        {
            return new AimJob
            {
                weight = FloatProperty.Bind(animator, this, PropertyUtil.PropName(nameof(weight))),
                speed = FloatProperty.Bind(animator, this, PropertyUtil.PropName(nameof(speed))),
                joint = animator.BindStreamTransform(joint),
                offset = Vector3Property.Bind(animator, this, PropertyUtil.PropName(nameof(offset))),
                parent = animator.BindStreamTransform(joint.parent),
                targetPosition = Vector3Property.Bind(animator, this, PropertyUtil.PropName(nameof(targetPosition))),
                axis = AxisUtil.GetAxisVector(axis),
                
                xAxisRange = Vector2Property.Bind(animator, this, PropertyUtil.PropName(nameof(xAxisRange))),
                yAxisRange = Vector2Property.Bind(animator, this, PropertyUtil.PropName(nameof(yAxisRange))),
                zAxisRange = Vector2Property.Bind(animator, this, PropertyUtil.PropName(nameof(zAxisRange))),
                
                lastRotation = joint.rotation,

                autoTweenBack = autoTweenBack
            };
        }

        public override AnimationScriptPlayable CreatePlayable(PlayableGraph graph, IAnimationJob job)
        {
            return AnimationScriptPlayable.Create(graph, (AimJob)job);
        }

        public override void UpdateJob(Animator animator, IAnimationJob job)
        {
            if (target != null)
            {
                targetPosition = target.position;
            }
        }
    }
}