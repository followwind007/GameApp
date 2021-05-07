using GameApp.Util;
using UnityEngine;
using UnityEngine.Playables;

namespace GameApp.Timeline.TimePlayable
{
    public class TimePlayableBehaviour : PlayableBehaviour
    {
        public float timeScale = 1f;

        public override void OnBehaviourPlay(Playable playable, FrameData info)
        {
            var preScale = Time.timeScale;
            Timer.Add((float)playable.GetDuration(), () => { Time.timeScale = preScale; });
        }
        
    }
}