using UnityEngine;
using UnityEngine.Playables;

namespace GameApp.AnimationRigging
{
    public interface IRigLayer
    {
        IRigConstraint[] Constraints { get; }
        RigRuntimeItem[] Items { get; }
        void Init(Animator animator, PlayableGraph graph);
        void Reset();
        void UpdateConstraint(Animator animator);
    }

    public static class RigLayerImpl
    {
        public static void ResetImpl(this IRigLayer target)
        {
            if (target.Constraints != null)
            {
                for (var i = 0; i < target.Constraints.Length; i++)
                {
                    var c = target.Constraints[i];
                    var item = target.Items[i];
                    c.DestroyJob(item.job);
                }
            }
        }

        public static void UpdateConstraintImpl(this IRigLayer target, Animator animator)
        {
            if (target.Constraints != null)
            {
                for (var i = 0; i < target.Constraints.Length; i++)
                {
                    var c = target.Constraints[i];
                    var item = target.Items[i];
                    c.UpdateJob(animator, item.job);
                }
            }
        }

    }
    
}