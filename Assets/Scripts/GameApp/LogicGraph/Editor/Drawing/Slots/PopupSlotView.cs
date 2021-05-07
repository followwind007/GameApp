using System.Collections.Generic;
using UnityEditor.UIElements;
using UnityEngine.UIElements;

namespace GameApp.LogicGraph
{
    public class PopupSlotView<T> : BaseSlotView
    {
        public readonly List<T> pops;
        public PopupSlotView(List<T> pops)
        {
            this.pops = pops;
        }
        
        protected override void CreateElement()
        {
            this.AddStyleSheetPath("Styles/Slots/PopupSlotView");
            var value = (T)GetValue();
            var idx = 0;
            
            for (var i = 0; i < pops.Count; i++)
            {
                if (pops[i].Equals(value)) idx = i;
            }
            var field = new PopupField<T>(pops, idx);
            field.RegisterValueChangedCallback(evt => { SetValue(evt.newValue); });
            
            Add(field);
        }
    }
}