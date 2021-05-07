using UnityEditor.UIElements;
using UnityEngine;
using UnityEngine.UIElements;

namespace GameApp.LogicGraph
{
    public class Vector4SlotView : BaseSlotView
    {
        protected override void CreateElement()
        {
            this.AddStyleSheetPath("Styles/Slots/Vector4SlotView");
            var vec4 = new Vector4Field {value = (Vector4) GetValue()};
            vec4.RegisterValueChangedCallback(evt => SetValue(evt.newValue));
            Add(vec4);
        }
    }
}