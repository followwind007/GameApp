using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text;
using UnityEditor;
using UnityEngine;
using ValueType = GameApp.DataBinder.ValueType;

namespace GameApp.LogicGraph
{
    public class LuaGraphGenerator
    {   
        protected readonly LogicGraphObject graphObject;
        protected readonly Dictionary<string, LogicNode> nodes;
        protected readonly List<SerializedEdge> edges;

        protected readonly StringBuilder sb = new StringBuilder();
        protected readonly LogicGraphSettings settings = LogicGraphSettings.Instance;
        protected string Id => graphObject.GraphId;

        private readonly LogicPortSlot.PortType[] _ptypes = {LogicPortSlot.PortType.Param, LogicPortSlot.PortType.TranstionIn};
        private readonly LogicPortSlot.PortType[] _rtypes = {LogicPortSlot.PortType.Return};

        public LuaGraphGenerator(string assetPath)
        {
            graphObject = AssetDatabase.LoadAssetAtPath<LogicGraphObject>(assetPath);
            graphObject.binds.Init();
            InitGraph(graphObject, out var ns);
            nodes = ns;
            edges = graphObject.logicGraphData.serializedEdges;
        }

        public static LogicNode GetLogicNode(SerializedNode serializedNode, LogicGraphObject graphObject)
        {
            var nodeType = typeof(LogicNode);
            if (serializedNode.ClassName == LogicGraphSettings.LuaClass.Property)
            {
                nodeType = typeof(LogicPropertyNode);
            }
            else if (serializedNode.ClassName == LogicGraphSettings.LuaClass.Variable)
            {
                nodeType = typeof(LogicVariableNode);
            }
            
            var nodeEditor = (LogicNode)JsonUtility.FromJson(serializedNode.json, nodeType);
            
            nodeEditor.GraphObject = graphObject;
            nodeEditor.SerializedNode = serializedNode;
            var res = nodeEditor.Init();

            return res ? nodeEditor : null;
        }

        public static void InitGraph(LogicGraphObject graphObject, out Dictionary<string, LogicNode> nodes)
        {
            nodes = new Dictionary<string, LogicNode>();
            var invalidNodes = new List<SerializedNode>();
            foreach (var serializedNode in graphObject.logicGraphData.serializedNodes)
            {
                var nodeEditor = GetLogicNode(serializedNode, graphObject);
                if (nodeEditor == null)
                {
                    invalidNodes.Add(serializedNode);
                    continue;
                }
                nodes[nodeEditor.nodeGuid] = nodeEditor;
            }
            
            //remove invalid nodes
            foreach (var node in invalidNodes)
            {
                graphObject.logicGraphData.serializedNodes.Remove(node);
            }
            
            var edges = graphObject.logicGraphData.serializedEdges;
            
            var inValidEdges = new List<SerializedEdge>();
            foreach (var edge in edges)
            {
                if (!nodes.ContainsKey(edge.sGuid) || !nodes.ContainsKey(edge.tGuid))
                    inValidEdges.Add(edge);
            }
            
            //remove invalid edges
            foreach (var edge in inValidEdges) edges.Remove(edge);

            foreach (var edge in edges)
            {
                var sn = nodes[edge.sGuid];
                var tn = nodes[edge.tGuid];
                foreach (var p in sn.portSlots)
                {
                    if (p.valueName == edge.sPort)
                    {
                        if (!sn.linkNodes.ContainsKey(p)) sn.linkNodes[p] = new List<LogicNode>();
                        sn.linkNodes[p].Add(tn);
                        break;
                    }
                }

                foreach (var p in tn.portSlots)
                {
                    if (p.valueName == edge.tPort)
                    {
                        if (!tn.linkNodes.ContainsKey(p)) tn.linkNodes[p] = new List<LogicNode>();
                        tn.linkNodes[p].Add(sn);
                        break;
                    }
                }
            }
        }

        public void Generate()
        {
            GenerateRequired();
            GenerateProperties();
            GenerateGraph();
            
            GenerateCallback();
            
            sb.Append($"return {Id}");
            Debug.Log($"generate: {settings.graphCodePath}{graphObject.GraphId}.lua\n{sb}");
            WriteToFile();
        }

