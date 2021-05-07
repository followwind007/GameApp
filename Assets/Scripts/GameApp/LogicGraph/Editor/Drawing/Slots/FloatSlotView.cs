using UnityEditor.UIElements;
using UnityEngine.UIElements;

namespace GameApp.LogicGraph
{
    public class FloatSlotView : BaseSlotView
    {
        protected override void CreateElement()
        {
            this.AddStyleSheetPath("Styles/Slots/FloatSlotView");
            var field = new FloatField {value = (float) GetValue()};
            field.RegisterValueChangedCallback(evt => SetValue(evt.newValue));
            Add(field);
        }
        
    }
}