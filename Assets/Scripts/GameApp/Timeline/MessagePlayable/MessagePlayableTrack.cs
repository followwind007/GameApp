using System;
using UnityEngine;
using UnityEngine.Playables;
using UnityEngine.Timeline;

namespace GameApp.Timeline
{
    [Serializable]
    [TrackClipType(typeof(MessagePlayableAsset))]
    [TrackColor(0.96f, 0.5f, 0.5f)]
    public class MessagePlayableTrack : TrackAsset
    {
        public override Playable CreateTrackMixer(PlayableGraph graph, GameObject go, int inputCount)
        {
            var mixer = new MessagePlayableMixer();
            return ScriptPlayable<MessagePlayableMixer>.Create(graph, mixer, inputCount);
        }
    }

}