#if UNITY_EDITOR
using System.Collections.Generic;

// ReSharper disable ConvertToAutoProperty
namespace GameApp.Assets
{
    public partial class AssetBundleProvider
    {
        public Dictionary<string, LoadAssetBundle> BundleCache => _bundleCache;
        public Dictionary<string, GroupParallel> GroupCache => _groupCache;
        public Dictionary<AssetLocation, LoadFromBundle> AssetCache => _assetCache;
        public Dictionary<string, LoadSceneRuntime> SceneCache => _sceneCache;
    }
}
#endif