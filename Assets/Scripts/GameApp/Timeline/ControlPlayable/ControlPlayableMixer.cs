using UnityEngine;
using UnityEngine.Playables;

namespace GameApp.Timeline
{
    public class ControlPlayableMixer : PlayableBehaviour, IPlayableTarget
    {
        private bool _activeSelf;
        private Vector3 _localScale;

        public GameObject Target { get; set; }
       
        public override void OnPlayableCreate(Playable playable)
        {
            if (Target)
            {
                _activeSelf = Target.activeSelf;
                _localScale = Target.transform.localScale;
            }
        }

        public override void OnPlayableDestroy(Playable playable)
        {
            if (!Application.isPlaying && Target)
            {
                Target.SetActive(_activeSelf);
                Target.transform.localScale = _localScale;
            }
        }

    }

}