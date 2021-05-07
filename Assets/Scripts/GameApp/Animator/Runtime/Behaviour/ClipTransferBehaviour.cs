
using UnityEngine;
using UnityEngine.Playables;

namespace GameApp.AnimatorBehaviour
{
    public class ClipTransferBehaviour : AnimatorTransferBehaviour
    {
        public ClipTransfer Transfer => (ClipTransfer) transfer;
        public readonly ClipStateBehaviour fb;
        public readonly ClipStateBehaviour tb;
        
        private float _exitTime;
        private float _expectExitTime;
        private int _exitCount;
        private int _exitingCount;
        
        public override void OnEnter()
        {
            if (Transfer.hasExitTime && fb.Clip)
            {
                _expectExitTime = Mathf.Clamp(fb.Clip.length - Transfer.tweenDuration, 0, fb.Clip.length);
                _exitTime = (float)fb.ClipPlayable.GetTime() % fb.Clip.length;
                _exitCount = fb.LoopTimes;
                fb.OnTransferExit();
            }
            else
            {
                fb.TransferExitImmediate();
                tb.OnTransferEnter();
            }
        }

        public override void OnUpdate()
        {
            if (fb.StateStatus == ClipStateBehaviour.Status.OnTransferExit)
            {
                if (_exitTime <= _expectExitTime)
                {
                    if (fb.CurrentTime >= _expectExitTime)
                    {
                        InternalTransfer();
                    }
                }
                else
                {
                    if (fb.Clip && fb.Clip.isLooping)
                    {
                        if (fb.LoopTimes > _exitCount && fb.CurrentTime >= _expectExitTime)
                        {
                            InternalTransfer();
                        }
                    }
                    else
                    {
                        InternalTransfer();
                    }
                }
            }
            else if (fb.StateStatus == ClipStateBehaviour.Status.TransferExiting)
            {
                if (fb.Clip && fb.Clip.isLooping && fb.LoopTimes > _exitingCount)
                {
                    fb.TransferExitDone();
                }
                else if (fb.Clip && !fb.Clip.isLooping && fb.ClipPlayable.IsDone())
                {
                    fb.TransferExitDone();
                }
            }
        }

        public override void OnExit()
        {
            fb.OnTransferDirectExit();
        }

        public ClipTransferBehaviour(AnimatorRunner runner, AnimatorTransfer transfer) : base(runner, transfer)
        {
            fb = runner.GetBehaviour<ClipStateBehaviour>(transfer.from.StateName);
            tb = runner.GetBehaviour<ClipStateBehaviour>(transfer.to.StateName);
        }
        
        private void InternalTransfer()
        {
            fb.TansferExiting();
            _exitingCount = fb.LoopTimes;
            tb.OnTransferEnter();
        }
    }
}