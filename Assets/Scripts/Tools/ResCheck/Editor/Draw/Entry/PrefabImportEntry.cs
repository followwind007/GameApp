using UnityEditor.UIElements;
using UnityEngine;
using UnityEngine.UIElements;

namespace Tools.ResCheck
{
    public class PrefabImportEntry : VisualElement
    {
        
        public PrefabImportEntry(CopyItem item)
        {
            var prefab = new ObjectField {name = "", objectType = typeof(GameObject), allowSceneObjects = false};
            prefab.SetEnabled(false);
            Add(prefab);
            
            var labelSavePath = new Label{name = "", text = item.savePath};
            Add(labelSavePath);
            
            var textSaveName = new TextField {name = ""};
            Add(textSaveName);
            
            var btnImport = new Button(() =>
            {
                
            }){text = "Create Prefab"};
            
            Add(btnImport);
        }
    }
}