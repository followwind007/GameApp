using System;
using System.Collections.Generic;
using System.Linq;
using UnityEngine;
using UnityEditor.Experimental.GraphView;
using UnityEngine.UIElements;

namespace GameApp.LogicGraph
{
    public class LogicNodeView : Node
    {
        private VisualElement _controlsDivider;
        private VisualElement _controlItems;
        protected VisualElement portInputContainer;
        
        private VisualElement _leafContainer;
        private Toggle _leafTgl;

        protected LogicGraphEditorView editorView;
        
        private IEdgeConnectorListener _connectorListener;

        public LogicNode NodeEditor { get; private set; }

        public LogicNodeView()
        {
            styleSheets.Add(Resources.Load<StyleSheet>("Styles/LogicNodeView"));
        }

        public virtual void Initialize(LogicGraphEditorView view, LogicNode node, IEdgeConnectorListener connectorListener)
        {
            editorView = view;

            _connectorListener = connectorListener;
            NodeEditor = node;
            title = NodeEditor.NodeTitle;
            titleContainer.tooltip = NodeEditor.Summary;

            _leafContainer = new VisualElement { name = "leafContainer" };
            _leafTgl = new Toggle { value = NodeEditor.hideLeaf };
            _leafTgl.RegisterValueChangedCallback(evt =>
            {
                NodeEditor.hideLeaf = evt.newValue;
                RefreshLeafVisible(!NodeEditor.hideLeaf);
            });
            _leafContainer.Add(_leafTgl);
            titleButtonContainer.Insert(0, _leafContainer);
            _leafContainer.visible = false;

            var contents = this.Q("contents");

            var controlsContainer = new VisualElement {name = "controls"};
            {
                _controlsDivider = new VisualElement {name = "divider"};
                _controlsDivider.AddToClassList("horizontal");
                controlsContainer.Add(_controlsDivider);
                _controlItems = new VisualElement {name = "items"};
                controlsContainer.Add(_controlItems);
            }
            contents.Add(controlsContainer);
            
            portInputContainer = new VisualElement
            {
                name = "portInputContainer",
                style = { overflow = Overflow.Hidden },
                pickingMode = PickingMode.Ignore
            };
            Add(portInputContainer);

            AddSlots(node.portSlots);
            UpdateInputs();
            portInputContainer.SendToBack();
            SetPosition(new Rect(node.position.x, node.position.y, 0, 0));

            RefreshExpandedState();
        }
        
        public override void BuildContextualMenu(ContextualMenuPopulateEvent evt)
        {
            if (evt.target is Node)
            {
                evt.menu.AppendAction("Hide Leaf", HideLeaf, action => NodeEditor.hideLeaf ? 
                    DropdownMenuAction.Status.Disabled : DropdownMenuAction.Status.Normal);
            }
            base.BuildContextualMenu(evt);
        }

        public List<LogicNodeView> GetConnectedNodes(Direction direct)
        {
            var list = new List<LogicNodeView>();
            var container = direct == Direction.Input ? inputContainer : outputContainer;
            
            foreach (var portView in container.Children().OfType<LogicPortView>())
            {
                foreach (var edge in portView.ConnectedEdge)
                {
                    var linkPort = edge.input == portView ? edge.output : edge.input as LogicPortView;
                    if (linkPort != null)
                    {
                        var node = linkPort.node as LogicNodeView;
                        list.Add(node);
                    }
                }
            }
            return list;
        }

        private void HideLeaf(DropdownMenuAction action)
        {
            _leafTgl.value = true;
        }

        public void RefreshExpand()
        {
            expanded = NodeEditor.expanded;
        }

        private void AddSlots(IEnumerable<LogicPortSlot> slots)
        {
            foreach (var slot in slots)
            {
                var port = LogicPortView.Create(this, slot, _connectorListener);
                if (slot.IsOutputSlot)
                    outputContainer.Add(port);
                else if (slot.IsInputSlot)
                    inputContainer.Add(port);
            }
        }

        private void UpdateInputs()
        {
            foreach (var port in inputContainer.Children().OfType<LogicPortView>())
            {
                if (!portInputContainer.Children().OfType<LogicPortInputView>().Any(a => Equals(a.portView, port)))
                {
                    if (port.PortSlot.IsScheduleSlot) continue;
                    var portInputView = new LogicPortInputView(port) { style = { position = Position.Absolute } };
                    port.InputView = portInputView;
                    portInputContainer.Add(portInputView);
                    port.RegisterCallback<GeometryChangedEvent>(evt => UpdatePortInput((LogicPortView)evt.target));
                }
            }
        }
        
        private void UpdatePortInputVisibilty()
        {
            foreach (var portInputView in portInputContainer.Children().OfType<LogicPortInputView>())
            {
                portInputView.visible = visible && expanded && portInputView.portView.ConnectedEdge.Count < 1;
                portInputContainer.MarkDirtyRepaint();
            }
        }

        private void UpdatePortInput(LogicPortView portView)
        {
            var inputView = portInputContainer.Children().OfType<LogicPortInputView>().First(x => Equals(x.portView, portView));

            var currentRect = new Rect(inputView.resolvedStyle.left, inputView.resolvedStyle.top, inputView.resolvedStyle.width, inputView.resolvedStyle.height);
            var targetRect = new Rect(0.0f, 0.0f, portView.layout.width, portView.layout.height);
            targetRect = portView.ChangeCoordinatesTo(inputView.hierarchy.parent, targetRect);
            var centerY = targetRect.center.y;
            var centerX = targetRect.xMax - currentRect.width;
            currentRect.center = new Vector2(centerX, centerY);

            inputView.style.top = currentRect.yMin;
            var newHeight = inputView.parent.Children().Aggregate(
                inputView.parent.layout.height, 
                (current, element) => Mathf.Max(current, element.style.top.value.value + element.layout.height));

            if (Math.Abs(inputView.parent.style.height.value.value - newHeight) > 1e-3)
                inputView.parent.style.height = newHeight;
        }

        public void RefreshLeafState()
        {
            _leafContainer.visible = NodeEditor.hideLeaf;
            if (NodeEditor.hideLeaf)
            {
                RefreshLeafVisible(false);
            }
        }

        private void RefreshLeafVisible(bool leafVisible)
        {
            _leafContainer.visible = NodeEditor.hideLeaf;
            var leafs = editorView.GraphView.GetNodeLeaf(this);
            foreach (var node in leafs)
            {
                node.visible = leafVisible;
                node.UpdatePortInputVisibilty();
                var portViews = new List<LogicPortView>();
                portViews.AddRange(node.inputContainer.Children().OfType<LogicPortView>());
                portViews.AddRange(node.outputContainer.Children().OfType<LogicPortView>());
                foreach (var p in portViews)
                {
                    foreach (var edge in p.ConnectedEdge)
                        edge.visible = leafVisible;           
                }
            }
        }

        public override bool expanded
        {
            get { return base.expanded; }
            set
            {
                if (base.expanded != value)
                    base.expanded = value;

                NodeEditor.expanded = value;
                RefreshExpandedState();
                UpdatePortInputVisibilty();
            }
        }

    }
}