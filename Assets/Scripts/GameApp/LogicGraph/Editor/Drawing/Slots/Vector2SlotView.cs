using UnityEditor.UIElements;
using UnityEngine;
using UnityEngine.UIElements;

namespace GameApp.LogicGraph
{
    public class Vector2SlotView : BaseSlotView
    {
        protected override void CreateElement()
        {
            this.AddStyleSheetPath("Styles/Slots/Vector2SlotView");
            var vec2 = new Vector2Field {value = (Vector2) GetValue()};
            vec2.RegisterValueChangedCallback(evt => SetValue(evt.newValue));
            Add(vec2);
        }
    }
}