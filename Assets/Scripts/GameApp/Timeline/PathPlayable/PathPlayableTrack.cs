
using UnityEngine;
using UnityEngine.Playables;
using UnityEngine.Timeline;

namespace GameApp.Timeline
{
    [TrackColor(0f, 0f, 1f)]
    [TrackBindingType(typeof(Transform))]
    [TrackClipType(typeof(PathPlayableAsset))]
    public class PathPlayableTrack : TrackAsset
    {
        public override Playable CreateTrackMixer(PlayableGraph graph, GameObject go, int inputCount)
        {
            var mixer = ScriptPlayable<PathPlayableMixer>.Create(graph, inputCount);
            var clone = mixer.GetBehaviour();
            var binding = go.GetComponent<PlayableDirector>().GetGenericBinding(this) as Transform;
            if (binding != null)
            {
                clone.transform = binding;
                clone.defaultPosition = binding.position;
                clone.defaultRotation = binding.rotation;
            }
            
            return mixer;
        }
        
        public override void GatherProperties(PlayableDirector director, IPropertyCollector driver)
        {
#if UNITY_EDITOR
            var comp = director.GetGenericBinding(this) as Transform;
            if (comp == null)
                return;
            var so = new UnityEditor.SerializedObject(comp);
            var iter = so.GetIterator();
            while (iter.NextVisible(true))
            {
                if (iter.hasVisibleChildren)
                    continue;
                driver.AddFromName<Transform>(comp.gameObject, iter.propertyPath);
            }
#endif
            base.GatherProperties(director, driver);
        }

    }

}