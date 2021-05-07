using UnityEngine;

namespace Controller
{
    [RequireComponent(typeof(Rigidbody))]
    [RequireComponent(typeof(HandleAnim))]
    public class StateManager : MonoBehaviour
    {
        public float horizontal;
        public float vertical;
        public bool dummy;
        public bool onGround = true;

        [HideInInspector]
        public HandleAnim hAnim;
        [HideInInspector]
        public HandleMovement hMovement;

        private static readonly int _onAir = Animator.StringToHash("onAir");

        private void Start()
        {
            hAnim = GetComponent<HandleAnim>();
            hMovement = GetComponent<HandleMovement>();

            hAnim.Init(this);
            hMovement.Init();
        }

        private void Update()
        {
            if (!dummy)
            {
                hAnim.Tick();
                hMovement.Tick();
                IsOnGround();              
            }
        }

        public void EnableController()
        {
            dummy = false;
            hMovement.rb.isKinematic = false;
            GetComponent<Collider>().isTrigger = false;
        }

        public void DisableController()
        {
            dummy = true;
            hMovement.rb.isKinematic = true;
            GetComponent<Collider>().isTrigger = true;
        }

        private void IsOnGround()
        {
            onGround = OnGround();

            if(onGround)
            {
                hAnim.anim.SetBool(_onAir, false);
                hMovement.rb.drag = 4;
            }
            else
            {
                hAnim.anim.SetBool(_onAir, true);
                hMovement.rb.drag = 0;                
            }
        }

        private bool OnGround()
        {
            var retVal = false;

            var origin = transform.position + Vector3.up / 18;
            var direction = -Vector3.up;
            const float distance = 0.2f;
            var lm = ~(1 << gameObject.layer);

            if(Physics.Raycast(origin, direction, out var hit, distance, lm))
            {
                if (hit.transform.gameObject.layer == gameObject.layer)
                    Debug.Log("OnGround hit an object with the same layer as the controller!!");

                retVal = true;
            }

            return retVal;
        }
    }
}
