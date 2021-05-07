using System.Collections.Generic;
using System.IO;
using Tools;
using UnityEditor;
using UnityEngine;

namespace GameApp.UIBuilder
{
    public static class PreviewUtil
    {
        public static void RegenerateAll()
        {
            var setting = BuilderSettings.instance;
            
            //delete old directory and regenerate it
            if (Directory.Exists(setting.previewIconPath))
                Directory.Delete(setting.previewIconPath, true);
            Directory.CreateDirectory(setting.previewIconPath);
            
            //prepare env
            var envGo = Object.Instantiate(setting.uiPreview.gameObject);
            envGo.transform.position = new Vector3(4096, 4096, 4096);
            var env = envGo.GetComponent<PreviewEnv>();

            var paths = new List<FileInfo>();
            AssetUtil.GetFiles(BuilderSettings.instance.prefabRootPath, paths, "*.prefab");
            foreach (var path in paths)
            {
                //EditorUtility.DisplayProgressBar("Generate Preview", path.FullName, (float) i / paths.Count);
                
                var relativePath = BuilderSettings.GetRelativeAssetPath(path.FullName);
                var iconPath = $"{setting.previewIconPath}{BuilderSettings.GetPreviewIconName(relativePath)}";
                env.AddTarget(iconPath, relativePath);
            }
            
        }

        public static Texture2D GetTexture(FileInfo file)
        {
            var path = BuilderSettings.GetIconPath(file.FullName);
            var tex = AssetDatabase.LoadAssetAtPath<Texture2D>(path);
            return tex;
        }

        public static Vector2Int GetPreviewSize(Rect rect)
        {
            var limit = BuilderSettings.instance.previewIconSize;
            var ratio = rect.height / rect.width;
            var size = new Vector2Int();

            if (ratio < 1)
            {
                if (limit > rect.width)
                {
                    size.x = Mathf.FloorToInt(rect.width);
                    size.y = Mathf.FloorToInt(rect.width * ratio);
                }
                else
                {
                    size.x = Mathf.FloorToInt(limit);
                    size.y = Mathf.FloorToInt(limit * ratio);
                }
            }
            else
            {
                if (limit > rect.height)
                {
                    size.x = Mathf.FloorToInt(rect.height / ratio);
                    size.y = Mathf.FloorToInt(rect.height);
                }
                else
                {
                    size.x = Mathf.FloorToInt(limit / ratio);
                    size.y = Mathf.FloorToInt(limit);
                }
            }

            if (size.x < 128)
            {
                size.x = 128;
                size.y = (int)(128f * ratio);
            }

            return size;
        }
        
    }
}