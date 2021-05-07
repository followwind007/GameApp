using UnityEngine;
using UnityEngine.Playables;

namespace GameApp.Timeline.TimePlayable
{
    public class TimePlayableAsset : PlayableAsset
    {
        public TimePlayableBehaviour template = new TimePlayableBehaviour();
        
        public override Playable CreatePlayable(PlayableGraph graph, GameObject owner)
        {
            var playable = ScriptPlayable<TimePlayableBehaviour>.Create(graph);

            return playable;
        }
    }
}