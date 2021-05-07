using UnityEngine;

namespace Controller
{
    public class HandleMovement : MonoBehaviour
    {

        public Rigidbody rb;
        private StateManager _states;

        private InputHandler _ih;

        public float moveSpeed = 4;
        public float rotateSpeed = 4;

        private Vector3 _storeDirection;

        public void Init()
        {
            _states = GetComponent<StateManager>();
            rb = GetComponent<Rigidbody>();
            _ih = GetComponent<InputHandler>();

            rb.angularDrag = 999;
            rb.drag = 4;
            rb.constraints = (RigidbodyConstraints)((int)RigidbodyConstraints.FreezeRotationX | (int)RigidbodyConstraints.FreezeRotationZ);
        }

        public void Tick()
        {
            var v = _ih.camHolder.forward * _states.vertical;
            var h = _ih.camHolder.right * _states.horizontal;

            v.y = 0;
            h.y = 0;

            if (_states.onGround)
            {
                Debug.Log((v + h).normalized * Speed());
                rb.AddForce((v + h).normalized * Speed());
            }

            if(Mathf.Abs(_states.vertical) > 0 || Mathf.Abs(_states.horizontal) > 0)
            {
                _storeDirection = (v + h).normalized;

                _storeDirection += transform.position;

                var targetDir = (_storeDirection - transform.position).normalized;
                targetDir.y = 0;

                if (targetDir == Vector3.zero)
                    targetDir = transform.forward;

                var targetRot = Quaternion.LookRotation(targetDir);
                transform.rotation = Quaternion.Slerp(transform.rotation, targetRot, rotateSpeed * Time.deltaTime);
            }
        }

        private float Speed()
        {
            return moveSpeed;
        }
    }
}
