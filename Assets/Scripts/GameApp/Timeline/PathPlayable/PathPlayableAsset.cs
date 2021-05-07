using Cinemachine;
using UnityEngine;
using UnityEngine.Playables;

namespace GameApp.Timeline
{
    [System.Serializable]
    public class PathPlayableAsset : PlayableAsset
    {
        public PathPlayableBehaviour template = new PathPlayableBehaviour();
        public ExposedReference<CinemachinePath> path;

        public override Playable CreatePlayable(PlayableGraph graph, GameObject go)
        {
            var playable = ScriptPlayable<PathPlayableBehaviour>.Create(graph, template);
            var clone = playable.GetBehaviour();
            clone.path = path.Resolve(graph.GetResolver());
            return playable;
        }
    }

}