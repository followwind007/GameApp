using UnityEngine;
using UnityEngine.Playables;

namespace GameApp.Timeline
{
    public class AnimationPlayableMixer : PlayableBehaviour, IPlayableTarget
    {
        public GameObject Target { get; set; }

        private Transform _trans;
        private Vector3 _position;
        private Quaternion _rotation;
        private Vector3 _scale;

        public override void OnPlayableCreate(Playable playable)
        {
            if (Target)
            {
                _trans = Target.transform;
                _position = _trans.position;
                _rotation = _trans.rotation;
                _scale = _trans.localScale;
            }
        }

        public override void OnPlayableDestroy(Playable playable)
        {
            if (!Application.isPlaying && Target)
            {
                _trans.position = _position;
                _trans.rotation = _rotation;
                _trans.localScale = _scale;
            }
        }

    }

}