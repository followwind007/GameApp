using UnityEngine;

namespace GameApp.AnimatorBehaviour
{
    [StateOverrideType(typeof(ClipState))]
    public class ClipStateOverride : AnimatorStateOverride, IClipState
    {
        public AnimationClip clip;

        public AnimationClip Clip => clip;
    }
}