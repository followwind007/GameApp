using System;

namespace GameApp.Assets
{
    public abstract class AssetProvider
    {
        public abstract bool IsAssetValid(string name);

        public abstract AsyncOperationBase GetAsset(string name, Type type);
        
        public abstract AsyncOperationBase GetScene(string name);

        public virtual AsyncOperationBase Init()
        {
            return default;
        }

    }
}