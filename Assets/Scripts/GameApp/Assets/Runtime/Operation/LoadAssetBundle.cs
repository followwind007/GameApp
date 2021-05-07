using UnityEngine;

namespace GameApp.Assets
{
    public class LoadAssetBundle : AsyncOperationBase
    {
        public AssetBundle Bundle => Result as AssetBundle;

        private readonly string _name;

        public LoadAssetBundle(string name)
        {
            _name = name;
        }
        
        public override void StartAsync()
        {
            base.StartAsync();
            
#if UNITY_EDITOR
            if (AssetSettings.instance.useVirtualBundle)
            {
                OnSuccess();
                return;
            }
#endif

            var bundleInfo = AssetBundleProvider.Instance.GetBundleInfo(_name);
            AssetBundleCreateRequest req;
            if (bundleInfo != null && bundleInfo.gidx > 0)
            {
                var path = AssetConfig.GetGroupPath(bundleInfo.gidx);
                req = AssetBundle.LoadFromFileAsync(path, bundleInfo.crc, bundleInfo.offset);
            }
            else
            {
                var path = AssetConfig.GetBundlePath(_name);
                req = AssetBundle.LoadFromFileAsync(path);
            }

            req.completed += operation =>
            {
                Result = req.assetBundle;
                if (Bundle != null)
                    OnSuccess();
                else
                    OnError();
            };
        }

        public override void Release()
        {
#if UNITY_EDITOR
            if (AssetSettings.instance.useVirtualBundle)
            {
                base.Release();
                return;
            }
#endif
            Bundle.Unload(false);
            Reset();
        }
    }
}