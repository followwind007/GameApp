using UnityEngine;
using UnityEngine.InputSystem;

namespace GameApp.Moving
{
    public abstract class MoveMode
    {
        protected readonly MoveController controller;
        protected PlayerInfo Player => controller.Player;

        protected static readonly int StateMovement = Animator.StringToHash("Movement");
        protected static readonly int StateClimb = Animator.StringToHash("Climb");
        
        protected MoveMode(MoveController moveController)
        {
            controller = moveController;
        }

        public virtual void OnEnter()
        {
        }

        public virtual void OnExit()
        {
        }

        public virtual void OnAxisMove(InputAction.CallbackContext context)
        {
        }

        public virtual void OnAxisView(InputAction.CallbackContext context)
        {
        }

        public virtual void OnJump(InputAction.CallbackContext context)
        {
        }

        public virtual void Tick()
        {
            
        }
    }
}