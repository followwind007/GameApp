using UnityEngine;
using UnityEngine.InputSystem;

namespace GameApp.Moving
{
    public class ClimbMode : MoveMode
    {
        private Vector2 _axis;
        
        public ClimbMode(MoveController moveController) : base(moveController)
        {
        }

        public override void OnEnter()
        {
            controller.Body.isKinematic = true;
        }

        public override void OnAxisMove(InputAction.CallbackContext context)
        {
            _axis = context.ReadValue<Vector2>();
        }

        public override void Tick()
        {
            if (_axis == Vector2.zero)
            {
                return;
            }

            var trans = Player.transform;

            var dir = trans.up * _axis.y + trans.right * _axis.x;
            
            Debug.DrawRay(Player.neck.position, dir, Color.blue);
        }
    }
}