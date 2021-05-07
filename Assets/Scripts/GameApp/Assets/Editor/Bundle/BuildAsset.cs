using System.Collections.Generic;
using System.Diagnostics;
using System.IO;
using System.Linq;
using UnityEditor;
using UnityEditor.Build.Pipeline;
using UnityEngine;
using UnityEngine.Build.Pipeline;
using Debug = UnityEngine.Debug;

namespace GameApp.Assets
{
    public static class BuildAsset
    {
        private static BundleSettings Settings => BundleSettings.Instance;
        
        private static List<BuildRule> GetBuildRules(List<BundleSettings.Build> builds)
        {
            var rules = new List<BuildRule>();

            foreach (var b in builds)
            {
                GenerateBuildRule(rules, b);
            }

            if (Settings.context.useScenesInBuild)
            {
                var dirPaths = new List<string>();
                foreach (var scene in EditorBuildSettings.scenes) dirPaths.Add(scene.path);
                rules.Add(new DirectSingleRule(dirPaths));
            }

            return rules;
        }

        private static void GenerateBuildRule(List<BuildRule> rules, BundleSettings.Build b)
        {
            switch (b.buildType)
            {
                case BuildType.Single:
                    b.paths.ForEach(p => { rules.Add(new SingleRule(p.pattern, p.Paths)); });
                    break;
                case BuildType.Aggregate:
                    b.paths.ForEach(p => { rules.Add(new AggregateRule(p.pattern, p.Paths)); });
                    break;
                case BuildType.Direct:
                    rules.Add(new DirectSingleRule(b.paths.Select(p => p.path).ToArray()));
                    break;
            }
        }

        public static void Build(BuildTarget target, BuildTargetGroup group)
        {
            var watch = new Stopwatch();
            watch.Start();

            var report = new BundleCommand.BundleReport(){target = target};
            var details = new Dictionary<string, BundleDetails>();
            Settings.preCommands.ForEach(cmd => cmd.Execute(report));
            
            var path = BuildConfig.GetBundlePath(target);
            if (Directory.Exists(path) && BundleSettings.Context.mode == BuildMode.Fresh)
            {
                Directory.Delete(path, true);
                AssetDatabase.DeleteAsset(AssetConfig.ManifestName);
            }
            Directory.CreateDirectory(path);
            var builds = new List<AssetBundleBuild>();
            
            foreach (var g in Settings.groups)
            {
                g.preCommands.ForEach(cmd => cmd.Execute(report));
                var valids = new List<AssetBundleBuild>();
                var rules = GetBuildRules(g.builds);
            
                foreach (var rule in rules)
                {
                    builds.AddRange(rule.builds);
                    valids.AddRange(rule.valids);
                }

                var buildContent = new BundleBuildContent(valids);
                var buildParams = new BundleBuildParameters(target, group, path);
                var returnCode = ContentPipeline.BuildAssetBundles(buildParams, buildContent, out var result);
                if (returnCode == ReturnCode.Success)
                {
                    foreach (var kv in result.BundleInfos)
                        details.Add(kv.Key, kv.Value);
                }
                
                Debug.Log($"build assets {BundleSettings.Context.mode} {valids.Count} bundle(s), take {watch.ElapsedMilliseconds / 1000f} seconds");
                g.postCommands.ForEach(cmd => cmd.Execute(report));
            }
            UpdateManifest(builds, details);
            Settings.postCommands.ForEach(cmd => cmd.Execute(report));
            BuildManifest(target, group, path);
            
            EditorUtility.SetDirty(BundleCache.Instance);
            AssetDatabase.SaveAssets();
        }

        private static void UpdateManifest(List<AssetBundleBuild> builds, Dictionary<string, BundleDetails> details)
        {
            var assetPath = AssetConfig.ManifestName;

            var manifest = BundleCache.Manifest;
            if (manifest == null)
            {
                manifest = ScriptableObject.CreateInstance<AssetManifest>();
                AssetDatabase.CreateAsset(manifest, assetPath);
                manifest.Init();
            }

            var buildDict = new Dictionary<string, AssetBundleBuild>();
            foreach (var build in builds) buildDict[build.assetBundleName] = build;
            
            foreach (var kv in details)
            {
                manifest.UpdateBundleInfo(kv.Key, buildDict[kv.Key].assetNames, kv.Value.Dependencies, kv.Value.Crc);
            }
            
            manifest.ClearRedundant(builds.Select(b => b.assetBundleName));
            
            EditorUtility.SetDirty(manifest);
            AssetDatabase.SaveAssets();
        }

        private static void BuildManifest(BuildTarget target, BuildTargetGroup group, string path)
        {
            var mBuild = new List<AssetBundleBuild>
            {
                new AssetBundleBuild
                {
                    assetBundleName = AssetConfig.ManifestBundle,
                    assetNames = new []{AssetConfig.ManifestName}
                }
            };
            var buildContent = new BundleBuildContent(mBuild);
            var buildParams = new BundleBuildParameters(target, group, path);
            ContentPipeline.BuildAssetBundles(buildParams, buildContent, out _);
        }

    }
}