using UnityEngine;
using UnityEngine.Playables;

namespace GameApp.Timeline
{
    [System.Serializable]
    public class AnimatorPlayableAsset : PlayableAsset, IPlayableAsset
    {
        public GameObject Target { get; set; }

        public AnimatorPlayableBehaviour template = new AnimatorPlayableBehaviour();

        public override Playable CreatePlayable(PlayableGraph graph, GameObject go)
        {
            template.Target = Target;
            return ScriptPlayable<AnimatorPlayableBehaviour>.Create(graph, template);
        }
    }
}
