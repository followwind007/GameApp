using System;
using System.Collections.Generic;
using UnityEditor;
using UnityEngine;

namespace GameApp.Assets
{
    public static class BuildConfig
    {
        public static readonly Dictionary<BuildTarget, RuntimePlatform> TargetMaps =
            new Dictionary<BuildTarget, RuntimePlatform>
            {
                {BuildTarget.StandaloneWindows64, RuntimePlatform.WindowsPlayer},
                {BuildTarget.StandaloneWindows, RuntimePlatform.WindowsPlayer},
                {BuildTarget.StandaloneOSX, RuntimePlatform.OSXPlayer},
                {BuildTarget.Android, RuntimePlatform.Android},
                {BuildTarget.iOS, RuntimePlatform.IPhonePlayer},
            };

        public static readonly HashSet<BuildTarget> UseStreamTargets = new HashSet<BuildTarget>
        {
            BuildTarget.Android, BuildTarget.iOS
        };

        public static readonly Dictionary<BuildTarget, string> TargetExtension = new Dictionary<BuildTarget, string>
        {
            {BuildTarget.StandaloneWindows, "exe"},
            {BuildTarget.StandaloneWindows64, "exe"},
            {BuildTarget.Android, "apk"}
        };
        
        private const string ProjectRoot = "Projects";
        public static string GetProjectPath(BuildTarget target)
        {
            var path = $"{ProjectRoot}/{target}/{PlayerSettings.productName}";
            if (TargetExtension.ContainsKey(target))
            {
                path = $"{path}.{TargetExtension[target]}";
            }
            return path;
        }

        public static string GetBundlePath(BuildTarget target)
        {
            if (!TargetMaps.ContainsKey(target))
            {
                throw new Exception($"Can not support current target {target}, add to map first!");
            }
            return $"{AssetConfig.PersistPath}/{TargetMaps[target]}/bundles";
        }

        public static string GetGroupPath(BuildTarget target)
        {
            if (!TargetMaps.ContainsKey(target))
            {
                throw new Exception($"Can not support current target {target}, add to map first!");
            }
            return $"{AssetConfig.PersistPath}/{TargetMaps[target]}/groups";
        }
        
        public static string BundleNameWithVersion(string path, int version)
        {
            return $"{path}_{version}";
        }
    }
}