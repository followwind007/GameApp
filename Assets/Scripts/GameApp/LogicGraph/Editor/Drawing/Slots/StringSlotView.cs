using UnityEngine.UIElements;

namespace GameApp.LogicGraph
{
    public class StringSlotView : BaseSlotView
    {
        protected override void CreateElement()
        {
            this.AddStyleSheetPath("Styles/Slots/StringSlotView");
            var field = new TextField {value = (string) GetValue()};
            field.RegisterValueChangedCallback(evt => SetValue(evt.newValue));
            Add(field);
        }
    }
}