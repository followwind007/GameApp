using UnityEngine;

namespace GameApp.AnimatorBehaviour
{
    [StateOverrideType(typeof(LayerClipState))]
    public class LayerClipStateOverride : AnimatorStateOverride, IClipState
    {
        public float weight;
        public AnimationClip clip;
        public AnimationClip Clip => clip;
        public AvatarMask mask;
    }
}