
using System.Collections.Generic;
using System.Linq;
using GameApp.DataBinder;
using GameApp.LuaResolver;
using UnityEditor.Experimental.GraphView;
using UnityEngine;
using UnityEngine.UIElements;

namespace GameApp.LogicGraph
{
    public class LogicPropertyNodeView : LogicNodeView
    {
        private const string NoneFlag = "none";
        
        private LogicPortView _targetPort;
        private LogicPortView _fieldPort;
        private LogicPortView _returnPort;
        private LogicPortView _valuePort;

        private List<string> _pops = new List<string> {NoneFlag};
        private LogicPropertyNode PropertyNode => (LogicPropertyNode) userData;
        
        public LogicPropertyNodeView()
        {
            styleSheets.Add(Resources.Load<StyleSheet>("Styles/LogicPropertyNodeView"));
        }

        public override void Initialize(LogicGraphEditorView view, LogicNode node, IEdgeConnectorListener connectorListener)
        {
            base.Initialize(view, node, connectorListener);
            
            var inputPorts = inputContainer.Children().OfType<LogicPortView>().ToList();
            inputPorts.ForEach(p =>
            {
                p.PortSlot.onValueChange += OnPortSlotValueChange;
                switch (p.PortSlot.valueName)
                {
                    case LogicPropertyNode.NameTarget:
                        _targetPort = p;
                        break;
                    case LogicPropertyNode.NameField:
                        _fieldPort = p;
                        break;
                    case LogicPropertyNode.NameValue:
                        _valuePort = p;
                        break;
                    case LogicPropertyNode.NameReturn:
                        _returnPort = p;
                        break;
                }
            });
        }

        private void OnPortSlotValueChange(LogicPortSlot slot)
        {
            var portView = inputContainer.Children().OfType<LogicPortView>().FirstOrDefault(p => p.PortSlot == slot);
            if (portView == null) return;
            if (portView == _targetPort)
            {
                if (portView.ConnectedEdge.Count > 0 && portView.ConnectedEdge[0].output is LogicPortView sourcePortView)
                {
                    ResetTarget(sourcePortView.PortSlot);
                }
                PropertyNode.OnGraphResolved();
            }
            else if (portView == _fieldPort)
            {
                ResetValueAndReturn();
            }
        }

        private void ResetTarget(LogicPortSlot source)
        {
            if (LuaParser.Classes.TryGetValue(source.ValType, out var cls))
            {
                _pops = new List<string>(cls.Properties.Keys);
                _pops.Insert(0, NoneFlag);
            }
            
            _fieldPort.InputView.RemoveSlotView(false);
            var slotView = new PopupSlotView<string>(_pops) {Type = ValueType.String};
            slotView.Init(_fieldPort.PortSlot);
            slotView.Create();
            _fieldPort.InputView.AddSlotView(slotView);
        }
        
        private void ResetValueAndReturn()
        {
            if (_valuePort != null)
            {
                _valuePort.Refresh();
                _valuePort.InputView.RemoveSlotView();
                _valuePort.InputView.AddSlotView();
            }

            _returnPort?.Refresh();
            //todo: is connected edges compatibale with new port value type ?
        }

    }
}