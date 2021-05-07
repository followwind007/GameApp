using UnityEditor.UIElements;
using UnityEngine.UIElements;

namespace GameApp.LogicGraph
{
    public class IntSlotView : BaseSlotView
    {
        protected override void CreateElement()
        {
            this.AddStyleSheetPath("Styles/Slots/IntSlotView");
            var field = new IntegerField {value = (int) GetValue()};
            field.RegisterValueChangedCallback(evt => SetValue(evt.newValue));
            Add(field);
        }
    }
}