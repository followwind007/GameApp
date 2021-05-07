using UnityEngine;

namespace GameApp.Util
{
    public class MonoSingleton<T> : MonoBehaviour where T : class
    {
        public static T Instance { get; private set; }
        
        public bool dontDestroy = true;

        protected void Awake()
        {
            if (Instance != null)
            {
                Debug.LogError($"[{typeof(T)}] is already defined!");
                return;
            }
            Instance = this as T;

            if (dontDestroy)
            {
                DontDestroyOnLoad(gameObject);
            }
        }
    }
}