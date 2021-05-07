
using UnityEngine;
using UnityEngine.AddressableAssets;

namespace ScriptsMine.Test
{
    public class TestAddressable : MonoBehaviour
    {
        public AssetReference reference;

        public void Start()
        {
            var req = reference.LoadAssetAsync<Object>();
            req.WaitForCompletion();
        }
    }
}