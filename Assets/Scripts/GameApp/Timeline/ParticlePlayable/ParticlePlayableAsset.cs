using UnityEngine;
using UnityEngine.Playables;

namespace GameApp.Timeline
{
    [System.Serializable]
    public class ParticlePlayableAsset : PlayableAsset, IPlayableTarget
    {
        public GameObject Target { get; set; }

        public ParticlePlayableBehaviour template = new ParticlePlayableBehaviour();

        public override Playable CreatePlayable(PlayableGraph graph, GameObject go)
        {
            template.Target = Target;
            return ScriptPlayable<ParticlePlayableBehaviour>.Create(graph, template);
        }
    }

}