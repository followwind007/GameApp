using GameApp.DataBinder;
using UnityEngine.UIElements;
using UnityEngine;
using ValueType = GameApp.DataBinder.ValueType;

namespace GameApp.LogicGraph
{
    public class PropertyBoardFieldView : VisualElement
    {
        public PropertyBoardFieldView(PropertyBoardProvider provider, GraphProperty prop)
        {
            styleSheets.Add(Resources.Load<StyleSheet>("Styles/PropertyBoardFieldView"));
            BaseSlotView slot = null;
            var wrap = provider.GraphObject.binds.GetWrap(prop.name);
            switch (wrap.type)
            {
                case ValueType.Int:
                    slot = new IntSlotView();
                    break;
                case ValueType.Float:
                    slot = new FloatSlotView();
                    break;
                case ValueType.String:
                    slot = new StringSlotView();
                    break;
                case ValueType.Vector2:
                    slot = new Vector2SlotView();
                    break;
                case ValueType.Vector3:
                    slot = new Vector3SlotView();
                    break;
                case ValueType.Vector4:
                    slot = new Vector4SlotView();
                    break;
                case ValueType.Rect:
                    slot = new RectSlotView();
                    break;
                case ValueType.Bounds:
                    slot = new BoundsSlotView();
                    break;
                case ValueType.Color:
                    slot = new ColorSlotView();
                    break;
                case ValueType.Curve:
                    slot = new CurveSlotView();
                    break;
                case ValueType.Bool:
                    slot = new BooleanSlotView();
                    break;
                case ValueType.Object:
                    slot = new ObjectSlotView();
                    break;
            }

            if (slot != null)
            {
                slot.binds = provider.GraphEditor.GraphObject.binds;
                slot.wrap = wrap;
                slot.ownerLogicGraphObject = provider.GraphObject;
                slot.Create();
                Add(slot);
            }

            if (provider.GraphObject.type == LogicGraphObject.GraphType.State)
            {
                var tglListen = new Toggle {text = "Listen", value = prop.isListen};
                tglListen.RegisterValueChangedCallback(evt => prop.isListen = evt.newValue);
                Add(tglListen);
                
                var tglTrigger = new Toggle {text = "Trigger", value = prop.isTrigger};
                tglTrigger.RegisterValueChangedCallback(evt => prop.isTrigger = evt.newValue);
                Add(tglTrigger); 
            }
        }
    }
}