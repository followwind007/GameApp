using UnityEngine;
using Framework.Manager;

namespace GameApp.ScenePlayable
{
    public abstract class ScenePlayableObject : MonoBehaviour
    {
        public string eventName = "";

        public bool isTrigger = false;

        public abstract void Pause();

        public abstract void Play();

        public abstract void Stop();

        private void Start()
        {
            ScenePlayableManager.Instance.RegisterPlayable(this);
            if (isTrigger)
            {
                gameObject.layer = ScenePlayableUtil.RECEIVER_LAYER;
            }
        }

        private void OnDestroy()
        {
            ScenePlayableManager.Instance.DeRegisterPlayable(this);
        }

    }
}
