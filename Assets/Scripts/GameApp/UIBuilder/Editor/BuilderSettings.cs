using System;
using UnityEngine;
using UnityEditor;

namespace GameApp.UIBuilder
{
    public class BuilderSettings : ScriptableObject
    {
        private static BuilderSettings _instance;

        public static BuilderSettings instance
        {
            get
            {
                if (_instance == null)
                {
                    _instance = Resources.Load<BuilderSettings>("UIBuilder/Settings");
                    if (_instance == null)
                    {
                        _instance = CreateInstance<BuilderSettings>();
                        AssetDatabase.CreateAsset(_instance, "Assets/Scripts/GameApp/UIBuilder/Editor/Resources/UIBuilder/Settings.asset");
                    }
                }

                return _instance;
            }
        }

        public string prefabRootPath = "Assets/Resources/Prefab/UI/";

        public string previewIconPath = "./Tools/UIBuilder/";

        public PreviewEnv uiPreview;
        
        [Range(128, 512)]
        public int previewIconSize = 256;

        public static string GetRelativeAssetPath(string path)
        {
            var idx = path.IndexOf("Assets", StringComparison.Ordinal);
            return idx > 0 ? path.Substring(idx) : path;
        }
        
        public static string GetPreviewIconName(string prefabPath)
        {
            var path = GetRelativeAssetPath(prefabPath);
            path = path.Replace("\\", "/");
            path = path.Replace(instance.prefabRootPath, "");
            
            path = path.Replace("/", "_");
            path = path.Replace(".prefab", ".png");
            return path;
        }

        public static string GetUniqueId(string path)
        {
            var replace = GetPreviewIconName(path).Replace(".png", "");
            return replace;
        }

        public static string GetIconPath(string prefabPath)
        {
            var iconName = GetPreviewIconName(prefabPath);
            return $"{instance.previewIconPath}{iconName}";
        }


    }
}