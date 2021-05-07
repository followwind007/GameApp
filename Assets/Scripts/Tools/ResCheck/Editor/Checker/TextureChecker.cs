using System.Collections.Generic;
using System.IO;
using UnityEditor;
using UnityEngine;

namespace Tools.ResCheck
{
    public class TextureChecker : IChecker
    {
        private static TextureChecker _instance;

        public static TextureChecker Instance => _instance ?? (_instance = new TextureChecker());

        public void StartCheck()
        {
            foreach (var config in ResCheckSettings.Instance.textureConfigs)
            {
                if (!Directory.Exists(config.path))
                {
                    continue;
                }
                CheckConfig(config);
            }
        }

        private void CheckConfig(ResCheckSettings.TextureConfig config)
        {
            var files = new List<FileInfo>();
            AssetUtil.GetFiles(config.path, files, "*");
            
            var summary = "";
            var count = files.Count;
            
            for (var i = 0; i < count; i++)
            {
                var f = files[i];
                if (f.Extension == ".meta")
                {
                    continue;
                }
                var path = AssetUtil.GetRelativePath(f.FullName);
                var texture = AssetDatabase.LoadAssetAtPath<Texture2D>(path);
                var importer = AssetImporter.GetAtPath(path) as TextureImporter;
                
                var res = TextureImportHelper.DealTextureImporter(importer, texture, false);
                summary += res;
                EditorUtility.DisplayProgressBar("Texture Checker", $"Handle: {path}", (float)i / count );
            }
            
            EditorUtility.ClearProgressBar();

            if (string.IsNullOrEmpty(summary))
            {
                Debug.Log($"[{config.path}]未发现任何问题~");
            }
            else
            {
                Debug.LogError($"[{config.path}]请解决以下问题:\n{summary}");
                //写文件
                //记录在AutoQAReports/UIReport目录下
                string name = config.path.Replace("Assets/Art/UI/", "").Replace("/", "");
                string dir = Application.dataPath + "/../AutoQAReports/UIReport";
                if (!Directory.Exists(dir))
                {
                    Directory.CreateDirectory(dir);
                }
                string file = dir + "/" + name + ".txt";
                if(File.Exists(file))
                    File.Delete(file);
                File.WriteAllText(file, summary);
            }
        }

    }
}