        protected virtual void GenerateGraph()
        {
            var eventNodes = nodes.Values.Where(n => n.ShortType == LogicGraphSettings.NodeType.Event).ToList();
            foreach (var en in eventNodes)
            {
                sb.AppendLine($"function {Id}:{en.NodeName}()");
                GenerateSchedule(en, 1);
                sb.AppendLine("end");
                sb.AppendLine();
            }
        }

        protected void GenerateSchedule(LogicNode node, int indent, bool includeSelf = false)
        {
            GenerateVariable(node, indent);
            var schedules = GetScheduleNodes(node, includeSelf);
            var runCount = new Dictionary<string, int>();
            foreach (var flow in schedules)
            {
                var lines = new List<string>();
                DfsCall(flow, lines, runCount);
                foreach (var l in lines)
                {
                    sb.IndentAppendLine(l, indent);
                }
            }
        }

        protected void GenerateVariable(LogicNode node, int indent)
        {
            var nodesDict = GetLinkNodes(node, LogicPortSlot.PortType.Return);
            foreach (var kv in nodesDict)
            {
                foreach (var n in kv.Value)
                {
                    if (n is LogicVariableNode vn)
                    {
                        var assign = $"self.{vn.propertyName} = {kv.Key.valueName}";
                        sb.IndentAppendLine(assign, indent);
                    }
                }
            }
        }

        protected void DfsCall(LogicNode node, List<string> lines, Dictionary<string, int> runCount)
        {
            if (DealSpecialNode(node, lines)) return;
            
            foreach (var kv in node.linkNodes)
            {
                if (kv.Value[0] is LogicVariableNode) continue;
                
                if (kv.Key.LogicPortType == LogicPortSlot.PortType.Param)
                    DfsCall(kv.Value[0], lines, runCount);
            }

            if (runCount.ContainsKey(node.nodeGuid)) return;
            runCount[node.nodeGuid] = 1;

            var p = "";
            var ps = GetSlots(node, _ptypes);
            for (var i = 0; i < ps.Count; i++)
            {
                p += GetParamName(node, ps[i]);
                if (i < ps.Count - 1) p += ", ";
            }
            
            if (settings.graphDebug.debugMode)
                lines.Add(string.Format(settings.graphDebug.formatNodeEnter, node.nodeGuid));
            
            var linkReturn = GetLinkNodes(node, LogicPortSlot.PortType.Return);
            if (linkReturn.Count > 0)
            {
                var r = "";
                var rs = GetSlots(node, _rtypes);
                for (var i = 0; i < rs.Count; i++)
                {
                    r += GetReturnName(node, rs[i]);
                    if (i < rs.Count - 1) r += ", ";
                }
                lines.Add($"local {r} = {node.CallName}({p})");
                foreach (var kv in linkReturn)
                {
                    foreach (var tn in kv.Value)
                    {
                        if (tn.ShortType == LogicGraphSettings.NodeType.Variable)
                            lines.Add($"self.{GetVariableName(tn)} = {kv.Key.BindKey}");
                    }
                }
            }
            else if (IsSelfReturn(node, out var desc))
                lines.Add($"{desc} = {node.CallName}({p})");
            else
                lines.Add($"{node.CallName}({p})");
        }

        private string GetVariableName(LogicNode node)
        {
            return node is LogicVariableNode vn ? vn.propertyName : null;
        }

        private bool IsSelfReturn(LogicNode node, out string desc)
        {
            desc = "self._";
            if (node.NodeName == settings.logicGraph.nodeAddTimer)
            {
                desc = $"self.timers.{node.ShortGuid}";
                return true;
            }
            return false;
        }

        private bool DealSpecialNode(LogicNode node, List<string> lines)
        {
            var dealed = false;
            if (node.ShortType == LogicGraphSettings.NodeType.Graph)
            {
                if (node.NodeName == settings.logicGraph.nodeDispose)
                {
                    lines.Add("self:Dispose()");
                }

                dealed = true;
            }
            else if (node.ShortType == LogicGraphSettings.NodeType.Callback)
            {
                dealed = true;
            }
            
            return dealed;
        }

