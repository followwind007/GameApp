using UnityEngine;

namespace GameApp.ScenePlayable
{
    public static class PlayableLoader
    {
        public const string RES_ROOT = "Assets/Res/";
        public const string RES_TEMP_ROOT = "Assets/ResTemp/";
        public const string RES_TEST_ROOT = "Assets/ResTest/";

        public static T LoadAssetAtPath<T>(string path) where T : Object
        {
            return LoadAssetAtPath(path, typeof(T)) as T;
        }

        public static Object LoadAssetAtPath(string path, System.Type type)
        {
            Object obj = null;
#if UNITY_EDITOR
            obj = UnityEditor.AssetDatabase.LoadAssetAtPath(path, type);
            if (obj == null)
            {
                var bundleName = GetBundleNameWithPath(path);
                obj = UnityEditor.AssetDatabase.LoadAssetAtPath(RES_ROOT + bundleName, type);
                if (obj == null) obj = UnityEditor.AssetDatabase.LoadAssetAtPath(RES_TEMP_ROOT + bundleName, type);
                if (obj == null) obj = UnityEditor.AssetDatabase.LoadAssetAtPath(RES_TEST_ROOT + bundleName, type);
                if (obj == null) Debug.LogWarning("fail to load resource from: " + path);
            }
#else
            obj = LoadAssetInBundle(path, type);
#endif
            
            return obj;
        }

        public static string GetBundleNameWithPath(string path)
        {
            string bundleName = null;
            if (path.StartsWith(RES_ROOT))
            {
                bundleName = path.Substring(RES_ROOT.Length);
            }
            else if (path.StartsWith(RES_TEMP_ROOT))
            {
                bundleName = path.Substring(RES_TEMP_ROOT.Length);
            }
            else if (path.StartsWith(RES_TEST_ROOT))
            {
                bundleName = path.Substring(RES_TEST_ROOT.Length);
            }
            return bundleName;
        }

        public static Object LoadAssetInBundle(string path, System.Type type)
        {
            //Object obj = null;
            //obj = Manager.ResourcesManager.Instance.LoadAssetAtPath(path, type);
            return null;
        }

    }

}