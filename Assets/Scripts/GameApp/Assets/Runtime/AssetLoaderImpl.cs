using System;
using System.Collections.Generic;
using UnityEngine.SceneManagement;

namespace GameApp.Assets
{
    internal class AssetLoaderImpl
    {
        private List<AssetProvider> _providers;
        public AssetLoaderImpl()
        {
            GetProvider();
        }

        public AsyncOperationBase InitAsync()
        {
            var ops = new List<AsyncOperationBase>();
            foreach (var provider in _providers)
            {
                var op = provider.Init();
                if (op != null) ops.Add(op);
            }

            var group = new GroupParallel(ops);
            group.Increase();

            group.OnComplete += () => { group.Decrease(); };
            group.StartAsync();

            return group;
        }

        public AssetLoadHandle<TObject> LoadAssetAsync<TObject>(string name)
        {
            var provider = GetProvider(name);
            var op = provider.GetAsset(name, typeof(TObject));
            var handle = new AssetLoadHandle<TObject>(op);
            return handle;
        }

        public AssetLoadHandle<Scene> LoadSceneAsync(string name)
        {
            var provider = GetProvider(name);
            var op = provider.GetScene(name);
            var handle = new AssetLoadHandle<Scene>(op);
            return handle;
        }

        private void GetProvider()
        {
#if UNITY_EDITOR
            _providers = new List<AssetProvider>();
            if (AssetSettings.instance.useAssetBundle)
            {
                _providers.Add(new AssetBundleProvider());
            }
            else
            {
                _providers.Add(new AssetDatabaseProvider());
            }
#else
            _providers = new List<AssetProvider> {new AssetBundleProvider()};
#endif
        }

        private AssetProvider GetProvider(string assetName)
        {
            foreach (var provider in _providers)
            {
                if (provider.IsAssetValid(assetName))
                {
                    return provider;
                }
            }

            throw new Exception($"Can not find any valid provider for asset:{assetName}");
        }
        
    }
}