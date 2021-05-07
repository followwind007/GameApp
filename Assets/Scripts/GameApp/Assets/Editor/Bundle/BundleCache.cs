using System;
using System.Collections.Generic;
using UnityEditor;
using UnityEngine;

namespace GameApp.Assets
{
    [CreateAssetMenu(fileName = "BundleCache", menuName = "Custom/Build/Bundle Cache", order = 202)]
    public class BundleCache : ScriptableObject
    {
        [Serializable]
        public class AssetData
        {
            public string name;
            
            [SerializeField]
            private string hash;

            private Hash128 _hash;
            public Hash128 Hash
            {
                get
                {
                    if (!_hash.isValid)
                    {
                        _hash = Hash128.Parse(hash);
                    }
                    return _hash;
                }
                set
                {
                    _hash = value;
                    hash = _hash.ToString();
                }
            }


            public bool IsDirty => AssetDatabase.GetAssetDependencyHash(name) != Hash;

            public AssetData(string name)
            {
                this.name = name;
                Hash = AssetDatabase.GetAssetDependencyHash(name);
            }

            public bool TryUpdateHash()
            {
                var curHash = AssetDatabase.GetAssetDependencyHash(name);
                if (curHash == Hash)
                {
                    return false;
                }
                else
                {
                    Hash = curHash;
                    return true;
                }
            }
        }
        
        [Serializable]
        public class BundleData
        {
            public string name;
            public int version;

            public BundleData(string name)
            {
                this.name = name;
                version = 0;
            }

            public void IncreaseVersion()
            {
                version++;
            }
        }

        private static BundleCache _instance;
        private static AssetManifest _manifest;
        public static AssetManifest Manifest => _manifest;

        public static BundleCache Instance
        {
            get
            {
                if (_instance != BuildSettings.Instance.bundleCache)
                {
                    _instance = BuildSettings.Instance.bundleCache;
                    _manifest = AssetDatabase.LoadAssetAtPath<AssetManifest>(AssetConfig.ManifestName);
                    if (_manifest != null)
                    {
                        _manifest.Init();
                    }
                    _instance.Init();
                }

                return _instance;
            }
        }

        public List<AssetData> assets;
        public List<BundleData> bundles;

        public readonly Dictionary<string, AssetData> assetDict = new Dictionary<string, AssetData>();
        public readonly Dictionary<string, BundleData> bundleDict = new Dictionary<string, BundleData>();

        public static bool IsBundleDirty(AssetBundleBuild build)
        {
            return Instance.IsBundleDirtyImpl(build);
        }

        public static void SaveCache(List<AssetBundleBuild> builds)
        {
            Instance.SaveCacheImpl(builds);
        }

        public static void SaveCache(AssetBundleBuild build)
        {
            Instance.SaveCacheImpl(build);
        }

        public static bool IsBuildValid(AssetBundleBuild build)
        {
            if (BundleSettings.Context.mode == BuildMode.Progressive)
            {
                if (!IsBundleDirty(build)) return false;
            }

            return true;
        }
        
        public static string BundleNameWithVersion(string path)
        {
            var ver = 0;
            if (Instance.bundleDict.TryGetValue(path, out var bundleData))
                ver = bundleData.version;
            
            return BuildConfig.BundleNameWithVersion(path, ver);
        }
        
        private void Init()
        {
            assetDict.Clear();
            foreach (var asset in assets)
            {
                assetDict[asset.name] = asset;
            }
            
            bundleDict.Clear();
            foreach (var bundle in bundles)
            {
                bundleDict[bundle.name] = bundle;
            }
        }
        
        private bool IsBundleDirtyImpl(AssetBundleBuild build)
        {
            if (Manifest == null) return true;

            var mBundle = Manifest.GetBundle(BundleNameWithVersion(build.assetBundleName));

            return mBundle == null;
        }
        
        private void SaveCacheImpl(List<AssetBundleBuild> builds)
        {
            foreach (var build in builds)
            {
                SaveCacheImpl(build);
            }
        }

        private void SaveCacheImpl(AssetBundleBuild build)
        {
            if (!bundleDict.TryGetValue(build.assetBundleName, out var bundleData))
            {
                bundleData = AddBundleData(build.assetBundleName);
            }

            var isDirty = false;
            foreach (var assetName in build.assetNames)
            {
                if (!assetDict.TryGetValue(assetName, out var assetData))
                {
                    AddAssetData(assetName);
                    continue;
                }

                if (assetData.TryUpdateHash())
                {
                    isDirty = true;
                }
            }

            if (isDirty)
            {
                bundleData.IncreaseVersion();
            }
        }

        private BundleData AddBundleData(string bundleName)
        {
            var bundleDate = new BundleData(bundleName); 
            bundles.Add(bundleDate);
            bundleDict[bundleName] = bundleDate;
            return bundleDate;
        }

        private void AddAssetData(string assetName)
        {
            var assetData = new AssetData(assetName);
            assets.Add(assetData);
            assetDict[assetName] = assetData;
        }

        public void ClearRedundant()
        {
            var valids = new List<AssetData>();
            foreach (var asset in assets)
            {
                var guid = AssetDatabase.AssetPathToGUID(asset.name);
                if (!string.IsNullOrEmpty(guid))
                {
                    valids.Add(asset);
                }
            }

            assets = valids;
            Init();
        }
        
    }
}