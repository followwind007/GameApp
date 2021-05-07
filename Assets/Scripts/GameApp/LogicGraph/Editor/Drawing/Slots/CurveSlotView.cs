using UnityEditor.UIElements;
using UnityEngine;
using UnityEngine.UIElements;

namespace GameApp.LogicGraph
{
    public class CurveSlotView : BaseSlotView
    {
        protected override void CreateElement()
        {
            this.AddStyleSheetPath("Styles/Slots/CurveSlotView");
            var field = new CurveField {value = (AnimationCurve)GetValue()};
            field.RegisterValueChangedCallback(evt => SetValue(evt.newValue));
            Add(field);
        }
    }
}