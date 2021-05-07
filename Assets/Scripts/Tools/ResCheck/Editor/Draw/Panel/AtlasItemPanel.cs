using UnityEngine.UIElements;

namespace Tools.ResCheck
{
    public class AtlasItemPanel : VisualElement
    {
        public AtlasItemPanel(AtlasChecker.AtlasItem item)
        {
            this.AddStyleSheetPath("ResCheck/Styles/AtlasItemPanel");
            var labelName = new Label { text = item.FilePath };
            labelName.AddManipulator(new EntryManipulator(item.FilePath));
            Add(labelName);

            foreach (var kv in item.references)
            {
                var labelRef = new Label { text = $"{kv.Key}: {kv.Value}" };
                labelRef.AddManipulator(new EntryManipulator(kv.Key));
                Add(labelRef);
            }
        }
        
    }
}