using UnityEditor;
using UnityEngine;
using UnityEngine.UIElements;

namespace Tools.ResCheck
{
    public class AtlasEntry : VisualElement
    {
        private readonly ResCheckSettings.TextureConfig _atlasConfig = ResCheckSettings.AtlasConfig;

        private const string FormatNormal = "formatNormal";
        private const string FormatError = "formatError";
        public AtlasEntry(AtlasChecker.AtlasItem item)
        {
            this.AddStyleSheetPath("ResCheck/Styles/AtlasEntry");
            var img = new Image { name = "img" };
            var tex = AssetDatabase.LoadAssetAtPath<Texture2D>(item.FilePath);
            img.style.backgroundImage = tex;
            Add(img);
            
            var content = new VisualElement { name = "content" };
            Add(content);

            var labelName = new Label { text = item.file.Name };
            content.Add(labelName);
            
            var detail = new VisualElement { name = "detail" };
            content.Add(detail);
            
            var labelRef = new Label
            {
                name = item.ReferenceCount > 0 ? FormatNormal : FormatError,
                text = $"引用:{item.ReferenceCount}",
            };
            detail.Add(labelRef);
            
            var labelSize = new Label
            {
                name = IsSizeValid(new Vector2Int(tex.width, tex.height))? FormatNormal : FormatError,
                text = $"({tex.width}, {tex.height})",
            };
            detail.Add(labelSize);
            
            this.AddManipulator(new EntryManipulator(item.FilePath, item));
        }

        private bool IsSizeValid(Vector2Int size)
        {
            return size.x <= _atlasConfig.maxSize.x && 
                   size.y <= _atlasConfig.maxSize.y && 
                   size.x % 2 == 0 && 
                   size.y % 2 == 0;
        }
        
    }
    
}