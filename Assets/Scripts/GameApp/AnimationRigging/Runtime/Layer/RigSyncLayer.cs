using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Playables;

namespace GameApp.AnimationRigging
{
    public class RigSyncLayer : IRigLayer
    {
        public IRigConstraint[] Constraints => _constraints;
        public RigRuntimeItem[] Items => _items;
        
        private IRigConstraint[] _constraints;
        private RigRuntimeItem[] _items;
        
        public void Init(Animator animator, PlayableGraph graph)
        {
            
        }

        public void InitSyncData(Animator animator, PlayableGraph graph, List<RigLayer> layers)
        {
            var c = new RigSyncConstraint();
            _constraints = new IRigConstraint[] {c};
            
            var job = c.CreateJob(animator, layers);
            var playable = c.CreatePlayable(graph, job);
            
            var item = new RigRuntimeItem {job = job, playable = playable};
            
            _items = new[] {item};
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
            return "RigSyncLayer";
        }
    }
}