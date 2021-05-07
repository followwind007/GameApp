
namespace GameApp.AnimatorBehaviour
{
    [TransforCompatible(typeof(ClipState), typeof(ClipState))]
    public class ClipTransfer : AnimatorTransfer
    {
        public const int ExitStateInputIndex = 1;
        public const int EnterStateInputIndex = 0;
        public const int OutputIndex = 0;

        public float tweenDuration;
        public bool hasExitTime;

        public override AnimatorTransferBehaviour CreateBehaviour(AnimatorRunner runner)
        {
            return new ClipTransferBehaviour(runner, this);
        }
    }
}