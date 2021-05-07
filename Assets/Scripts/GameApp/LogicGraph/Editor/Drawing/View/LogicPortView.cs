using System;
using System.Collections.Generic;
using UnityEditor.Experimental.GraphView;
using UnityEngine.UIElements;
using UnityEngine;

namespace GameApp.LogicGraph
{
    public sealed class LogicPortView : Port
    {
        private readonly CustomStyleProperty<Color> _portColorProperty = new CustomStyleProperty<Color>("--port-color");
        public LogicPortSlot PortSlot
        {
            get { return _portSlot; }
            set
            {
                if (ReferenceEquals(value, _portSlot))
                    return;
                if (value == null)
                    throw new NullReferenceException();
                if (_portSlot != null && value.IsInputSlot != _portSlot.IsInputSlot)
                    throw new ArgumentException("Cannot change direction of already created port");
                _portSlot = value;
                Refresh();
            }
        }

        public List<Edge> ConnectedEdge { get; set; }
        public LogicNodeView NodeView { get; set; }
        public LogicPortInputView InputView { get; set; }

        private LogicPortSlot _portSlot;
        
        public static Port Create(LogicNodeView nodeView, LogicPortSlot slot, IEdgeConnectorListener connectorListener)
        {
            var port = new LogicPortView(Orientation.Horizontal, 
                slot.IsInputSlot ? Direction.Input : Direction.Output,
                slot.IsInputSlot ? Capacity.Single : Capacity.Multi,
                null, slot)
            {
                m_EdgeConnector = new EdgeConnector<Edge>(connectorListener),
            };
            port.AddManipulator(port.m_EdgeConnector);
            port.ConnectedEdge = new List<Edge>();
            port.NodeView = nodeView;
            return port;
        }
        
        private LogicPortView(Orientation portOrientation, Direction portDirection, Capacity portCapacity, Type type, LogicPortSlot slot)
            : base(portOrientation, portDirection, portCapacity, type)
        {
            this.AddStyleSheetPath("Styles/LogicPortView");
            PortSlot = slot;
            RegisterCallback<CustomStyleResolvedEvent>(OnCustomStyleResolved);
        }
        
        public void AddEdge(Edge edge)
        {
            ConnectedEdge.Add(edge);
            PortSlot.onValueChange?.Invoke(PortSlot);
        }

        public void RemoveEdge(Edge edge)
        {
            ConnectedEdge.Remove(edge);
            PortSlot.onValueChange?.Invoke(PortSlot);
        }

        public void Refresh()
        {
            portName = PortSlot.DisplayName;
            tooltip = PortSlot.ToolTip;
            EnableInClassList("port", false);
            visualClass = $"type{PortSlot.DisplayValueType}";
            m_ConnectorText.AddToClassList($"type{PortSlot.DisplayValueType}");
        }

        private void OnCustomStyleResolved(CustomStyleResolvedEvent e)
        {
            if (e.customStyle.TryGetValue(_portColorProperty, out var c))
            {
                portColor = c;
            }
        }

    }
}
