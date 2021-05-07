using UnityEngine;

namespace GameApp.Util
{
    public class MonoHolder : MonoBehaviour
    {
        private static GameObject _holder;
        
        public static GameObject Holder {
            get
            {
                if (_holder == null)
                {
                    _holder = new GameObject("_MonoHolder");
                    DontDestroyOnLoad(_holder);
                }

                return _holder;
            }
        }
    }
}