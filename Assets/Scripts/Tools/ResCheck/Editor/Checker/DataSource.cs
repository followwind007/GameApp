using System.Collections.Generic;
using System.IO;
using UnityEditor;

namespace Tools.ResCheck
{
    public static class DataSource
    {
        public class PrefabInfo
        {
            public string path;
            public readonly List<string> particles = new List<string>();
        }
        public class ModuleInfo
        {
            public string name;
            
            public readonly Dictionary<string, PrefabInfo> prefabs = new Dictionary<string, PrefabInfo>();

            //AtlasChecker
            public readonly Dictionary<string, AtlasChecker.AtlasInfo> atlasDict = new Dictionary<string, AtlasChecker.AtlasInfo>();
            public readonly Dictionary<string, AtlasChecker.AtlasItem> spriteDict = new Dictionary<string, AtlasChecker.AtlasItem>();
            public readonly Dictionary<string, List<string>> spriteRefs = new Dictionary<string, List<string>>();

            public override string ToString()
            {
                var res = $"Module: {name}\n";
                foreach (var kv in atlasDict)
                {
                    res += kv.Value + "\n";
                }
                return res;
            }
        }

        private static Dictionary<string, ModuleInfo> _modules;

        public static Dictionary<string, ModuleInfo> Modules
        {
            get
            {
                if (_modules == null)
                {
                    Init();
                }

                return _modules;
            }
        }

        public static void Init()
        {
            _modules = new Dictionary<string, ModuleInfo>();
            var targetDirs = CheckHelper.GetPrefabPaths();

            for (var i = 0; i < targetDirs.Count; i++)
            {
                var dir = targetDirs[i];
                var module = new ModuleInfo { name = dir.Name};
                var files = new List<FileInfo>();
                AssetUtil.GetFiles(dir.FullName, files, "*.prefab");
                foreach (var f in files)
                {
                    var filePath = AssetUtil.GetRelativePath(f.FullName);
                    module.prefabs[filePath] = new PrefabInfo { path = filePath };
                }
                _modules[module.name] = module;
                
                EditorUtility.DisplayProgressBar("Init Module", AssetUtil.GetRelativePath(dir.FullName), (float)i / targetDirs.Count);
            }
            EditorUtility.ClearProgressBar();
        }

        /// <summary>
        /// lty添加的方法，assetDataPath判定是否是一个uiprefab
        /// </summary>
        /// <param name="assetDataPath"></param>
        /// <returns></returns>
        public static bool IsPrefabValid(string assetDataPath)
        {
            if (assetDataPath.EndsWith(".prefab"))
            {
                var dic = Modules;
                foreach (var de in dic)
                {
                    if (de.Value.prefabs.ContainsKey(assetDataPath))
                    {
                        return true;
                    }
                }
            }
            return false;
        }

    }
}