using UnityEngine;
using UnityEngine.Animations;
using UnityEngine.Playables;

namespace GameApp.AnimatorBehaviour
{
    public class ClipStateBehaviour : AnimatorStateBehaviour
    {
        public enum Status
        {
            Default, OnEnter, OnTransferEnter, OnExit, OnTransferExit, TransferExiting, TransferExitDone, TransferImmediate      
        }

        public AnimationClip Clip
        {
            get
            {
                if (stateOverride && stateOverride is ClipStateOverride co)
                {
                    return co.clip;
                }

                return ((ClipState) state).clip;
            }
        }

        public int LoopTimes => Clip != null ? Mathf.CeilToInt((float)ClipPlayable.GetTime() / Clip.length) : -1;

        public AnimationMixerPlayable Mixer => runner.Handler.mixer;

        public double CurrentTime => ClipPlayable.IsNull() ? -1 : ClipPlayable.GetTime() % Clip.length;

        public AnimationClipPlayable ClipPlayable { get; private set; }

        public Status StateStatus { get; private set; }

        public ClipStateBehaviour(AnimatorRunner runner, AnimatorState state, AnimatorStateOverride stateOverride = null) : 
            base(runner, state, stateOverride)
        {
        }
        
        public override void OnEnter()
        {
            StateStatus = Status.OnEnter;
            CreatePlayable();
            
            Mixer.DisconnectInput(ClipTransfer.EnterStateInputIndex);
            Mixer.DisconnectInput(ClipTransfer.ExitStateInputIndex);
            Mixer.ConnectInput(ClipTransfer.EnterStateInputIndex, ClipPlayable, ClipTransfer.OutputIndex);
            Mixer.SetInputWeight(ClipPlayable, 1);
            ClipPlayable.SetTime(0);
            ClipPlayable.SetDone(false);
        }

        public void OnTransferEnter()
        {
            StateStatus = Status.OnTransferEnter;
            CreatePlayable();

            Mixer.DisconnectInput(ClipTransfer.EnterStateInputIndex);
            Mixer.ConnectInput(ClipTransfer.EnterStateInputIndex, ClipPlayable, ClipTransfer.OutputIndex);
            Mixer.SetInputWeight(ClipTransfer.EnterStateInputIndex, 1);
            ClipPlayable.SetTime(0);
        }
        
        public void OnTransferExit()
        {
            StateStatus = Status.OnTransferExit;
        }

        public void OnTransferDirectExit()
        {
            if (StateStatus == Status.OnTransferExit || StateStatus == Status.TransferExiting)
            {
                Mixer.DisconnectInput(ClipTransfer.EnterStateInputIndex);
                Mixer.DisconnectInput(ClipTransfer.ExitStateInputIndex);
            }
        }

        public override void OnExit()
        {
            StateStatus = Status.OnExit;
            if (ClipPlayable.IsNull()) return;
            
            Mixer.DisconnectInput(ClipTransfer.EnterStateInputIndex);
        }

        public override void OnUpdate()
        {
            if (StateStatus == Status.OnEnter || StateStatus == Status.OnTransferEnter)
            {
                if (!Clip || Clip.isLooping) return;
                
                if (ClipPlayable.IsDone())
                {
                    runner.TryTransfer();
                }
                else
                {
                    foreach (var t in state.transfers)
                    {
                        if (!runner.IsTransferValid(t)) continue;
                        if (t is ClipTransfer ct && ct.hasExitTime && 
                            ct.tweenDuration <= ClipPlayable.GetTime())
                        {
                            runner.Transfer(t);
                        }
                    }
                }
            }
        }

        public override void OnStop()
        {
            if (ClipPlayable.IsNull())
            {
                ClipPlayable.Destroy();
            }
        }

        public void TansferExiting()
        {
            StateStatus = Status.TransferExiting;

            if (ClipPlayable.IsNull()) return;
            
            Mixer.DisconnectInput(ClipTransfer.EnterStateInputIndex);

            Mixer.DisconnectInput(ClipTransfer.ExitStateInputIndex);
            Mixer.ConnectInput(ClipTransfer.ExitStateInputIndex, ClipPlayable, ClipTransfer.OutputIndex);
            Mixer.SetInputWeight(ClipTransfer.ExitStateInputIndex, 1);
            ClipPlayable.SetTime(ClipPlayable.GetTime());
        }

        public void TransferExitDone()
        {
            StateStatus = Status.TransferExitDone;
            if (ClipPlayable.IsNull()) return;
            Mixer.DisconnectInput(ClipTransfer.ExitStateInputIndex);
            Mixer.SetInputWeight(ClipTransfer.ExitStateInputIndex, 0);
            ClipPlayable.SetTime(0);
        }

        public void TransferExitImmediate()
        {
            StateStatus = Status.TransferImmediate;
            Mixer.DisconnectInput(ClipTransfer.EnterStateInputIndex);
            ClipPlayable.SetTime(0);
        }

        private void CreatePlayable()
        {
            if (ClipPlayable.IsNull())
            {
                ClipPlayable = AnimationClipPlayable.Create(runner.Graph, Clip);
                if (Clip && !Clip.isLooping) ClipPlayable.SetDuration(Clip.length);
            }
        }

    }
}