namespace GameApp.AnimatorBehaviour
{
    public class AnimatorTransferBehaviour
    {
        protected AnimatorRunner runner;
        protected AnimatorTransfer transfer;
        protected AnimatorTransferBehaviour(AnimatorRunner runner, AnimatorTransfer transfer)
        {
            this.runner = runner;
            this.transfer = transfer;
        }
        
        public virtual void OnEnter()
        {
        }

        public virtual void OnUpdate()
        {
        }

        public virtual void OnExit()
        {
        }
    }
}