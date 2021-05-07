using GameApp.DataBinder;
using UnityEditor;
using UnityEngine.UIElements;
using ValueType = GameApp.DataBinder.ValueType;

namespace GameApp.LogicGraph
{
    public abstract class BaseSlotView : VisualElement
    {
        public BindableValues binds;
        public ValueWrap wrap;
        public LogicGraphObject ownerLogicGraphObject;

        public ValueType Type { get; set; }
        
        public LogicPortSlot PortSlot { get; private set; }

        public void Init(LogicPortSlot slot)
        {
            PortSlot = slot;
            ownerLogicGraphObject = slot.owner.GraphObject;
            binds = slot.owner.GraphObject.binds;
            var item = binds.GetWrap(slot.BindKey);
            
            //type missmatch should also create new
            if (item == null || item.type != Type)
            {
                item = new ValueWrap {name = slot.BindKey, type = Type, value = BindableValues.GetDefault(Type)};
                binds.wraps.Add(item);
            }

            wrap = item;
        }

        public void Create()
        {
            CreateElement();
        }

        protected void SetValue(object val)
        {
            Undo.RegisterCompleteObjectUndo(ownerLogicGraphObject,string.Concat("Edit ",Type.ToString()));
            wrap.value = val;
            binds.Regenerate();
            PortSlot?.onValueChange?.Invoke(PortSlot);
        }

        protected object GetValue()
        {
            return wrap.value ?? (wrap.value = BindableValues.GetDefault(wrap.type));
        }

        protected abstract void CreateElement();

    }
}