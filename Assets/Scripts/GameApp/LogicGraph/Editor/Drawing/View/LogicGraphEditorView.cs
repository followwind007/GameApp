using System.Linq;
using System.Collections.Generic;
using GameApp.DataBinder;
using UnityEditor;
using UnityEditor.Experimental.GraphView;
using UnityEngine;
using UnityEngine.UIElements;

namespace GameApp.LogicGraph
{
    public partial class LogicGraphEditorView : VisualElement
    {
        public LogicGraphView GraphView { get; }
        public LogicGraphObject GraphObject { get; private set; }

        public LogicGraphEditorWindow EditorWindow { get; }
        private LogicGraphData GraphData { get { return GraphObject.logicGraphData; } }
        
        private readonly EdgeConnectorListener _edgeConnectorListener;
        private readonly PropertyBoardProvider _propertyBoard;

        private bool _reloadGraph;

        private bool _unSyncDataFlag;
        
        public LogicGraphEditorView(LogicGraphEditorWindow editorWindow)
        {
            this.AddStyleSheetPath("Styles/LogicGraphEditorView");
            EditorWindow = editorWindow;
            //top bar
            var toolbar = new LogicGraphToolbar {EditorWindow = editorWindow, EditorView = this};
            toolbar.Init();
            Add(toolbar.toolbar);
            
            //search window
            var searchWindowProvider = ScriptableObject.CreateInstance<SearchWindowProvider>();
            searchWindowProvider.Initialize(editorWindow, this);
            _edgeConnectorListener = new EdgeConnectorListener(this, searchWindowProvider);

            //graph view content
            var graphContent = new VisualElement {name = "content"};
            
            //graph
            GraphView = new LogicGraphView(GraphObject)
            {
                name = "GraphView",
                EditorView = this,
                graphViewChanged = GraphViewChanged
            };

            GraphView.SetupZoom(LogicGraphView.MinZoom, ContentZoomer.DefaultMaxScale);
            GraphView.AddManipulator(new ContentDragger());
            GraphView.AddManipulator(new SelectionDragger());
            GraphView.AddManipulator(new RectangleSelector());
            GraphView.AddManipulator(new ClickSelector());

            GraphView.nodeCreationRequest = c =>
            {
                SearchWindow.Open(new SearchWindowContext(c.screenMousePosition), searchWindowProvider);
            };
            
            //property board
            _propertyBoard = new PropertyBoardProvider(this);
            GraphView.Add(_propertyBoard.PropBoard);
            
            graphContent.Add(GraphView);

            Add(graphContent);
        }

        public void SetGraphObject(LogicGraphObject logicGraphObject)
        {
            GraphObject = logicGraphObject;
            GraphView.GraphObject = GraphObject;
            _propertyBoard.RefreshProps();
            
            _unSyncDataFlag = true;
            
            //remvove old element
            GraphView.edges.ForEach(e => e.RemoveFromHierarchy());
            GraphView.nodes.ForEach(n => n.RemoveFromHierarchy());
            
            //use new node and edges
            LoadElements();
            
            _unSyncDataFlag = false;
        }

        public IEnumerable<LogicNodeView> GetNodes()
        {
            var list = new List<LogicNodeView>();
            var nodeList = GraphView.nodes.ToList();
            foreach (var node in nodeList)
            {
                if (node is LogicNodeView view) list.Add(view);
            }
            
            return list;
        }

        public IEnumerable<LogicPortSlot> GetInputPort()
        {
            var list = new List<LogicPortSlot>();
            var edges = GraphView.edges.ToList();
            foreach (var edge in edges)
            {
                if (edge.input is LogicPortView view)
                {
                    list.Add(view.PortSlot);
                }
            }
            
            return list;
        }
        
        public void AddNode(LogicNode nodeEditor)
        {
            RegisterUndo("Add Node " + nodeEditor.NodeType);

            var serializedNode = new SerializedNode
            {
                type = nodeEditor.NodeType,
                count = GraphData.GetNodeCount(nodeEditor.NodeType)
            };
            
            nodeEditor.SerializedNode = serializedNode;
            nodeEditor.nodeGuid = $"{nodeEditor.NodeType}_{serializedNode.count}";
            nodeEditor.Init();

            serializedNode.json = JsonUtility.ToJson(nodeEditor);
            GraphData.serializedNodes.Add(serializedNode);
            
            var nodeView = AddNodeView(nodeEditor);
            GraphView.AddToSelection(nodeView);
        }
        
        public void AddEdge(Edge edgeView)
        {
            var sourceSlot = ((LogicPortView)edgeView.output).PortSlot;
            var targetSlot = ((LogicPortView)edgeView.input).PortSlot;

            RegisterUndo("Connect Edge");
            var serializedEdge = new SerializedEdge
            {
                sGuid = sourceSlot.owner.nodeGuid,
                sPort = sourceSlot.valueName,
                tGuid = targetSlot.owner.nodeGuid,
                tPort = targetSlot.valueName
            };

            GraphData.serializedEdges.Add(serializedEdge);
            edgeView.userData = serializedEdge;

            ChangeEdgeView(edgeView);
        }

        public void RemoveEdge(Edge edge, bool notify = true)
        {
            ChangeEdgeView(edge, false, notify);
            GraphData.serializedEdges.Remove(edge.userData as SerializedEdge);
        }
        
