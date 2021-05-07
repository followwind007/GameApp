using UnityEngine;
using UnityEngine.InputSystem;

namespace GameApp.Moving
{
    public class WalkMode : MoveMode
    {
        public WalkMode(MoveController moveController) : base(moveController) { }

        private Vector2 _axis;
        
        public override void OnAxisMove(InputAction.CallbackContext context)
        {
            _axis = context.ReadValue<Vector2>();
        }

        public override void OnJump(InputAction.CallbackContext context)
        {
            if (!context.performed) return;
            foreach (var holder in controller.Sensor.CurHolders)
            {
                var res = holder.GetCrossPoint(Player.neck.position, Vector3.up * 5, out var cross);
                if (res)
                {
                    #if UNITY_EDITOR
                    Debug.DrawLine(Player.neck.position, cross, Color.magenta, 2);
                    #endif
                    controller.Anim.SetBool(StateClimb, true);
                    controller.TransferToMode(MoveController.Mode.Climb);
                    break;
                }
            }
        }

        public override void OnEnter()
        {
            controller.Body.isKinematic = false;
        }

        public override void Tick()
        {
            if (_axis.magnitude > 0)
            {
                var trans = controller.transform;

                var strength = _axis.magnitude * Player.runSpeed;
                controller.Body.AddForce(trans.forward * strength);
                
                //rotate
                var camTrans = controller.CamTrans;
                
                var forward = camTrans.TransformDirection(Vector3.forward);
                var right = camTrans.TransformDirection(Vector3.right);
                
                var targetDir = _axis.x * right + _axis.y * forward;
                targetDir.y = 0;

                var targetRot = Quaternion.LookRotation(targetDir, trans.up);
                var rotT = Player.rotateSpeed * Time.deltaTime;
                
                trans.rotation = Quaternion.Slerp(trans.rotation, targetRot, rotT);
            }

            controller.Anim.SetFloat(StateMovement, controller.Body.velocity.magnitude);
        }

    }
}