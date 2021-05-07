#if UNITY_EDITOR
using System;
using System.Collections.Generic;

namespace GameApp.Assets
{
    public class AssetDatabaseProvider : AssetProvider
    {
        private readonly Dictionary<string, LoadFromAsset> _assetCache = new Dictionary<string, LoadFromAsset>();
        private readonly Dictionary<string, LoadSceneEditor> _sceneCache = new Dictionary<string, LoadSceneEditor>();
        
        public override bool IsAssetValid(string name)
        {
            return true;
        }

        public override AsyncOperationBase GetAsset(string name, Type type)
        {
            if (!_assetCache.TryGetValue(name, out var asset))
            {
                asset = new LoadFromAsset(name, type);
                _assetCache.Add(name, asset);
            }

            if (asset.ShouldStart)
            {
                asset.StartAsync();
            }
            
            return asset;
        }

        public override AsyncOperationBase GetScene(string name)
        {
            if (!_sceneCache.TryGetValue(name, out var scene))
            {
                scene = new LoadSceneEditor(name);
                _sceneCache.Add(name, scene);
            }

            if (scene.ShouldStart || !scene.Scene.isLoaded)
            {
                scene.StartAsync();
            }

            return scene;
        }
    }
}
#endif