        protected string GetParamName(LogicNode node, LogicPortSlot slot)
        {
            if (!node.linkNodes.ContainsKey(slot))
            {
                var wrapName = $"{slot.BindKey}";
                if (slot.ValType == settings.luaTypes.typeElua)
                {
                    return (string)graphObject.binds.GetData(wrapName);
                }
                return $"self.{wrapName}";
            }

            foreach (var edge in edges)
            {
                if (edge.tGuid == node.nodeGuid && edge.tPort == slot.valueName)
                {
                    string desc;
                    var sNode = nodes[edge.sGuid];
                    if (sNode.ShortType == LogicGraphSettings.NodeType.Variable && sNode is LogicVariableNode logicNode)
                        desc = $"self.{logicNode.propertyName}";
                    else if (sNode.ShortType == LogicGraphSettings.NodeType.Callback)
                        desc = GetCallbackName(sNode, sNode.GetPortSlot(edge.sPort));
                    else
                        desc = $"{sNode.GetPortSlot(edge.sPort).BindKey}";
                    
                    return desc;
                }
            }

            return null;
        }

        private string GetCallbackName(LogicNode node, LogicPortSlot slot)
        {
            var cb = "";
            if (slot.LogicPortType == LogicPortSlot.PortType.TranstionOut)
            {
                var paras = GetCallbackParams(node);
                cb = string.IsNullOrEmpty(paras) ? 
                    $"function({paras}) coroutine.start(self.{node.ShortGuid}, nil, self) end" : 
                    $"function({paras}) coroutine.start(self.{node.ShortGuid}, nil, self, {paras}) end";
            }
            else if (slot.LogicPortType == LogicPortSlot.PortType.Param)
            {
                cb = slot.DisplayName;
            }
            
            return cb;
        }

        private void GenerateRequired()
        {
            var requires = new Dictionary<string, string>();
            foreach (var node in nodes.Values)
            {
                if (node.Cls == null) continue;
                if (LogicGraphSettings.IsPlaceHolderNode(node.ShortType)) continue;
                if (!node.Cls.IsCsType) requires[node.RequireName] = LogicGraphSettings.GetRequirePath(node.Cls?.RelativePath);
            }

            foreach (var r in requires)
            {
                var require = $"local {r.Key} = require('{LogicGraphSettings.GetRequirePath(r.Value)}')";
                sb.AppendLine(require);
            }

            sb.AppendLine();
            sb.AppendLine($"local {Id} = DefineClass(BaseLogicGraph)");
            sb.AppendLine();
        }

        private void GenerateProperties()
        {
            sb.AppendLine($"function {Id}:Ctor()");
            sb.IndentAppendLine($"self.graphId = '{graphObject.GraphId}'", 1);
            sb.IndentAppendLine("self.timers = {}", 1);
            sb.IndentAppendLine("self.coroutines = {}", 1);
            sb.AppendLine();
            if (!graphObject.useAsset)
            {
                graphObject.binds.Init();
            
                var ports = new Dictionary<string, LogicPortSlot>();
                foreach (var n in nodes.Values)
                {
                    foreach (var p in n.portSlots) ports[p.BindKey] = p;
                }
                
                foreach (var w in graphObject.binds.wraps)
                {
                    ports.TryGetValue(w.name, out var p);

                    var isValid = true;

                    if (graphObject.properties.All(prop => prop.name != w.name))
                    {
                        if (p != null && (p.owner.ShortType == LogicGraphSettings.NodeType.Variable || p.owner.linkNodes.ContainsKey(p)))
                        {
                            isValid = false;
                        }

                        if (p?.ValType == settings.luaTypes.typeElua)
                        {
                            isValid = false;
                        }
                    }

                    if (!isValid) continue;
                    
                    var dc = "nil";
                    var t = w.type;
                    if (t == ValueType.Int || t == ValueType.Float)
                    {
                        dc = $"{w.value}";
                    }
                    else if (t == ValueType.Bool)
                    {
                        dc = $"{w.value}".ToLower();
                    }
                    else if (t == ValueType.Vector2)
                    {
                        var valVec2 = (Vector2) w.value;
                        dc = $"Vector2({valVec2.x},{valVec2.y})";
                    }
                    else if (t == ValueType.Vector3)
                    {
                        var valVec3 = (Vector3) w.value;
                        dc = $"Vector3({valVec3.x},{valVec3.y},{valVec3.z})";
                    }
                    else if (t == ValueType.Vector4)
                    {
                        var valVec4 = (Vector4) w.value;
                        dc = $"Vector4({valVec4.x},{valVec4.y},{valVec4.z},{valVec4.w})";
                    }
                    else if (t == ValueType.Rect)
                    {
                        var valRect = (Vector4) w.value;
                        dc = $"Rect({valRect.x},{valRect.y},{valRect.z},{valRect.w})";
                    }
                    else if (t == ValueType.Color)
                    {
                        var valCol = (Color) w.value;
                        dc = $"Color({valCol.r},{valCol.g},{valCol.b})";
                    }
                    else if (t == ValueType.String)
                    {
                        if (p != null && p.ValType == settings.luaTypes.typeElua && string.IsNullOrEmpty((string)w.value))
                            dc = $"{w.value}";
                        else
                            dc = $"\"{w.value}\"";
                    }
                    
                    sb.IndentAppendLine($"self.{w.name} = {dc}", 1);
                }
            }

            sb.AppendLine("end");
            sb.AppendLine();
        }

