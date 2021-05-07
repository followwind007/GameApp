using System.Collections.Generic;
using Cinemachine;
using UnityEngine;
using UnityEngine.InputSystem;

namespace GameApp.Moving
{
    [RequireComponent(typeof(Rigidbody), typeof(Animator), typeof(EnvironmentSensor))]
    public class MoveController : MonoBehaviour
    {
        public enum Mode
        {
            Walk,
            Climb,
            Swim,
            Dive,
        }
        
        [SerializeField] private Rigidbody body;
        public Rigidbody Body => body;

        [SerializeField] private Animator anim;
        public Animator Anim => anim;

        [SerializeField] private EnvironmentSensor sensor;
        public EnvironmentSensor Sensor => sensor;
        
        [SerializeField] private PlayerInfo player;
        public PlayerInfo Player => player;

        public Transform CamTrans => GetCamTrans();
        private Transform _camTrans;

        public CinemachineFreeLook freeLook;

        public Mode mode = Mode.Walk;

        private Dictionary<Mode, MoveMode> _modes;

        private MoveMode CurMode => _modes[mode];

        private void Awake()
        {
            _modes = new Dictionary<Mode, MoveMode>
            {
                {Mode.Walk, new WalkMode(this)},
                {Mode.Climb, new ClimbMode(this)}
            };
        }

        public void OnJump(InputAction.CallbackContext context)
        {
            CurMode.OnJump(context);
        }

        public void OnAxisMove(InputAction.CallbackContext context)
        {
            CurMode.OnAxisMove(context);
        }

        public void OnAxisView(InputAction.CallbackContext context)
        {
            CurMode.OnAxisView(context);
        }

        public Transform GetCamTrans()
        {
            if (_camTrans == null)
            {
                var bCount = CinemachineCore.Instance.BrainCount;
                for (var i = 0; i < bCount; i++)
                {
                    var b = CinemachineCore.Instance.GetActiveBrain(i);
                    if (b.ActiveVirtualCamera as CinemachineFreeLook == freeLook)
                    {
                        _camTrans = b.transform;
                        break;
                    }
                }
            }
            
            return _camTrans;
        }

        public void TransferToMode(Mode nextMode)
        {
            if (nextMode == mode) return;
            
            CurMode.OnExit();
            mode = nextMode;
            CurMode.OnExit();
        }

        private void Update()
        {
            CurMode.Tick();
        }

        #if UNITY_EDITOR
        private void OnDrawGizmos()
        {
            
        }
        #endif
    }
}