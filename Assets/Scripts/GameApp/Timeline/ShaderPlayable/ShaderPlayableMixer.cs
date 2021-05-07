using UnityEngine;
using UnityEngine.Playables;
using System.Collections.Generic;

namespace GameApp.Timeline
{
    public class ShaderPlayableMixer : PlayableBehaviour, IPlayableTarget
    {
        private Dictionary<Renderer, Material[]> _rdDict;

        public GameObject Target { get; set; }

        public override void OnPlayableCreate(Playable playable)
        {
            if (Target)
            {
                _rdDict = new Dictionary<Renderer, Material[]>();
                
                var rds = Target.GetComponents<Renderer>();
                foreach (var rd in rds)
                {
                    _rdDict[rd] = rd.sharedMaterials;
                }
            }
        }

        public override void OnPlayableDestroy(Playable playable)
        {
            if (!Application.isPlaying && Target)
            {
                var rds = Target.GetComponents<Renderer>();
                foreach (var rd in rds)
                {
                    if (_rdDict.ContainsKey(rd))
                    {
                        rd.sharedMaterials = _rdDict[rd];
                    }
                }
            }
        }

    }

}