        private IEnumerable<LogicNode> GetScheduleNodes(LogicNode start, bool includeSelf = false)
        {
            var list = new List<LogicNode>();
            var q = new Queue<LogicNode>();
            
            q.Enqueue(start); 
            if (includeSelf) list.Add(start);
            
            while (q.Count > 0)
            {
                var cur = q.Dequeue();
                var kv = GetLinkNodes(cur, LogicPortSlot.PortType.ScheduleOut);
                
                foreach (var nodeList in kv.Values)
                {
                    foreach (var node in nodeList)
                    {
                        list.Add(node);
                        q.Enqueue(node);
                    }
                }
            }
            return list;
        }

        private void GenerateCallback()
        {
            var cnodes = nodes.Values.Where(n => n.ShortType == LogicGraphSettings.NodeType.Callback).ToList();
            foreach (var node in cnodes)
            {
                sb.AppendLine($"function {Id}:{node.ShortGuid}({GetCallbackParams(node)})");
                if (settings.graphDebug.debugMode)
                    sb.IndentAppendLine(string.Format(settings.graphDebug.formatNodeEnter, node.nodeGuid), 1);
                sb.IndentAppendLine("table.insert(self.coroutines, coroutine.running())", 1);
                GenerateSchedule(node, 1);
                sb.AppendLine("end");
                sb.AppendLine();
            }
        }

        private string GetCallbackParams(LogicNode node)
        {
            var ps = "";
            var paras = node.Func.paramters;
            for (var i = 0; i < paras.Count; i++)
            {
                var p = paras[i];
                ps += i < paras.Count - 1 ? $"{p.name}, " : p.name;
            }
            return ps;
        }
        
        private string GetReturnName(LogicNode node, LogicPortSlot slot)
        {
            return !node.linkNodes.ContainsKey(slot) ? "_" : $"{slot.BindKey}";
        }
        
        private Dictionary<LogicPortSlot, List<LogicNode>> GetLinkNodes(LogicNode node, LogicPortSlot.PortType type)
        {
            return node.linkNodes.Where(kv => kv.Key.LogicPortType == type).ToDictionary(
                    p => p.Key, 
                    p => p.Value);
        }
        
        private List<LogicPortSlot> GetSlots(LogicNode node, LogicPortSlot.PortType[] types)
        {
            var list = new List<LogicPortSlot>();
            foreach (var p in node.portSlots)
            {
                list.AddRange(from type in types where p.LogicPortType == type select p);
            }
            return list;
        }

        private void WriteToFile()
        {
            var name = graphObject.GraphId;
            var path = $"{settings.graphCodePath}{name}.lua";
            File.WriteAllText(path, sb.ToString());

            graphObject.exportPath = path;
            
            EditorUtility.SetDirty(graphObject);
            AssetDatabase.SaveAssets();
            AssetDatabase.Refresh();
        }
        
    }
}
