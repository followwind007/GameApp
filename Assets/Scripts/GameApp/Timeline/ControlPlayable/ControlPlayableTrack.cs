using System;
using UnityEngine;
using UnityEngine.Playables;
using UnityEngine.Timeline;

namespace GameApp.Timeline
{
    [Serializable]
    [TrackClipType(typeof(ControlPlayableAsset))]
    [TrackColor(0.92f, 0.25f, 0f)]
    [TrackBindingType(typeof(GameObject))]
    public class ControlPlayableTrack : TrackAsset
    {
        public override Playable CreateTrackMixer(PlayableGraph graph, GameObject go, int inputCount)
        {
            var director = go.GetComponent<PlayableDirector>();
            var binding = director.GetGenericBinding(this) as GameObject;
            if (binding)
            {
                foreach (var clip in GetClips())
                {
                    var asset = clip.asset as ControlPlayableAsset;
                    asset.Target = binding;
                }
            }
            var mixer = new ControlPlayableMixer
            {
                Target = binding
            };
            return ScriptPlayable<ControlPlayableMixer>.Create(graph, mixer, inputCount);
        }
    }

}