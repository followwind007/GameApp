using System;
using UnityEditor.Experimental.GraphView;
using UnityEngine.UIElements;
using UnityEngine;
using ValueType = GameApp.DataBinder.ValueType;

namespace GameApp.LogicGraph
{
    public class LogicPortInputView : GraphElement, IDisposable
    {
        public BaseSlotView slotView;
        private readonly CustomStyleProperty<Color> _edgeColorProperty = new CustomStyleProperty<Color>("--edge-color");

        public readonly LogicPortView portView;

        public readonly LogicPortSlot portSlot;
        
        private readonly EdgeControl _edgeControl;
        public readonly VisualElement container;

        public Color EdgeColor { get; private set; }

        public LogicPortInputView(LogicPortView portView)
        {
            styleSheets.Add(Resources.Load<StyleSheet>("Styles/LogicPortInputView"));
            pickingMode = PickingMode.Ignore;
            ClearClassList();
            
            this.portView = portView;
            portSlot = portView.PortSlot;
            
            AddToClassList("type" + portSlot.DisplayValueType);

            _edgeControl = new EdgeControl
            {
                from = new Vector2(412f - 21f, 11.5f),
                to = new Vector2(412f, 11.5f),
                edgeWidth = 2,
                pickingMode = PickingMode.Ignore
            };
            Add(_edgeControl);
            
            container = new VisualElement { name = "container" };
            {
                AddSlotView();

                var slotElement = new VisualElement { name = "slot" };
                {
                    slotElement.Add(new VisualElement { name = "dot" });
                }
                container.Add(slotElement);
            }
            Add(container);
            RegisterCallback<CustomStyleResolvedEvent>(OnCustomStyleResolved);
        }

        protected void OnCustomStyleResolved(CustomStyleResolvedEvent e)
        {
            if (e.customStyle.TryGetValue(_edgeColorProperty, out var c))
            {
                EdgeColor = c;
            }

            _edgeControl.UpdateLayout();
            _edgeControl.inputColor = EdgeColor;
            _edgeControl.outputColor = EdgeColor;
        }

        public void Dispose()
        {
            
        }
        
        public BaseSlotView InstantiateSlot()
        {
            BaseSlotView view;
            var s = portView.PortSlot;
            switch (s.ValType)
            {
                case "boolean":
                    view = new BooleanSlotView {Type = ValueType.Bool};
                    break;
                case "number":
                    view = new FloatSlotView {Type = ValueType.Float};
                    break;
                case "integer":
                    view = new IntSlotView {Type = ValueType.Int};
                    break;
                case "string":
                    view = new StringSlotView {Type = ValueType.String};
                    break;
                case "Elua":
                    view = new StringSlotView {Type = ValueType.String};
                    break;
                case "UnityEngine.Vector2":
                    view = new Vector2SlotView {Type = ValueType.Vector2};
                    break;
                case "UnityEngine.Vector3":
                    view = new Vector3SlotView {Type = ValueType.Vector3};
                    break;
                case "UnityEngine.Vector4":
                    view = new Vector4SlotView {Type = ValueType.Vector4};
                    break;
                case "UnityEngine.Rect":
                    view = new RectSlotView {Type = ValueType.Rect};
                    break;
                case "UnityEngine.Color":
                    view = new ColorSlotView {Type = ValueType.Color};
                    break;
                case "UnityEngine.AnimationCurve":
                    view = new CurveSlotView {Type = ValueType.Curve};
                    break;
                case "fun":
                    view = new StringSlotView {Type = ValueType.String};
                    break;
                default:
                    view = new ObjectSlotView {Type = ValueType.Object};
                    break;
            }

            view.Init(s);
            view.Create();

            if (s.owner.ShortType == LogicGraphSettings.NodeType.Variable)
            {
                view.SetEnabled(false);
            }

            return view;
        }

        public void AddSlotView(BaseSlotView slot = null)
        {
            if (slot == null)
            {
                slot = InstantiateSlot();
            }
            slotView = slot;
            container.Insert(0, slotView);
        }

        public void RemoveSlotView(bool removeValue = true)
        {
            container.Remove(slotView);
            if (removeValue)
            {
                slotView.PortSlot.RemoveValue();
            }
            slotView = null;
        }

    }
}