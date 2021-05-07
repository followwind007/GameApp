using System.Collections.Generic;
using System.IO;
using UnityEngine;

namespace Tools.ResCheck
{
    public static class CheckHelper
    {
        public static List<DirectoryInfo> GetPrefabPaths()
        {
            var moduleRoot = new DirectoryInfo(ResCheckSettings.Instance.uiPrefabRoot);
            if (!moduleRoot.Exists)
            {
                Debug.LogError($"not exist prefab root: {ResCheckSettings.Instance.uiPrefabRoot}");
                return null;
            }
            
            //get all modules
            var targetDirs = new List<DirectoryInfo>();
            foreach (var ex in ResCheckSettings.Instance.extraPrefabList)
            {
                targetDirs.Add(new DirectoryInfo(ex));
                var paths = new List<FileInfo>();
                AssetUtil.GetFiles(ex, paths, "*.prefab");
            }
            targetDirs.AddRange(moduleRoot.GetDirectories());
            return targetDirs;
        }

        public static void InitUIChecker()
        {
            AtlasChecker.Instance.StartCheck();
        }
        
        public static bool IsResValid(string path)
        {
            if (path.EndsWith(".png") && path.Contains(ResCheckSettings.Instance.atlasPath))
            {
                var spriteName = AtlasChecker.GetSpriteName(path);
                AtlasChecker.Items.TryGetValue(spriteName, out var sprite);
                if (sprite != null)
                {
                    return sprite.ReferenceCount > 0;
                }
                return false;
            }
            
            return false;
        }
        
    }
}