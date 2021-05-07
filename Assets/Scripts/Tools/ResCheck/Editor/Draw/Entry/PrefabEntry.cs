using UnityEngine.UIElements;

namespace Tools.ResCheck
{
    public class PrefabEntry : VisualElement
    {
        public PrefabEntry(DataSource.PrefabInfo prefab)
        {
            this.AddStyleSheetPath("ResCheck/Styles/PrefabEntry");
            
            this.AddManipulator(new EntryManipulator(prefab.path));
            
            var content = new VisualElement { name = "content" };
            Add(content);
            
            var labelName = new Label { text = prefab.path };
            content.Add(labelName);
            
            if (prefab.particles.Count > 0)
            {
                var contentParticle = new VisualElement { name = "contentSub1" };
                content.Add(contentParticle);
                
                var labelDesc = new Label { name = "labelDesc", text = "Particle Systems:" };
                contentParticle.Add(labelDesc);
                foreach (var particle in prefab.particles)
                {
                    var label = new Label { name = "labelParticle", text = particle};
                    contentParticle.Add(label);
                }
            }

        }
        
    }
}