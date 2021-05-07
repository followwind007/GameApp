using System.IO;
using UnityEditor.Build.Reporting;
using UnityEngine;

namespace GameApp.Assets.Commands
{
    [CreateAssetMenu(fileName = "ClearStreamingAsset", menuName = "Custom/Build/Build Command/ClearStreamingAsset", order = 0)]
    public class ClearStreaming : BuildCommand
    {
        public override void Execute(BuildReport report)
        {
            var platform = report.summary.platform;
            if (BuildConfig.UseStreamTargets.Contains(platform))
            {
                if (Directory.Exists(AssetConfig.BundleStreamPath))
                {
                    Debug.Log($"Remove bundles from {AssetConfig.BundleStreamPath}");
                    Directory.Delete(AssetConfig.BundleStreamPath, true);
                    File.Delete($"{AssetConfig.BundleStreamPath}.meta");
                }

                if (Directory.Exists(AssetConfig.GroupStreamPath))
                {
                    Debug.Log($"Remove groups from {AssetConfig.GroupStreamPath}");
                    Directory.Delete(AssetConfig.GroupStreamPath, true);
                    File.Delete($"{AssetConfig.GroupStreamPath}.meta");
                }
            }
        }
    }
}