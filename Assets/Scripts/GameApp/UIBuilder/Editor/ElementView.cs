using System.IO;
using UnityEngine;
using UnityEngine.UIElements;

namespace GameApp.UIBuilder
{
    public class ElementView : VisualElement
    {
        public const float BaseAppend = 24f;
        public string prefabPath { get; }

        private readonly Vector2 _size;
        
        public ElementView(FileInfo file)
        {
            styleSheets.Add(Resources.Load<StyleSheet>("UIBuilder/Styles/ElementView"));
            prefabPath = BuilderSettings.GetRelativeAssetPath(file.FullName);
            tooltip = prefabPath;
            
            var tex = PreviewUtil.GetTexture(file);
            if (tex == null)
            {
                tex = Resources.Load<Texture2D>("UIBuilder/Icon/Null");
            }

            _size = PreviewUtil.GetPreviewSize(new Rect(0, 0, tex.width, tex.height));
            
            var content = new VisualElement { name = "content" };
            content.style.backgroundImage = tex;
            
            Add(content);
            
            var labelName = new Label { name = "labelName", text = file.Name.Replace(".prefab", "")};
            Add(labelName);
            
            var manipulator = new ElementManipulator();
            this.AddManipulator(manipulator);
        }

        public void RefreshSize(float scaleFactor)
        {
            var size = _size * scaleFactor;
            style.width = size.x;
            style.height = size.y + BaseAppend;
        }
        
    }
}