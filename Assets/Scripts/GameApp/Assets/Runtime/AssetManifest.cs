using System;
using System.Collections.Generic;
using UnityEngine;

namespace GameApp.Assets
{
    public class AssetManifest : ScriptableObject
    {
        [Serializable]
        public class BundleInfo
        {
            public string bundle;
            public ulong offset;
            public ushort gidx;
            public uint crc;
            public string[] assets;
            public string[] deps;
        }
        
        public List<BundleInfo> bundles = new List<BundleInfo>();
        
        private Dictionary<string, string> _bundleAssetMap;
        private Dictionary<string, BundleInfo> _bundleDict;

        public void Init()
        {
            _bundleAssetMap = new Dictionary<string, string>();
            _bundleDict = new Dictionary<string, BundleInfo>();
            
            foreach (var b in bundles)
            {
                _bundleDict[string.Intern(b.bundle)] = b;
                foreach (var assetName in b.assets)
                {
                    _bundleAssetMap[string.Intern(assetName)] = string.Intern(b.bundle);
                }
            }
        }

        public void UpdateBundleInfo(string bundle, string[] assets, string[] deps, uint crc)
        {
            if (_bundleDict.TryGetValue(bundle, out var b))
            {
                b.assets = assets;
                b.deps = deps;
                b.crc = crc;
            }
            else
            {
                var bundleInfo = new BundleInfo
                {
                    bundle = bundle,
                    assets = assets,
                    deps = deps,
                    crc = crc
                };
                bundles.Add(bundleInfo);
                _bundleDict[bundle] = bundleInfo;
                foreach (var assetName in bundleInfo.assets)
                {
                    _bundleAssetMap[assetName] = bundle;
                }
            }
        }

        public void ClearRedundant(IEnumerable<string> validBundles)
        {
            var valids = new List<BundleInfo>();
            foreach (var bundleName in validBundles)
            {
                if (_bundleDict.TryGetValue(bundleName, out var bundleInfo))
                {
                    valids.Add(bundleInfo);
                }
            }

            bundles = valids;
        }

        public string AssetNameToBundleName(string assetName)
        {
            _bundleAssetMap.TryGetValue(assetName, out var bundleName);
            return bundleName;
        }

        public BundleInfo GetBundle(string bundleName)
        {
            _bundleDict.TryGetValue(bundleName, out var bundleInfo);
            return bundleInfo;
        }
        
    }
}