using System.Collections.Generic;
using UnityEngine;
using UnityEditor;
using System.IO;
using System.Linq;

namespace Tools
{
    public static class AssetUtil
    {
        public static T CreateAsset<T>(string assetName = null) where T : ScriptableObject
        {
            var asset = ScriptableObject.CreateInstance<T>();

            var path = AssetDatabase.GetAssetPath(Selection.activeObject);
            if (!string.IsNullOrEmpty(Path.GetExtension(path)))
            {
                var lastIdx = path.LastIndexOf('/');
                if (lastIdx >= 0)
                {
                    path = path.Substring(0, lastIdx + 1);
                }
            }
            if (string.IsNullOrEmpty(path))
            {
                EditorUtility.DisplayDialog("Warning", "Select a folder first", "OK");
                return null;
            }
            if (string.IsNullOrEmpty(assetName)) assetName = typeof(T).ToString();
            
            var assetPathAndName = AssetDatabase.GenerateUniqueAssetPath(string.Format("{0}/{1}.asset", path, assetName));

            AssetDatabase.CreateAsset(asset, assetPathAndName);

            AssetDatabase.SaveAssets();
            AssetDatabase.Refresh();
            EditorUtility.FocusProjectWindow();
            Selection.activeObject = asset;

            return asset;
        }
        
        public static T GetOrCreateScriptableAsset<T>(string path) where T : ScriptableObject
        {
            var scriptableAsset = AssetDatabase.LoadAssetAtPath<T>(path);
            if (scriptableAsset != null)
            {
                return scriptableAsset;
            }

            scriptableAsset = ScriptableObject.CreateInstance<T>();
            AssetDatabase.CreateAsset(scriptableAsset, path);

            EditorUtility.SetDirty(scriptableAsset);
            AssetDatabase.Refresh();

            return scriptableAsset;
        }
        
        public static void GetFiles(string path, List<FileInfo> fileList, string pattern)
        {
            var dir = new DirectoryInfo(path);
            if (!dir.Exists)
            {
                Debug.LogError($"[{path}] is not exsit!");
            }
            var fil = dir.GetFiles(pattern);
            var dii = dir.GetDirectories();
            
            fileList.AddRange(fil.Select(f => f));

            foreach (var d in dii)
            {
                GetFiles(d.FullName, fileList, pattern);
            }
        }
        
        public static string GetRelativePath(string path)
        {
            var curDir = Directory.GetCurrentDirectory();
            curDir += curDir.Contains("/") ? "/" : "\\";
            return path.Replace(curDir, "").Replace("\\", "/");
        }

        public static string GetFileFolderName(string path)
        {
            var nPath = GetRelativePath(path);
            var last = nPath.LastIndexOf('/');
            var sec = -1;
            for (var i = last - 1; i >= 0; i--)
            {
                if (nPath[i] == '/')
                {
                    sec = i;
                    break;
                }
            }
            return sec > 0 ? nPath.Substring(sec + 1, last - sec - 1) : null;
        }

        public static string GetPathInGameObject(GameObject go)
        {
            var path = "";
            var parts = new List<string>();

            var cur = go.transform;
            while (cur)
            {
                parts.Add(cur.gameObject.name);
                cur = cur.parent;
            }

            for (var i = parts.Count - 1; i >= 0; i--)
            {
                path += i == 0 ? parts[i] : parts[i] + "/";
            }
            
            return path;
        }
        
        public static T CreateOrReplaceAsset<T> (T newAsset, string path) where T:Object
        {
            var existingAsset = AssetDatabase.LoadAssetAtPath<T>(path);

            if (existingAsset == null)
            {
                AssetDatabase.CreateAsset(newAsset, path);
                existingAsset = newAsset;
            }
            else
            {
                EditorUtility.CopySerialized(newAsset, existingAsset);
            }
            return existingAsset;
        }

    }
}
