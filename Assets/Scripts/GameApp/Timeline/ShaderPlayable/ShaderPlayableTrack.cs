using System;
using UnityEngine;
using UnityEngine.Playables;
using UnityEngine.Timeline;

namespace GameApp.Timeline
{
    [Serializable]
    [TrackColor(0.55f, 0.45f, 0.35f)]
    [TrackClipType(typeof(ShaderPlayableAsset))]
    [TrackBindingType(typeof(GameObject))]
    public class ShaderPlayableTrack : TrackAsset
    {
        public override Playable CreateTrackMixer(PlayableGraph graph, GameObject go, int inputCount)
        {
            var director = go.GetComponent<PlayableDirector>();
            var binding = director.GetGenericBinding(this) as GameObject;
            if (binding)
            {
                foreach (var clip in GetClips())
                {
                    var asset = clip.asset as ShaderPlayableAsset;
                    asset.Target = binding;
                }
            }
            var mixer = new ShaderPlayableMixer
            {
                Target = binding
            };
            return ScriptPlayable<ShaderPlayableMixer>.Create(graph, mixer, inputCount);
        }

    }

}