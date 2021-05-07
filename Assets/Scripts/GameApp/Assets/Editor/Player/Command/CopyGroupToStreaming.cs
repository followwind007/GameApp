using System.IO;
using UnityEditor.Build.Reporting;
using UnityEngine;

namespace GameApp.Assets.Commands
{
    [CreateAssetMenu(fileName = "CopyGroupToStreaming", menuName = "Custom/Build/Build Command/CopyGroupToStreaming", order = 0)]
    public class CopyGroupToStreaming : BuildCommand
    {
        public override void Execute(BuildReport report)
        {
            var platform = report.summary.platform;
            if (BuildConfig.UseStreamTargets.Contains(platform))
            {
                var path = BuildConfig.GetGroupPath(platform);
                if (Directory.Exists(path))
                {
                    Debug.Log($"Copy groups from {path} to {AssetConfig.GroupStreamPath}");
                    DirectoryUtil.DirectoryCopy(path, AssetConfig.GroupStreamPath, true);
                }

                //copy manifest
                var bundlePath = BuildConfig.GetBundlePath(platform);
                var manifestPath = $"{bundlePath}/{AssetConfig.ManifestBundle}";
                var manifestStreamPath = $"{AssetConfig.BundleStreamPath}/{AssetConfig.ManifestBundle}";
                
                var fileInfo = new FileInfo(manifestStreamPath);
                if (fileInfo.Directory is {Exists: false})
                {
                    Directory.CreateDirectory(fileInfo.Directory.FullName);
                }
                File.Copy(manifestPath, manifestStreamPath, true);
                Debug.Log($"Copy manifest file from {manifestPath} to {manifestStreamPath}");
            }
        }
    }
}