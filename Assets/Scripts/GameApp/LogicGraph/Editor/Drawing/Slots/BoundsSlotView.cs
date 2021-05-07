using UnityEditor.UIElements;
using UnityEngine;
using UnityEngine.UIElements;

namespace GameApp.LogicGraph
{
    public class BoundsSlotView : BaseSlotView
    {
        protected override void CreateElement()
        {
            this.AddStyleSheetPath("Styles/Slots/BoundsSlotView");
            var bd = new BoundsField {value = (Bounds) GetValue()};
            bd.RegisterValueChangedCallback(evt => SetValue(evt.newValue));
            Add(bd);
        }
    }
}