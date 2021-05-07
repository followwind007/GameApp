using System;
using UnityEngine;
using UnityEngine.Playables;
using UnityEngine.Timeline;

namespace GameApp.Timeline
{
    [Serializable]
    [TrackColor(0.8f, 1f, 0.25f)]
    [TrackClipType(typeof(ParticlePlayableAsset))]
    [TrackBindingType(typeof(GameObject))]
    public class ParticlePlayableTrack : TrackAsset
    {
        public override Playable CreateTrackMixer(PlayableGraph graph, GameObject go, int inputCount)
        {
            var director = go.GetComponent<PlayableDirector>();
            var binding = director.GetGenericBinding(this) as GameObject;
            foreach (var clip in GetClips())
            {
                var asset = clip.asset as ParticlePlayableAsset;
                asset.Target = binding;
            }
            return base.CreateTrackMixer(graph, go, inputCount);
        }

    }

}