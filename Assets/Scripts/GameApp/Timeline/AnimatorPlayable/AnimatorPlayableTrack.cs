using System;
using UnityEngine;
using UnityEngine.Playables;
using UnityEngine.Timeline;

namespace GameApp.Timeline
{
    [Serializable]
    [TrackClipType(typeof(AnimatorPlayableAsset))]
    [TrackColor(0.4f, 0.45f, 0.9f)]
    [TrackBindingType(typeof(GameObject))]
    public class AnimatorPlayableTrack : TrackAsset
    {
        public override Playable CreateTrackMixer(PlayableGraph graph, GameObject go, int inputCount)
        {
            var director = go.GetComponent<PlayableDirector>();
            var binding = director.GetGenericBinding(this) as GameObject;
            if (binding)
            {
                foreach (var clip in GetClips())
                {
                    var asset = clip.asset as AnimatorPlayableAsset;
                    asset.Target = binding;
                }
            }
            return base.CreateTrackMixer(graph, go, inputCount);
        }
    }

}