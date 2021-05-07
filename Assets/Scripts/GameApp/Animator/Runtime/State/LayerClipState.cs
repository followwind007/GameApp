using UnityEngine;

namespace GameApp.AnimatorBehaviour
{
    public class LayerClipState : AnimatorState, IClipState
    {
        public AnimationClip clip;
        public AnimationClip Clip => clip;

        public AvatarMask mask;
        
        public float weight = 1f;
        
        public override AnimatorStateBehaviour CreateBehaviour(AnimatorRunner runner)
        {
            return new LayerClipStateBehaviour(runner, this, runner.GetOverride<LayerClipStateOverride>(StateName));
        }
    }
}