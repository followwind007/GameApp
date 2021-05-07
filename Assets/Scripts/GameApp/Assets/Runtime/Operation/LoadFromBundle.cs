
using System;

namespace GameApp.Assets
{
    public class LoadFromBundle : AppendOperation
    {
        private readonly Type _type;

        /// <summary>
        /// not finish and not running or success but unloaded
        /// </summary>
        public override bool ShouldStart => !IsRunning && !IsFinish || IsSuccess && !HasResult;

        public LoadFromBundle(string name, Type type, AsyncOperationBase prevOperation = null) : base(name, prevOperation)
        {
            _type = type;
        }

        protected override void LoadAsset()
        {
#if UNITY_EDITOR
            if (AssetSettings.instance.useVirtualBundle)
            {
                Result = UnityEditor.AssetDatabase.LoadAssetAtPath(name, _type);
                if (Result != null)
                    OnSuccess();
                else
                    OnError();

                return;
            }
#endif
            var bundleName = Provider.AssetNameToBundleName(name);

            var bundleOp = Provider.GetBundle(bundleName);
            var bundle = bundleOp.Bundle;
            var req = bundle.LoadAssetAsync(name, _type);
            req.allowSceneActivation = true;
            req.completed += operation =>
            {
                Result = req.asset;
                if (Result != null)
                    OnSuccess();
                else
                    OnError();
            };
        }
        
    }
}