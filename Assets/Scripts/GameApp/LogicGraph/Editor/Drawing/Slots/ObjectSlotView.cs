using UnityEditor.UIElements;
using UnityEngine;
using UnityEngine.UIElements;

namespace GameApp.LogicGraph
{
    public class ObjectSlotView : BaseSlotView
    {
        protected override void CreateElement()
        {
            this.AddStyleSheetPath("Styles/Slots/ObjectSlotView");
            var field = new ObjectField{value = (Object)GetValue(), objectType = typeof(Object)};
            field.RegisterValueChangedCallback(evt => SetValue(evt.newValue));
            Add(field);
        }
    }
}