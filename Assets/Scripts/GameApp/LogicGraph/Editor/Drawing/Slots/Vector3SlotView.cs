using UnityEditor.UIElements;
using UnityEngine;
using UnityEngine.UIElements;

namespace GameApp.LogicGraph
{
    public class Vector3SlotView : BaseSlotView
    {
        protected override void CreateElement()
        {
            this.AddStyleSheetPath("Styles/Slots/Vector3SlotView");
            var vec3 = new Vector3Field {value = (Vector3) GetValue()};

            vec3.RegisterValueChangedCallback(evt => SetValue(evt.newValue));
            vec3.AddStyleSheetPath("Styles/Slots/Vector3SlotView");
            
            Add(vec3);
        }
    }
}