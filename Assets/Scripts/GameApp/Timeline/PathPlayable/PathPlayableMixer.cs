using UnityEngine;
using UnityEngine.Playables;

namespace GameApp.Timeline
{
    public class PathPlayableMixer : PlayableBehaviour
    {
        public Vector3 defaultPosition;
        public Quaternion defaultRotation;
        public Transform transform;

        public override void OnPlayableDestroy(Playable playable)
        {
            transform.position = defaultPosition;
            transform.rotation = defaultRotation;
        }

        public override void ProcessFrame(Playable playable, FrameData info, object playerData)
        {
            var inputCount = playable.GetInputCount();
            var position = Vector3.zero;
            var rotation = Vector3.zero;
            for (var i = 0; i < inputCount; i++)
            {
                var input = (ScriptPlayable<PathPlayableBehaviour>)playable.GetInput(i);
                var clone = input.GetBehaviour();
                
                var time = (float)(input.GetTime() / input.GetDuration());

                var weight = playable.GetInputWeight(i);
                
                position += clone.EvaluatePosition(time) * weight;
                rotation += clone.EvaluateRotation(time).eulerAngles * weight;
            }

            transform.position = position;
            transform.rotation = Quaternion.Euler(rotation);
        }

    }
}