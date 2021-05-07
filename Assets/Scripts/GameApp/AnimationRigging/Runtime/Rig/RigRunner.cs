using System.Collections.Generic;
using Tools.Table;
using UnityEngine;
using UnityEngine.Animations;
using UnityEngine.Experimental.Animations;

using UnityEngine.Playables;

namespace GameApp.AnimationRigging
{
    [RequireComponent(typeof(Animator))]
    public class RigRunner : MonoBehaviour
    {
        #if UNITY_2019_3_OR_NEWER
        private const ushort Priority = 1000;
        #endif
        
        public DirectorUpdateMode updateMode = DirectorUpdateMode.GameTime;

        public bool playOnAwake;
        
        [ReorderableItem]
        public List<RigLayer> layers;

        public PlayableGraph graph;

        private Animator _animator;

        [HideInInspector]
        public List<IRigLayer> allLayers = new List<IRigLayer>();
        
        private void OnEnable()
        {
            if (Application.isPlaying && playOnAwake)
            {
                Build();
            }
        }

        private void OnDisable()
        {
            if (Application.isPlaying)
            {
                Clear();
            }
        }

        private void OnDestroy()
        {
            Clear();
        }

        private void LateUpdate()
        {
            if (graph.IsValid())
            {
                foreach (var l in allLayers)
                {
                    l.UpdateConstraint(_animator);
                }
            }
        }

        public void Build()
        {
            Clear();

            _animator = GetComponent<Animator>();

            allLayers.Clear();
            allLayers.Add(new RigSyncLayer());
            allLayers.AddRange(layers);

            graph = CreateGraph(_animator);

            if (graph.IsValid())
            {
                graph.Play();
            }
        }

        public void Clear()
        {
            if (graph.IsValid())
            {
                graph.Destroy();
            }

            foreach (var l in allLayers)
            {
                l.Reset();
            }
        }

        private PlayableGraph CreateGraph(Animator animator)
        {
            var graphName = $"RigGraph_{animator.name}_{animator.GetInstanceID()}";
            var g = PlayableGraph.Create(graphName);
            g.SetTimeUpdateMode(updateMode);

            foreach (var l in layers)
            {
                l.Init(animator, g);
            }

            if (allLayers[0] is RigSyncLayer sl)
            {
                sl.InitSyncData(animator, g, layers);
            }

            foreach (var l in allLayers)
            {
                if (l.Items == null) continue;

                var last = l.Items[l.Items.Length - 1].playable;
                
                var output = AnimationPlayableOutput.Create(g, $"Out_{l}", animator);
                
                output.SetAnimationStreamSource(AnimationStreamSource.PreviousInputs);
                #if UNITY_2019_3_OR_NEWER
                output.SetSortingOrder(Priority);
                #endif
                output.SetSourcePlayable(last);
            }
            
            return g;
        }


    }
}