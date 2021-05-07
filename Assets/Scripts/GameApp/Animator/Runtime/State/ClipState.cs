using UnityEngine;

namespace GameApp.AnimatorBehaviour
{
    [StateColor("#3e4145", "#65c294", "#65c294")]
    public class ClipState : AnimatorState, IClipState
    {
        public AnimationClip clip;
        public AnimationClip Clip => clip;

        public override AnimatorStateBehaviour CreateBehaviour(AnimatorRunner runner)
        {
            return new ClipStateBehaviour(runner, this, runner.GetOverride<AnimatorStateOverride>(StateName));
        }
    }
}