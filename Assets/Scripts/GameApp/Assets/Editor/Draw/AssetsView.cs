using UnityEditor;
using UnityEditor.UIElements;
using UnityEngine.UIElements;

namespace GameApp.Assets
{
    public class AssetsView : VisualElement
    {
        public AssetsView()
        {
            style.flexGrow = 1;
            
            var bs = BuildSettings.Instance;
            var bsObj = new SerializedObject(bs);
            this.Bind(bsObj);
            
            Add(new PropertyField(bsObj.FindProperty("playerSettings")));
            Add(new PropertyField(bsObj.FindProperty("bundleSettings")));
            Add(new PropertyField(bsObj.FindProperty("bundleCache")));

            var bundle = bs.bundleSettings;
            var bundleObj = new SerializedObject(bundle);
            this.Bind(bundleObj);
            
            Add(new PropertyField(bundleObj.FindProperty("context")));

            var sp = new VisualElement();
            sp.style.flexGrow = 1;
            Add(sp);

            var btnBuildAll = new Button(BuildEntry.BuildCurrentAll) {text = "Build All"};
            Add(btnBuildAll);
            
            var btnBuildAssets = new Button(BuildEntry.BuildCurrentAsset) {text = "Build Assets"};
            Add(btnBuildAssets);

            var btnBuildPlayer = new Button(BuildEntry.BuildCurrentPlayer) {text = "Build Player"};
            Add(btnBuildPlayer);
            
            var btnRunPlayer = new Button(BuildEntry.RunCurrentPlayer) {text = "Run Player"};
            Add(btnRunPlayer);
        }
    }
}