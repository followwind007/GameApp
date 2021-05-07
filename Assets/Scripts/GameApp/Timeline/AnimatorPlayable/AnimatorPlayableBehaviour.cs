using UnityEngine;
using UnityEngine.Playables;
using System.Collections.Generic;
using GameApp.ScenePlayable;

namespace GameApp.Timeline
{
    [System.Serializable]
    public class AnimatorPlayableBehaviour : PlayableBehaviour, IPlayableTarget
    {
        public GameObject Target { get; set; }

        [Header("运行时Animator路径")]
        [PathRef(typeof(RuntimeAnimatorController))]
        public string animatorPath;

        [Header("状态机操作队列")]
        public List<AnimatorPlayableCommand> commandList;

        public override void OnBehaviourPlay(Playable playable, FrameData info)
        {
            if (!Application.isPlaying) return;
            DoCommands();
        }

        private void DoCommands()
        {
            if (Target == null || commandList == null) return;
            var animator = Target.GetComponent<Animator>();
            if (animator == null) animator = Target.AddComponent<Animator>();
            if (animator.runtimeAnimatorController == null)
            {
                var aniCtrl = PlayableLoader.LoadAssetAtPath<RuntimeAnimatorController>(animatorPath);
                animator.runtimeAnimatorController = aniCtrl;
            }
            if (animator != null)
            {
                foreach (AnimatorPlayableCommand command in commandList)
                    command.DoCommand(animator);
            }
        }

    }

}