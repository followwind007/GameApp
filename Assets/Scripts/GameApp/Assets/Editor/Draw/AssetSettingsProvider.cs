using System.Collections.Generic;
using UnityEditor;
using UnityEditor.UIElements;
using UnityEngine.UIElements;

namespace GameApp.Assets
{
    public class AssetSettingsProvider : SettingsProvider
    {
        [SettingsProvider]
        private static SettingsProvider CreateAssetsSettingProvider()
        {
            return new AssetSettingsProvider("Project/Assets", SettingsScope.Project, 
                GetSearchKeywordsFromSerializedObject(AssetSettings.instance.GetSerializedObject()));
        }

        public AssetSettingsProvider(string path, SettingsScope scopes, IEnumerable<string> keywords = null) : base(path, scopes, keywords) { }

        public override void OnActivate(string searchContext, VisualElement rootElement)
        {
            AssetSettings.instance.Save();
            var settings = AssetSettings.instance;

            var serObj = new SerializedObject(settings);
            rootElement.Bind(serObj);

            var iter = serObj.GetIterator();
            iter.NextVisible(true);
            iter.NextVisible(false);
            do
            {
                rootElement.Add(new PropertyField(iter));
            } while (iter.NextVisible(false));
        }

        public override void OnDeactivate()
        {
            AssetSettings.instance.Save();
        }
    }
}