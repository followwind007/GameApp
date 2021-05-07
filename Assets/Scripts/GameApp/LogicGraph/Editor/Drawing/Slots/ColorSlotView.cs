using UnityEditor.UIElements;
using UnityEngine;
using UnityEngine.UIElements;

namespace GameApp.LogicGraph
{
    public class ColorSlotView : BaseSlotView
    {
        protected override void CreateElement()
        {
            this.AddStyleSheetPath("Styles/Slots/ColorSlotView");
            var cl = new ColorField {value = (Color) GetValue()};
            cl.RegisterValueChangedCallback(evt => SetValue(evt.newValue));
            Add(cl);
        }
    }
}