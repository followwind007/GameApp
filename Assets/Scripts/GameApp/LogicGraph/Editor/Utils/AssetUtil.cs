using UnityEngine;
using UnityEditor;
using System.IO;
using System.Collections.Generic;
using System.Linq;

namespace GameApp.LogicGraph
{
    public static class AssetUtil
    {
        [MenuItem("Assets/Create/Custom/LogicGraphSettings")]
        private static void CreateSettings()
        {
            CreateAsset<LogicGraphSettings>("LogicGraphSettings");
        }
        
        public static void CreateAsset<T>(string assetName = null, string assetPath = null) where T : ScriptableObject
        {
            var asset = ScriptableObject.CreateInstance<T>();

            var path = string.IsNullOrEmpty(assetPath) ? AssetDatabase.GetAssetPath(Selection.activeObject) : assetPath;
            if (string.IsNullOrEmpty(path) || !string.IsNullOrEmpty(Path.GetExtension(path)))
            {
                EditorUtility.DisplayDialog("Warning", "Select a folder first", "OK");
                return;
            }
            if (string.IsNullOrEmpty(assetName)) assetName = typeof(T).ToString();
            
            var assetPathAndName = AssetDatabase.GenerateUniqueAssetPath(string.Format("{0}/{1}.asset", path, assetName));

            AssetDatabase.CreateAsset(asset, assetPathAndName);

            AssetDatabase.SaveAssets();
            AssetDatabase.Refresh();
            EditorUtility.FocusProjectWindow();
            Selection.activeObject = asset;
        }
        
        public static string GetRelativePath(string path)
        {
            var curDir = Directory.GetCurrentDirectory();
            curDir += curDir.Contains("/") ? "/" : "\\";
            return path.Replace(curDir, "").Replace("\\", "/");
        }
        
        public static void GetFiles(string path, List<FileInfo> fileList, string pattern)
        {
            var dir = new DirectoryInfo(path);
            var fil = dir.GetFiles(pattern);
            var dii = dir.GetDirectories();
            
            fileList.AddRange(fil.Select(f => f));

            foreach (var d in dii)
            {
                GetFiles(d.FullName, fileList, pattern);
            }
        }

    }
}