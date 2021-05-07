using UnityEngine;
using UnityEngine.Playables;
using GameApp.ScenePlayable;

namespace GameApp.Timeline
{
    [System.Serializable]
    public class AnimationPlayableAsset : PlayableAsset, IPlayableAsset
    {
        public GameObject Target { get; set; }

        public AnimationPlayableBehaviour template = new AnimationPlayableBehaviour();

        public override Playable CreatePlayable(PlayableGraph graph, GameObject go)
        {
            template.Target = Target;
            return ScriptPlayable<AnimationPlayableBehaviour>.Create(graph, template);
        }
    }

}