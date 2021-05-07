using System;
using System.Collections.Generic;
using UnityEditor;
using UnityEngine;

namespace GameApp.DebugConsole
{
    public static class RemoteDebugCommand
    {
        public enum UploadType
        {
            Upload, SaveFile, ReloadFile, ClearSelected
        }

        [MenuItem("Assets/DebugConsole/Save and Reload", false, 501)]
        public static void UploadAsset()
        {
            DoUpload(UploadType.Upload);
        }

        [MenuItem("Assets/DebugConsole/Save File", false, 502)]
        public static void SaveFile()
        {
            DoUpload(UploadType.SaveFile);
        }

        [MenuItem("Assets/DebugConsole/Reload File", false, 503)]
        public static void ReloadFile()
        {
            DoUpload(UploadType.ReloadFile);
        }
        
        [MenuItem("Assets/DebugConsole/Clear Selected", false, 503)]
        public static void ClearSelected()
        {
            DoUpload(UploadType.ClearSelected);
        }
        
        [MenuItem("Assets/DebugConsole/Clear All", false, 503)]
        public static void ClearAll()
        {
            if (!Application.isPlaying) return;
            ServerDebugHandler.Instance.ClearSelected(null,true);
        }

        public static void DoUpload(UploadType t)
        {
            if (!Application.isPlaying) return;
            
            var paths = GetSelectedLuaPaths();
            paths.ForEach(p =>
            {
                var server = ServerDebugHandler.Instance;
                switch (t)
                {
                    case UploadType.Upload:
                        server.Upload(p);
                        break;
                    case UploadType.SaveFile:
                        server.SaveFile(p);
                        break;
                    case UploadType.ReloadFile:
                        server.ReloadFile(p);
                        break;
                    case UploadType.ClearSelected:
                        server.ClearSelected(paths, false);
                        break;
                    default:
                        throw new ArgumentOutOfRangeException(nameof(t), t, null);
                }
            });
        }

        private static List<string> GetSelectedLuaPaths()
        {
            var paths = new List<string>();
            var assets = Selection.GetFiltered<DefaultAsset>(SelectionMode.DeepAssets);
            foreach (var asset in assets)
            {
                var p = AssetDatabase.GetAssetPath(asset);
                if (p.EndsWith(".lua")) paths.Add(p);
            }

            return paths;
        }
    }
}