using UnityEngine.UIElements;

namespace GameApp.LogicGraph
{
    public class BooleanSlotView : BaseSlotView
    {
        protected override void CreateElement()
        {
            this.AddStyleSheetPath("Styles/Slots/BooleanSlotView");
            var toggle = new Toggle {value = (bool) GetValue()};
            toggle.RegisterValueChangedCallback(evt => SetValue(evt.newValue));
            Add(toggle);
        }
    }
}