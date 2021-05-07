using UnityEngine;
using UnityEngine.Playables;

namespace GameApp.Timeline
{
    [System.Serializable]
    public class ControlPlayableAsset : PlayableAsset, IPlayableAsset
    {
        public GameObject Target { get; set; }

        public ControlPlayableBehaviour template = new ControlPlayableBehaviour();

        public override Playable CreatePlayable(PlayableGraph graph, GameObject go)
        {
            template.Target = Target;
            return ScriptPlayable<ControlPlayableBehaviour>.Create(graph, template);
        }
    }

}