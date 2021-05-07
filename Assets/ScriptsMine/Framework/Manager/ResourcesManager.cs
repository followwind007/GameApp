using UnityEngine;
using System.Collections;

namespace Pangu.Manager
{
    public class ResourcesManager : MonoBehaviour
    {
        public static ResourcesManager Instance;

        private void Awake()
        {
            Instance = this;
        }

        public Object LoadAssetAtPath(string path, System.Type type, string assetName = null)
        {
            return null;
        }
    }

}