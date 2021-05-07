
using UnityEditor;
using UnityEngine;
using UnityEngine.UIElements;

namespace GameApp.AnimatorBehaviour
{
    public abstract class AnimatorStateOverrideView : ExpandableView
    {
        protected AnimatorStateOverrideView(AnimatorStateOverride stateOverride)
        {
            var enabledField = new Toggle
            {
                value = stateOverride.enabled
            };
            {
                enabledField.ElementAt(0).style.marginTop = 0;
                enabledField.RegisterValueChangedCallback(e =>
                {
                    stateOverride.enabled = e.newValue;
                    Save(stateOverride);
                });
            }
            
            head.Add(enabledField);
            
            var nameField = new Label(stateOverride.stateName);
            {
                nameField.style.minWidth = 135;
            }
            head.Add(nameField);
        }

        protected void Save(Object obj)
        {
            EditorUtility.SetDirty(obj);
            AssetDatabase.SaveAssets();
        }
    }
}