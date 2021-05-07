using System.Collections.Generic;
using System.Linq;

namespace GameApp.LogicGraph
{
    public class StateGraphGenerator : LuaGraphGenerator
    {
        private readonly LogicGraphSettings.StateGraphSetting _stateGraph = LogicGraphSettings.Instance.stateGraph;

        private List<LogicNode> _stateNodes;
        private List<LogicNode> _transNodes;
        
        private readonly Dictionary<string, string> _transMap = new Dictionary<string, string>
        {
            {"==", "Equal"},
            {"~=", "NotEqual"},
            {"<", "Less"},
            {"<=", "LessEqual"},
            {">", "Greater"},
            {">=", "GreaterEqual"},
        };

        public StateGraphGenerator(string assetPath) : base(assetPath) { }

        protected override void GenerateGraph()
        {
            _stateNodes = nodes.Values.Where(n => n.NodeName == _stateGraph.nodeState).ToList();
            _transNodes = nodes.Values.Where(n => n.NodeName == _stateGraph.nodeTransition).ToList();
            
            GenerateCreate();
            GenerateEntry();
            GenerateState();
            GenerateExit();
            //var anyNode = Nodes.Values.First(n => n.nodeName == _stateGraph.nodeAnyState);
        }

        private void GenerateCreate()
        {
            sb.AppendLine($"function {Id}:Init()");
            sb.IndentAppendLine(_stateGraph.formatCreate, 1);
            sb.IndentAppendLine("self.fsm = fsm\n", 1);
            
            foreach (var sn in _stateNodes)
            {
                var doFunc = "nil";
                var exitFunc = "nil";
                if (sn.linkNodes.Any(kv => kv.Key.valueName == "enter"))
                {
                    doFunc = sn.linkNodes.Any(kv => kv.Value[0].NodeName == "Exit") ? 
                        $"function() self:{sn.ShortGuid}_Enter(); self:Exit() end" : 
                        $"function() self:{sn.ShortGuid}_Enter() end";
                }
                if (sn.linkNodes.Any(kv => kv.Key.valueName == "exit"))
                {
                    exitFunc = $"function() self:{sn.ShortGuid}_Exit() end";
                }
                sb.IndentAppendLine(string.Format(_stateGraph.formatCreateState, sn.ShortGuid, doFunc, exitFunc), 1);
                sb.IndentAppendLine(string.Format(_stateGraph.formatAddState, sn.ShortGuid), 1);
                sb.AppendLine();
            }

            foreach (var tn in _transNodes)
            {
                GenerateCondition(tn, 1);
            }

            sb.AppendLine();
            GenerateParam(1);
            sb.AppendLine("end\n");
        }

        private void GenerateParam(int indent)
        {
            foreach (var prop in graphObject.properties.Where(p => p.isListen).ToList())
            {
                sb.IndentAppendLine(string.Format(
                    _stateGraph.formatAddParam, 
                    prop.name, 
                    $"self.{prop.name}",
                    prop.isListen ? "true" : "false",
                    prop.isTrigger ? "true" : "false"), 
                    indent);
            }
        }

        private void GenerateCondition(LogicNode node, int indent)
        {
            var type = "nil";
            var paramName = "nil";
            var fromNode = "nil";
            var toNode = "nil";
            
            foreach (var ll in node.linkNodes)
            {
                if (ll.Key.IsOutputSlot) type = ll.Key.valueName;

                var cnodes = GetConnectedNode(ll.Key);
                if (cnodes.Count < 1) continue;

                var n = cnodes[0];
                if (ll.Key.valueName == "param" && n is LogicVariableNode logicNode)
                    paramName = logicNode.propertyName;
                else if (ll.Key.valueName == "in" && n.NodeName == _stateGraph.nodeState)
                    fromNode = n.ShortGuid;
                else if (ll.Key.IsOutputSlot && n.NodeName == _stateGraph.nodeState)
                    toNode = n.ShortGuid;
            }
            
            foreach (var tm in _transMap) if (tm.Key == type) type = tm.Value;
            var runCount = new Dictionary<string, int>();
            var target = GetTarget(node, indent, runCount);
            var add = string.Format(_stateGraph.formatAddCondition, node.nodeGuid, type, $"{paramName}", target);
            sb.IndentAppendLine(add, indent);

            var link = string.Format(_stateGraph.formatAddLink, fromNode, toNode, node.nodeGuid);
            sb.IndentAppendLine(link, indent);
        }

