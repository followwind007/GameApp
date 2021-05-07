using System;
using UnityEngine;
using UnityEngine.Playables;
using UnityEngine.Timeline;
using GameApp.ScenePlayable;

namespace GameApp.Timeline
{
    [Serializable]
    [TrackClipType(typeof(AnimationPlayableAsset))]
    [TrackColor(0f, 0.8f, 1f)]
    [TrackBindingType(typeof(GameObject))]
    public class AnimationPlayableTrack : TrackAsset
    {
        public SceneObjectFinder finder;

        public override Playable CreateTrackMixer(PlayableGraph graph, GameObject go, int inputCount)
        {
            var director = go.GetComponent<PlayableDirector>();
            var binding = director.GetGenericBinding(this) as GameObject;
            if (binding == null && finder != null)
                binding = finder.Target;

            if (binding)
            {
                foreach (var clip in GetClips())
                {
                    var asset = clip.asset as AnimationPlayableAsset;
                    asset.Target = binding;
                }
            }

            var mixer = new AnimationPlayableMixer
            {
                Target = binding
            };
            return ScriptPlayable<AnimationPlayableMixer>.Create(graph, mixer, inputCount);
        }
    }

}