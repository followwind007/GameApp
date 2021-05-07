using UnityEngine;

namespace GameApp.Assets
{
    [CreateAssetMenu(fileName = "BuildSettings", menuName = "Custom/Build/BuildSettings", order = 0)]
    public class BuildSettings : ScriptableObject
    {
        private static BuildSettings _instance;
        public static BuildSettings Instance
        {
            get
            {
                if (_instance == null)
                {
                    _instance = Resources.Load<BuildSettings>("BuildSettings");
                }
                return _instance;
            }
        }

        public BundleSettings bundleSettings;
        public BundleCache bundleCache;
        public BuildPlayerSettings playerSettings;
    }
}