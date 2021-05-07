using System.IO;
using UnityEngine;

namespace GameApp.Assets
{
    public static class AssetConfig
    {
        public static readonly string DataPath = Application.dataPath;

        public static readonly string StreamPath = Application.streamingAssetsPath;

        public static readonly string PersistPath = Application.persistentDataPath;

        public static readonly string BundleDataPath =
#if UNITY_EDITOR_WIN || UNITY_STANDALONE_WIN
            $"{PersistPath}/{RuntimePlatform.WindowsPlayer}/bundles";
#elif UNITY_EDITOR_OSX || UNITY_STANDALONE_OSX
            $"{PersistPath}/{RuntimePlatform.OSXPlayer}/bundles";
#else
            $"{DataPath}/bundles";
#endif
        
        public static readonly string GroupDataPath =
#if UNITY_EDITOR_WIN || UNITY_STANDALONE_WIN
            $"{PersistPath}/{RuntimePlatform.WindowsPlayer}/groups";
#elif UNITY_EDITOR_OSX || UNITY_STANDALONE_OSX
            $"{PersistPath}/{RuntimePlatform.OSXPlayer}/groups";
#else
            $"{DataPath}/groups";
#endif
        
        public static readonly string BundleStreamPath = $"{StreamPath}/bundles";
        public static readonly string GroupStreamPath = $"{StreamPath}/groups";

        public const string ManifestBundle = "assets/manifest";
        public const string ManifestName = "Assets/Manifest.asset";

        public static string GetBundlePath(string bundleName)
        {
            var dataPath = $"{BundleDataPath}/{bundleName}";
            if (File.Exists(dataPath)) return dataPath;

            var streamPath = $"{BundleStreamPath}/{bundleName}";
            return streamPath;
        }

        public static string GetGroupPath(int groupIdx)
        {
            var dataPath = $"{GroupDataPath}/{groupIdx}";
            if (File.Exists(dataPath)) return dataPath;

            var streamPath = $"{GroupStreamPath}/{groupIdx}";
            return streamPath;
        }

        public static string GetSceneName(string path)
        {
            var idx = path.LastIndexOf('/');
            var sub = idx > 0 ? path.Substring(idx + 1, path.Length - idx - 1) : path;
            return sub.Replace(".unity", "");
        }
    }
}