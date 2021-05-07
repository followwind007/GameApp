using UnityEditor;
using UnityEngine;

namespace GameApp.AssetProcessor
{
    public static class PrefabUtil
    {
        public static void ClonePrefab(string sourcePath, string targetPath)
        {
            var go = AssetDatabase.LoadAssetAtPath<GameObject>(targetPath);
            if (go == null)
            {
                var sourceGo = AssetDatabase.LoadAssetAtPath<GameObject>(sourcePath);
                var model = (GameObject)PrefabUtility.InstantiatePrefab(sourceGo);
                
                PrefabUtility.UnpackPrefabInstance(model, PrefabUnpackMode.OutermostRoot, InteractionMode.AutomatedAction);
                PrefabUtility.SaveAsPrefabAsset(model, targetPath);
                
                Object.DestroyImmediate(model);
            }
        }
    }
}