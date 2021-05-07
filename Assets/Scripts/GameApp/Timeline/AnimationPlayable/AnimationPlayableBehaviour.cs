using UnityEngine;
using UnityEngine.Playables;
using GameApp.ScenePlayable;

namespace GameApp.Timeline
{
    [System.Serializable]
    public class AnimationPlayableBehaviour : PlayableBehaviour, IPlayableTarget
    {
        public GameObject Target { get; set; }

        [Header("Timeline停止, 停止循环动画")]
        public bool stopLoopWhenStop = false;

        [Header("Animation Clip路径")]
        [PathRef(typeof(AnimationClip))]
        public string clipPath;

        private Animation _animation;

        private AnimationClip _clip;

        public override void OnBehaviourPlay(Playable playable, FrameData info)
        {
            _clip = PlayableLoader.LoadAssetAtPath<AnimationClip>(clipPath);
            if (!Application.isPlaying) return;
            PlayAnimation();
        }

        public override void ProcessFrame(Playable playable, FrameData info, object playerData)
        {
            if (Application.isPlaying || _clip == null || Target == null) return;
            _clip.SampleAnimation(Target, (float)playable.GetTime());
        }

        public override void OnBehaviourPause(Playable playable, FrameData info)
        {
            if (!Application.isPlaying) return;
            StopAnimation();
        }

        private void PlayAnimation()
        {
            if (Target == null || _clip == null) return;
            if (_clip.legacy != true)
            {
                _clip.legacy = true;
                Debug.Log("animation clip:" + _clip.name + "is not set as legacy, will change it");
            }

            _animation = Target.GetOrAddComponent<Animation>();
            if (_animation.GetClip(_clip.name) == null)
            {
                _animation.AddClip(_clip, _clip.name);
            }
            _animation.Play(_clip.name);
        }

        private void StopAnimation()
        {
            if (_animation == null || _clip == null) return;
            
            if (_clip.wrapMode == WrapMode.Loop && stopLoopWhenStop || _clip.wrapMode != WrapMode.Loop)
            {
                _animation.Stop(_clip.name);
                if (_animation.GetClip(_clip.name) == _clip)
                {
                    _animation.RemoveClip(_clip);
                }
                _clip = null;
            }
            
        }


    }
}

