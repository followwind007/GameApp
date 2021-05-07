using UnityEngine;
using UnityEngine.Animations;
using UnityEngine.Playables;

namespace GameApp.AnimatorBehaviour
{
    public class AnimationHandler
    {
        public readonly AnimationPlayableOutput output;
        public readonly AnimationMixerPlayable mixer;

        public AnimationLayerMixerPlayable layerMixer;

        public AnimationClipPlayable input0;
        public AnimationClipPlayable input1;
        
        public AnimationHandler(PlayableGraph graph, Animator animator)
        {
            output = AnimationPlayableOutput.Create(graph, "Animation Output", animator);
            
            layerMixer = AnimationLayerMixerPlayable.Create(graph, 1);
            output.SetSourcePlayable(layerMixer);

            mixer = AnimationMixerPlayable.Create(graph, 2);
            layerMixer.ConnectInput(0, mixer, 0, 1);
            
            input0 = AnimationClipPlayable.Create(graph, null);
            input1 = AnimationClipPlayable.Create(graph, null);
            
            mixer.ConnectInput(0, input0, 0);
            mixer.ConnectInput(1, input1, 0);
            
            mixer.SetInputWeight(0, 1);
            mixer.SetInputWeight(0, 1);
        }

        public void AttachLayerPlayable(Playable playable, float weight, AvatarMask mask)
        {
            var count = layerMixer.GetInputCount();
            layerMixer.SetInputCount(count + 1);
            layerMixer.SetLayerMaskFromAvatarMask((uint)count, mask);
            layerMixer.ConnectInput(count, playable, 0, weight);
        }

        public void DetachLayerPlayable(Playable playable)
        {
            var count = layerMixer.GetInputCount();
            for (var i = 0; i < count; i++)
            {
                var p = layerMixer.GetInput(i);
                if (p.Equals(playable))
                {
                    layerMixer.DisconnectInput(i);
                    return;
                }
            }
        }

    }
}