        private GraphViewChange GraphViewChanged(GraphViewChange change)
        {
            if (_unSyncDataFlag) return change;
            if (change.movedElements != null)
            {
                RegisterUndo("Graph Element Moved.");
                foreach (var element in change.movedElements)
                {
                    var nodeEditor = (LogicNode)element.userData;
                    var pos = element.GetPosition().position;
                    nodeEditor.position = new Vector2Int((int)pos.x, (int)pos.y);
                    nodeEditor.Serialize();
                }
            }

            if (change.elementsToRemove != null)
            {
                RegisterUndo("Deleted Graph Elements.");
                foreach (var nodeView in change.elementsToRemove.OfType<LogicNodeView>())
                {
                    nodeView.NodeEditor.OnRemove();
                    GraphData.serializedNodes.Remove(nodeView.NodeEditor.SerializedNode);
                }

                foreach (var edge in change.elementsToRemove.OfType<Edge>())
                {
                    RemoveEdge(edge);
                }

                foreach (var field in change.elementsToRemove.OfType<BlackboardField>())
                {
                    var prop = (GraphProperty)field.userData;
                    GraphObject.binds.RemoveWrap(prop.name);
                    foreach (var p in GraphObject.properties)
                    {
                        if (p.name == prop.name)
                        {
                            GraphObject.properties.Remove(p);
                            break;
                        }
                    }
                    _propertyBoard.RefreshProps();
                }
            }
            
            return change;
        }

        private void LoadElements()
        {
            LuaGraphGenerator.InitGraph(GraphObject, out var ns);

            foreach (var n in ns.Values)
            {
                n.OnGraphResolved();
                AddNodeView(n);
            }
            
            foreach (var t in GraphData.serializedEdges) AddEdgeFromLoad(t);
            foreach (var node in GetNodes())
            {
                node.RefreshExpand();
                node.RefreshLeafState();
            }
        }

        private LogicNodeView AddNodeView(LogicNode node)
        {
            var nodeView = node.classId == LogicGraphSettings.LuaClass.Property ? new LogicPropertyNodeView() : new LogicNodeView();
            nodeView.userData = node;

            GraphView.AddElement(nodeView);
            nodeView.Initialize(this, node, _edgeConnectorListener);
            nodeView.MarkDirtyRepaint();
            return nodeView;
        }
        
        private void AddEdgeFromLoad(SerializedEdge serializedEdge)
        {
            var sourceNodeView = GraphView.nodes.ToList().OfType<LogicNodeView>()
                .FirstOrDefault(x => x.NodeEditor.nodeGuid == serializedEdge.sGuid);
            if (sourceNodeView == null) return;
            
            var sourceAnchor = sourceNodeView.outputContainer.Children().OfType<LogicPortView>()
                .FirstOrDefault(x => x.PortSlot.valueName == serializedEdge.sPort);

            var targetNodeView = GraphView.nodes.ToList().OfType<LogicNodeView>()
                .FirstOrDefault(x => x.NodeEditor.nodeGuid == serializedEdge.tGuid);
            if (targetNodeView == null) return;
            
            var targetAnchor = targetNodeView.inputContainer.Children().OfType<LogicPortView>()
                .FirstOrDefault(x => x.PortSlot.valueName == serializedEdge.tPort);

            var edgeView = new Edge
            {
                userData = serializedEdge,
                output = sourceAnchor,
                input = targetAnchor
            };

            ChangeEdgeView(edgeView);
        }
        
        private void ChangeEdgeView(Edge edgeView, bool isAdd = true, bool notify = true)
        {
            var inputPort = (LogicPortView) edgeView.input;
            var outputPort = (LogicPortView) edgeView.output;

            if (isAdd)
            {
                edgeView.output?.Connect(edgeView);
                edgeView.input?.Connect(edgeView);
                GraphView.AddElement(edgeView);

                if (notify)
                {
                    inputPort.AddEdge(edgeView);
                    outputPort.AddEdge(edgeView);
                }
                else
                {
                    inputPort.ConnectedEdge.Add(edgeView);
                    outputPort.ConnectedEdge.Add(edgeView);
                }
            }
            else
            {
                if (notify)
                {
                    inputPort.RemoveEdge(edgeView);
                    outputPort.RemoveEdge(edgeView);
                }
                else
                {
                    inputPort.ConnectedEdge.Remove(edgeView);
                    outputPort.ConnectedEdge.Remove(edgeView);
                }
            }
            
            if (inputPort.InputView != null)
            {
                inputPort.InputView.visible = !isAdd;
            }
        }

        private void RegisterUndo(string mark)
        {
            Undo.RegisterCompleteObjectUndo(GraphObject, mark);
        }

        public void SaveLogicGraphViewTransform()
        {
            GraphObject.lastClosePosition = GraphView.viewTransform.position;  
            GraphObject.lastCloseScale = GraphView.viewTransform.scale;
            if (GraphObject.lastCloseScale.x < LogicGraphView.MinZoom)
                GraphObject.lastCloseScale.x = LogicGraphView.MinZoom;
            if (GraphObject.lastCloseScale.y < LogicGraphView.MinZoom)
                GraphObject.lastCloseScale.y = LogicGraphView.MinZoom;
            if (GraphObject.lastCloseScale.z < LogicGraphView.MinZoom)
                GraphObject.lastCloseScale.z = LogicGraphView.MinZoom;
        }
        
    }
}