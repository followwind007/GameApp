using UnityEngine.UIElements;

namespace Tools.ResCheck
{
    public class ModuleEntry : VisualElement
    {
        public ModuleEntry(AtlasChecker.AtlasInfo atlas, DataSource.ModuleInfo module)
        {
            this.AddStyleSheetPath("ResCheck/Styles/ModuleEntry");
            var content = new VisualElement { name = "content" };
            Add(content);
            
            var labelName = new Label { text = atlas.name };
            content.Add(labelName);

            var spriteContent = new VisualElement { name = "spriteContent" };
            content.Add(spriteContent);
            foreach (var sprite in module.spriteDict.Values)
            {
                if (sprite.atlasName != atlas.name) continue;
                var atlasEntry = new AtlasEntry(sprite);
                spriteContent.Add(atlasEntry);

                if (!module.spriteRefs.ContainsKey(sprite.name)) continue;
                foreach (var refPath in module.spriteRefs[sprite.name])
                {
                    var labelRef = new Label { text = refPath};
                    spriteContent.Add(labelRef);
                }
            }
        }
        
    }
}