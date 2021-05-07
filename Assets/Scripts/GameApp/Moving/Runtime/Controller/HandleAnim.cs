using UnityEngine;

namespace Controller
{
    public class HandleAnim : MonoBehaviour
    {
        private StateManager _states;
        public Animator anim;
        private static readonly int _movement = Animator.StringToHash("Movement");

        public void Init(StateManager st)
        {
            _states = st;
            anim = GetComponent<Animator>();
            
            var childAnims = GetComponentsInChildren<Animator>();

            foreach (var t in childAnims)
            {
                if(t != anim)
                {
                    anim.avatar = t.avatar;
                    Destroy(t);
                    break;
                }
            }
        }

        public void Tick()
        {
            var animValue = Mathf.Abs(_states.horizontal) + Mathf.Abs(_states.vertical);
            animValue = Mathf.Clamp01(animValue);

            anim.SetFloat(_movement,animValue);
        }
    }
}
