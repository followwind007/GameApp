namespace GameApp.AnimatorBehaviour
{
    public class AnimatorStateBehaviour
    {
        protected readonly AnimatorState state;
        protected readonly AnimatorRunner runner;
        protected readonly AnimatorStateOverride stateOverride;

        protected AnimatorStateBehaviour(AnimatorRunner runner, AnimatorState state, AnimatorStateOverride stateOverride = null)
        {
            this.state = state;
            this.runner = runner;
            this.stateOverride = stateOverride;
        }
        
        /// <summary>
        /// logical enter, called by AnimatorRunner
        /// </summary>
        public virtual void OnEnter()
        {
        }

        /// <summary>
        /// logical exit, called by AnimatorRunner
        /// </summary>
        public virtual void OnExit()
        {
        }

        public virtual void OnUpdate()
        {
        }

        public virtual void OnStop()
        {
        }
    }
}