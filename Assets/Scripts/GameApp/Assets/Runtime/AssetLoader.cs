using System;
using UnityEngine.SceneManagement;

namespace GameApp.Assets
{
    public static class AssetLoader
    {
        private static readonly AssetLoaderImpl Impl = new AssetLoaderImpl();

        public static void Init(Action onFinish = null)
        {
            var req = Impl.InitAsync();
            req.OnComplete += onFinish;
        }
        
        public static AssetLoadHandle<TObject> LoadAssetAsync<TObject>(string name)
        {
            return Impl.LoadAssetAsync<TObject>(name);
        }

        public static AssetLoadHandle<Scene> LoadSceneAsync(string name)
        {
            return Impl.LoadSceneAsync(name);
        }
    }
}