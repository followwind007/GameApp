using System.IO;
using UnityEditor.Build.Reporting;
using UnityEngine;

namespace GameApp.Assets.Commands
{
    [CreateAssetMenu(fileName = "CopyToStreamingAsset", menuName = "Custom/Build/Build Command/CopyToStreamingAsset", order = 0)]
    public class CopyToStreaming : BuildCommand
    {
        public override void Execute(BuildReport report)
        {
            var platform = report.summary.platform;
            if (BuildConfig.UseStreamTargets.Contains(platform))
            {
                var path = BuildConfig.GetBundlePath(platform);
                if (Directory.Exists(path))
                {
                    Debug.Log($"Copy bundles from {path} to {AssetConfig.BundleStreamPath}");
                    DirectoryUtil.DirectoryCopy(path, AssetConfig.BundleStreamPath, true);
                }
            }
        }
    }
}