        private string GetTarget(LogicNode node, int indent, Dictionary<string, int> runCount)
        {
            var slot = node.portSlots.First(p => p.valueName == "target");
            var paramName = GetParamName(node, slot);

            if (node.linkNodes.ContainsKey(slot))
            {
                var lNodes = node.linkNodes[slot];
                var lines = new List<string>();
                if (lNodes.Count > 0)
                {
                    DfsCall(lNodes[0], lines, runCount);
                }

                foreach (var l in lines)
                {
                    sb.IndentAppendLine(l, indent);
                }
            }
            
            return paramName;
        }

        private void GenerateState()
        {
            foreach (var sn in _stateNodes)
            {
                foreach (var kv in sn.linkNodes)
                {
                    if (kv.Key.valueName == "enter")
                    {
                        sb.AppendLine($"function {Id}:{sn.ShortGuid}_Enter()");
                        if (settings.graphDebug.debugMode)
                            sb.IndentAppendLine(string.Format(settings.graphDebug.formatNodeEnter, sn.nodeGuid), 1);
                        GenerateSchedule(kv.Value[0], 1, true);
                        sb.AppendLine("end\n");
                    }
                    else if (kv.Key.valueName == "exit")
                    {
                        sb.AppendLine($"function {Id}:{sn.ShortGuid}_Exit()");
                        GenerateSchedule(kv.Value[0], 1, true);
                        sb.AppendLine("end\n");
                    }
                }
            }
        }

        private void GenerateEntry()
        {
            sb.AppendLine($"function {Id}:Entry()");
            var entryNode = nodes.Values.First(n => n.NodeName == _stateGraph.nodeEntry);
            if (entryNode != null)
            {
                if (settings.graphDebug.debugMode)
                    sb.IndentAppendLine(string.Format(settings.graphDebug.formatNodeEnter, entryNode.nodeGuid), 1);
                
                var pair = entryNode.linkNodes.First(kv => kv.Key.IsOutputSlot);
                if (pair.Value != null)
                {
                    sb.IndentAppendLine(string.Format(_stateGraph.formatPlayState, pair.Value[0].ShortGuid), 1);
                }
            }
            sb.AppendLine("end\n");
        }

        private void GenerateExit()
        {
            sb.AppendLine($"function {Id}:Exit()");
            var exitNode = nodes.Values.First(n => n.NodeName == _stateGraph.nodeExit);
            if (exitNode != null)
            {
                if (settings.graphDebug.debugMode)
                    sb.IndentAppendLine(string.Format(settings.graphDebug.formatNodeEnter, exitNode.nodeGuid), 1);
                
                GenerateSchedule(exitNode, 1);
            }
            sb.IndentAppendLine(_stateGraph.formatDispose, 1);
            sb.IndentAppendLine(settings.logicGraph.formatDispose, 1);
            sb.AppendLine("end");
        }

        private List<LogicNode> GetConnectedNode(LogicPortSlot port)
        {
            var list = new List<LogicNode>();
            foreach (var edge in graphObject.logicGraphData.serializedEdges)
            {
                if (edge.sGuid == port.owner.nodeGuid && edge.sPort == port.valueName)
                {
                    list.Add(nodes[edge.tGuid]);
                }
                else if (edge.tGuid == port.owner.nodeGuid && edge.tPort == port.valueName)
                {
                    list.Add(nodes[edge.sGuid]);
                }
            }
            return list;
        }
        
    }
}