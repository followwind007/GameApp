using UnityEngine;
using UnityEngine.Animations;
using UnityEngine.Playables;

namespace GameApp.AnimationRigging
{
    public abstract class RigConstraintBehaviour : MonoBehaviour, IRigConstraint
    {
        private RigLayer _layer;

        protected RigLayer Layer
        {
            get
            {
                if (_layer == null)
                {
                    _layer = GetComponentInParent<RigLayer>();
                    if (_layer == null)
                    {
                        Debug.LogError($"Can not find [RigLayer] in parent of {gameObject.name}");
                    }
                }

                return _layer;
            }
        }
        
        public abstract IAnimationJob CreateJob(Animator animator);

        public virtual void DestroyJob(IAnimationJob job)
        {
            
        }

        public abstract AnimationScriptPlayable CreatePlayable(PlayableGraph graph, IAnimationJob job);

        public virtual void UpdateJob(Animator animator, IAnimationJob job)
        {
            
        }

        #if UNITY_EDITOR
        private void OnValidate()
        {
            //rebuild graph when change property
            if (gameObject.activeInHierarchy && Layer.Runner.graph.IsValid())
            {
                Layer.Runner.Build();
            }
        }
        #endif
    }
}