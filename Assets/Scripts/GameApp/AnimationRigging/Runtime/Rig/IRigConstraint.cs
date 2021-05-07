using UnityEngine;
#if UNITY_2019_3_OR_NEWER
using UnityEngine.Animations;
#else
using UnityEngine.Experimental.Animations;
#endif
using UnityEngine.Playables;

namespace GameApp.AnimationRigging
{
    public interface IRigConstraint
    {
        IAnimationJob CreateJob(Animator animator);

        void DestroyJob(IAnimationJob job);

        AnimationScriptPlayable CreatePlayable(PlayableGraph graph, IAnimationJob job);

        void UpdateJob(Animator animator, IAnimationJob job);
    }

}