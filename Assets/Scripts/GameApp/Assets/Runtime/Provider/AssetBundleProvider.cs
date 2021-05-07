using System;
using System.Collections.Generic;

namespace GameApp.Assets
{
    public partial class AssetBundleProvider : AssetProvider
    {
        public static AssetBundleProvider Instance { get; private set; }

        private readonly Dictionary<string, LoadAssetBundle> _bundleCache = new Dictionary<string, LoadAssetBundle>();
        private readonly Dictionary<string, GroupParallel> _groupCache = new Dictionary<string, GroupParallel>();
        private readonly Dictionary<AssetLocation, LoadFromBundle> _assetCache = new Dictionary<AssetLocation, LoadFromBundle>();
        private readonly Dictionary<string, LoadSceneRuntime> _sceneCache = new Dictionary<string, LoadSceneRuntime>();

        private readonly Dictionary<string, string> _assetBundleMap = new Dictionary<string, string>
        {
            {AssetConfig.ManifestName, AssetConfig.ManifestBundle}
        };

        private AssetManifest _manifest;

        private bool _inited;

        public AssetBundleProvider()
        {
            Instance = this;
        }

        public override bool IsAssetValid(string name)
        {
            var bundleName = AssetNameToBundleName(name);
            return !string.IsNullOrEmpty(bundleName);
        }
        
        public override AsyncOperationBase Init()
        {
            var bundle = GetBundleCache(AssetConfig.ManifestBundle);
            var asset = new LoadFromBundle(AssetConfig.ManifestName, typeof(AssetManifest), bundle);
            _assetCache.Add(new AssetLocation(AssetConfig.ManifestName, typeof(AssetManifest)), asset);
            asset.OnComplete += () =>
            {
                _manifest = asset.Result as AssetManifest;
                if (_manifest != null)
                {
                    _manifest.Init();
                    _inited = true;
                }
            };
            return asset;
        }

        public override AsyncOperationBase GetScene(string name)
        {
            if (!_sceneCache.TryGetValue(name, out var scene))
            {
                var group = GetGroupCache(name);
            
                scene = new LoadSceneRuntime(name, group);
                _sceneCache.Add(name, scene);
            }
            
            if (scene.ShouldStart)
            {
                scene.StartAsync();
            }

            return scene;
        }

        public override AsyncOperationBase GetAsset(string name, Type type)
        {
            var location = new AssetLocation(name, type);
            if (!_assetCache.TryGetValue(location, out var asset))
            {
                var group = GetGroupCache(name);

                //load asset
                asset = new LoadFromBundle(name, type, group);
                _assetCache.Add(location, asset);
            }
            
            if (asset.ShouldStart)
            {
                asset.StartAsync();
            }
            
            return asset;
        }

        public LoadAssetBundle GetBundle(string bundleName)
        {
            _bundleCache.TryGetValue(bundleName, out var bundle);
            return bundle;
        }

        public AssetManifest.BundleInfo GetBundleInfo(string bundleName)
        {
            return _manifest != null ? _manifest.GetBundle(bundleName) : null;
        }

        public string AssetNameToBundleName(string assetName)
        {
            var bundleName = _inited ? _manifest.AssetNameToBundleName(assetName) : null;
            
            if (string.IsNullOrEmpty(bundleName))
            {
                _assetBundleMap.TryGetValue(assetName, out bundleName);
            }
            
            return bundleName;
        }

        private GroupParallel GetGroupCache(string name)
        {
            var bundleName = AssetNameToBundleName(name);
            if (!_groupCache.TryGetValue(bundleName, out var group))
            {
                var depCount = 1;
                
                var bundleInfo = _manifest.GetBundle(bundleName);
                var deps = bundleInfo.deps;
                depCount += deps.Length;

                var loads = new List<AsyncOperationBase>(depCount); 
                
                var bundle = GetBundleCache(bundleName);
                loads.Add(bundle);

                foreach (var dep in deps)
                {
                    var depBundle = GetBundleCache(dep);
                    loads.Add(depBundle);
                }

                group = new GroupParallel(loads);
                _groupCache.Add(bundleName, group);
            }

            return group;
        }
        
        private LoadAssetBundle GetBundleCache(string bundleName)
        {
            if (!_bundleCache.TryGetValue(bundleName, out var bundle))
            {
                bundle = new LoadAssetBundle(bundleName);
                _bundleCache.Add(bundleName, bundle);
            }
            return bundle;
        }
        
    }
}