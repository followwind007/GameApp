using UnityEditor;
using UnityEngine;

namespace Tools.ResCheck
{
    public static class PrefabUtil
    {
        public static void CreatePrefab()
        {
            
        }

        public static GameObject CreatePrefabVariant(string prefabPath, string variantAssetPath)
        {
            var source = Resources.Load(prefabPath);
            var objSource = (GameObject)PrefabUtility.InstantiatePrefab(source);
            var obj = PrefabUtility.SaveAsPrefabAsset(objSource, variantAssetPath);
            return obj;
        }
    }
}