using UnityEditor.UIElements;
using UnityEngine;
using UnityEngine.UIElements;

namespace GameApp.LogicGraph
{
    public class RectSlotView : BaseSlotView
    {
        protected override void CreateElement()
        {
            this.AddStyleSheetPath("Styles/Slots/RectSlotView");
            var rect = new RectField {value = (Rect) GetValue()};
            rect.RegisterValueChangedCallback(evt => SetValue(evt.newValue));
            Add(rect);
        }
    }
}