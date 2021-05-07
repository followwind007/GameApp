#if UNITY_EDITOR
using System;
using UnityEngine;
using UnityEditor;
using System.IO;
using System.Collections.Generic;
using System.Diagnostics;
using Object = UnityEngine.Object;

namespace ToolEditor.Editor.UITools
{
    public class ProfileTimeItem : IComparable<ProfileTimeItem>
    {
        public const string COL_PATH = "path";
        public const string COL_LOAD = "load (ms)";
        public const string COL_INSTANTIATE = "instantiate (ms)";
        public const string COL_TOTAL = "total (ms)";
        public const string COL_SIZE = "size (KB)";

        public enum SortType
        {
            Path, LoadDuration, InstantiateDuration, TotalDuration, Size
        }

        public string path;
        public double loadDura;
        public double instantiateDura;
        public long fileSize;

        public static SortType sortType = SortType.LoadDuration;
        public static bool isInverse;

        private string _subPath;
        public string SubPath
        {
            get
            {
                if (string.IsNullOrEmpty(_subPath))
                {
                    var index = path.IndexOf(UIConst.PATH_GUI_PREFAB, StringComparison.Ordinal);
                    _subPath = index >= 0 ? path.Substring(index + UIConst.PATH_GUI_PREFAB.Length) : path;
                }
                return _subPath;
            }
        }

        public double TotalDura
        {
            get
            {
                return loadDura + instantiateDura;
            }
        }

        public int CompareTo(ProfileTimeItem other)
        {
            int result = 0;
            switch (sortType)
            {
                case SortType.Path:
                    result = String.Compare(path, other.path, StringComparison.Ordinal);
                    break;
                case SortType.LoadDuration:
                    result = loadDura.CompareTo(other.loadDura);
                    break;
                case SortType.InstantiateDuration:
                    result = instantiateDura.CompareTo(other.instantiateDura);
                    break;
                case SortType.TotalDuration:
                    result = TotalDura.CompareTo(other.TotalDura);
                    break;
                case SortType.Size:
                    result = fileSize.CompareTo(other.fileSize);
                    break;
                default:
                    break;
            }
            return isInverse ? -result : result;
        }

        public override string ToString()
        {
            return string.Format("{0}\t{1}\t{2}\t{3}\t{4}", path, loadDura, instantiateDura, TotalDura, fileSize);
        }
    }

    public class UIProfiler
    {
        public string fullPath;

        public string filePattern = ".prefab";

        public Action<int, int, string> progressCallback;

        public List<ProfileTimeItem> ResultTimeList { get; private set; }

        public void Profile()
        {
            Selection.activeObject = null;
            Resources.UnloadUnusedAssets();

            ResultTimeList = new List<ProfileTimeItem>();
            if (Directory.Exists(fullPath))
            {
                var directory = new DirectoryInfo(fullPath);
                FileInfo[] files = directory.GetFiles("*.prefab", SearchOption.AllDirectories);
                for (int i = 0; i < files.Length; i++)
                {
                    var fileInfo = files[i];
                    var index = fileInfo.FullName.IndexOf("Asset", StringComparison.Ordinal);
                    var path = fileInfo.FullName.Substring(index);
                    path = path.Replace('\\', '/');
                    var resTime = ProfileSingleFile(path);
                    if (resTime != null && progressCallback != null)
                        progressCallback(i + 1, files.Length, resTime.SubPath);
                }
            }
            else if (File.Exists(fullPath))
            {
                ProfileSingleFile(fullPath);
            }
            else
            {
                var warn = string.Format("{0}: {1}", UIConst.WARN_FILE_NOT_EXIST, fullPath);
                EditorUtility.DisplayDialog(UIConst.NAME_MODULE, warn, UIConst.NAME_OK);
            }
        }

        private ProfileTimeItem ProfileSingleFile(string path)
        {
            if (!path.EndsWith(filePattern) || !File.Exists(path)) return null;
            var watch = new Stopwatch();
            watch.Start();

            FileInfo fileInfo = new FileInfo(path);
            var resTime = new ProfileTimeItem()
            {
                path = path,
                fileSize = fileInfo.Length,
            };

            var prefab = AssetDatabase.LoadAssetAtPath<GameObject>(path);
            
            resTime.loadDura = watch.Elapsed.TotalMilliseconds;

            watch.Reset();
            watch.Start();
            var instance = Object.Instantiate(prefab);
            resTime.instantiateDura = watch.Elapsed.TotalMilliseconds;

            watch.Stop();
            ResultTimeList.Add(resTime);
            
            Object.DestroyImmediate(instance);
            return resTime;
        }

        public void WriteToFile()
        {

        }



    }
}
#endif