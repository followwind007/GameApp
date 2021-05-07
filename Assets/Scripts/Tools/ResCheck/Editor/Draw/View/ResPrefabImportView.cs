using UnityEngine.UIElements;

namespace Tools.ResCheck
{
    public class ResPrefabImportView : VisualElement
    {
        public ResPrefabImportView()
        {
            var content = new VisualElement {name = "content"};
            Add(content);

            var setting = PrefabImportSetting.Instance;
            foreach (var item in setting.copyItems)
            {
                var entry = new PrefabImportEntry(item);
                content.Add(entry);
            }
        }
    }
}