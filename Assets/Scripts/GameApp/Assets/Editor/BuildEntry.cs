using System.Diagnostics;
using System.IO;
using UnityEditor;

namespace GameApp.Assets
{
    public static class BuildEntry
    {
        public static BuildTarget Target => EditorUserBuildSettings.activeBuildTarget;
        public static BuildTargetGroup Group => EditorUserBuildSettings.selectedBuildTargetGroup;
        
        [MenuItem("Build/All", false, 201)]
        public static void BuildCurrentAll()
        {
            BuildCurrentAsset();
            BuildCurrentPlayer();
        }
        
        [MenuItem("Build/Assets %&B", false, 202)]
        public static void BuildCurrentAsset()
        {
            UnityEngine.Debug.Log($"Start build assets {Target} {Group}");
            BuildAsset.Build(Target, Group);
        }

        [MenuItem("Build/Player %&P", false, 203)]
        public static void BuildCurrentPlayer()
        {
            UnityEngine.Debug.Log($"Start build player {Target}");
            BuildPlayer.Build(Target);
        }

        [MenuItem("Build/Run %&R", false, 204)]
        public static void RunCurrentPlayer()
        {
            var process = new Process();
            var file = new FileInfo(BuildConfig.GetProjectPath(Target));
            if (!file.Exists)
            {
                UnityEngine.Debug.LogWarning($"{file.FullName} not exsit, try build first!");
                return;
            }
            process.StartInfo.FileName = file.FullName;
            process.Start();
        }
        
    }
}