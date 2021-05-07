#if UNITY_EDITOR
using UnityEditor;

namespace GameApp.Assets
{
    [FilePath("ProjectSettings/AssetSettings.asset", FilePathAttribute.Location.ProjectFolder)]
    public class AssetSettings : ScriptableSingleton<AssetSettings>
    {
        public bool useAssetBundle;

        public bool useVirtualBundle;

        public bool autoStartPlayer;

        private void OnDisable()
        {
            Save();
        }

        public void Save()
        {
            Save(true);
        }
        
        public SerializedObject GetSerializedObject()
        {
            return new SerializedObject(this);
        }
    }
}
#endif