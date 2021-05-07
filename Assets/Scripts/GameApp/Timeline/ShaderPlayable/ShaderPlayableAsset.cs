using UnityEngine;
using UnityEngine.Playables;

namespace GameApp.Timeline
{
    [System.Serializable]
    public class ShaderPlayableAsset : PlayableAsset, IPlayableAsset
    {
        public GameObject Target { get; set; }
        public ShaderPlayableBehavior template = new ShaderPlayableBehavior();

        public override Playable CreatePlayable(PlayableGraph graph, GameObject go)
        {
            template.Target = Target;

            return ScriptPlayable<ShaderPlayableBehavior>.Create(graph, template);
        }
    }
}
