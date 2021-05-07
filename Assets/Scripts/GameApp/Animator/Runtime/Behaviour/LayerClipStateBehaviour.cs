using UnityEngine;
using UnityEngine.Animations;
using UnityEngine.Playables;

namespace GameApp.AnimatorBehaviour
{
    public class LayerClipStateBehaviour : AnimatorStateBehaviour
    {
        public AnimationClip Clip => stateOverride ? ((LayerClipStateOverride) stateOverride).clip : ((LayerClipState) state).clip;

        public AvatarMask Mask => stateOverride ? ((LayerClipStateOverride) stateOverride).mask : ((LayerClipState) state).mask;

        public float Weight => stateOverride ? ((LayerClipStateOverride) stateOverride).weight : ((LayerClipState) state).weight;

        public AnimationClipPlayable ClipPlayable { get; private set; }

        public LayerClipStateBehaviour(AnimatorRunner runner, AnimatorState state, AnimatorStateOverride stateOverride = null) : 
            base(runner, state, stateOverride)
        {
        }

        public override void OnEnter()
        {
            if (ClipPlayable.IsNull())
            {
                ClipPlayable = AnimationClipPlayable.Create(runner.Graph, Clip);
            }
            runner.Handler.AttachLayerPlayable(ClipPlayable, Weight, Mask);
        }

        public override void OnExit()
        {
            if (!ClipPlayable.IsNull())
            {
                runner.Handler.DetachLayerPlayable(ClipPlayable);
            }
        }

        public override void OnUpdate()
        {
            if (ClipPlayable.IsNull() || ClipPlayable.IsDone())
            {
                runner.StopAddtive(state.StateName);
            }
        }
    }
}