using UnityEngine;
#if UNITY_2019_3_OR_NEWER
using UnityEngine.Animations;
#else
using UnityEngine.Experimental.Animations;
#endif
using UnityEngine.Playables;

namespace GameApp.AnimationRigging
{
    public struct RigRuntimeItem
    {
        public IAnimationJob job;
        public AnimationScriptPlayable playable;
    }
    
    public class RigLayer : MonoBehaviour, IRigLayer
    {
        [Range(0f, 1f)]
        public float weight = 1f;

        public float Weight => Mathf.Clamp01(weight);

        public IRigConstraint[] Constraints => _constraints;
        public RigRuntimeItem[] Items => _items;
        
        private IRigConstraint[] _constraints;
        private RigRuntimeItem[] _items;
        
        private RigRunner _runner;

        public RigRunner Runner
        {
            get
            {
                if (_runner == null)
                {
                    _runner = GetComponentInParent<RigRunner>();
                    if (_runner == null)
                    {
                        Debug.LogError($"Can not find [RigRunner] in parent of {gameObject.name}");
                    }
                }

                return _runner;
            }
        }

        public void Init(Animator animator, PlayableGraph graph)
        {
            _constraints = GetComponentsInChildren<IRigConstraint>();
            _items = null;
            if (_constraints != null)
            {
                _items = new RigRuntimeItem[_constraints.Length];
                for (var i = 0; i < _constraints.Length; i++)
                {
                    var c = _constraints[i];
                    
                    var job = c.CreateJob(animator);
                    var playable = c.CreatePlayable(graph, job);

                    if (i > 0)
                    {
                        var prev = _items[i - 1].playable;
                        playable.AddInput(prev, 0, 1);
                    }

                    var item = new RigRuntimeItem {job = job, playable = playable};

                    _items[i] = item;
                }
            }
        }

        public void Reset()
        {
            this.ResetImpl();
            _constraints = null;
            _items = null;
        }

        public void UpdateConstraint(Animator animator)
        {
            this.UpdateConstraintImpl(animator);
        }

        public override string ToString()
        {
            return $"RigLayer_{name}";
        }
    }
}