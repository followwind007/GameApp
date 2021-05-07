using System.Collections.Generic;
using System.Linq;
using UnityEditor;
using UnityEditor.Experimental.GraphView;
using UnityEngine;
using UnityEngine.UIElements;

namespace GameApp.LogicGraph
{
	public class LogicGraphView : GraphView
	{		
		public LogicGraphObject GraphObject { get; set; }
		public LogicGraphEditorView EditorView { get; set; }
		public const float MinZoom = 0.05f;

		private LogicGraphData _copyGraphData;
		
		public LogicGraphView()
		{
			styleSheets.Add(Resources.Load<StyleSheet>("Styles/LogicGraphView"));
			serializeGraphElements = SerializeGraphElementsImplementation;
			canPasteSerializedData = CanPasteSerializedDataImplementation;
			unserializeAndPaste = UnserializeAndPasteImplementation;
		}
		
		public LogicGraphView(LogicGraphObject logicGraph) : this()
		{
			GraphObject = logicGraph;
			RegisterCallback<DragUpdatedEvent>(OnDragUpdatedEvent);
			RegisterCallback<DragPerformEvent>(OnDragPerformEvent);
		}

		private string SerializeGraphElementsImplementation(IEnumerable<GraphElement> elements)
		{
			var enumerable = elements.ToList();
			var ns = enumerable.OfType<LogicNodeView>().Select(x => x.NodeEditor);
			var es = enumerable.OfType<Edge>();
			
			_copyGraphData = new LogicGraphData();

			foreach (var node in ns)
			{
				_copyGraphData.serializedNodes.Add(node.SerializedNode);
			}

			foreach (var edge in es)
			{
				_copyGraphData.serializedEdges.Add((SerializedEdge)edge.userData);
			}
			
			return JsonUtility.ToJson(_copyGraphData, true);
		}

		private bool CanPasteSerializedDataImplementation(string serializedData)
		{           
			try
			{
				return JsonUtility.FromJson<LogicGraphData>(serializedData) != null;
			}
			catch
			{
				// ignored. just means copy buffer was not a graph :(
				return false;
			}
		}

		private void UnserializeAndPasteImplementation(string operationName, string serializedData)
		{
			Undo.RegisterCompleteObjectUndo(EditorView.GraphObject, "Paste nodes and edges");
			
			ClearSelection();
			_copyGraphData = JsonUtility.FromJson<LogicGraphData>(serializedData);
			foreach (var node in _copyGraphData.serializedNodes)
			{
				var nodeEditor = LuaGraphGenerator.GetLogicNode(node, GraphObject);
				if (nodeEditor == null)
				{
					Debug.LogWarning("No NodeEditor found for " + node);
					return;
				}
					
				var newNode = new LogicNode(nodeEditor.classId, nodeEditor.functionId)
				{
					GraphObject = nodeEditor.GraphObject,
					position = nodeEditor.position + new Vector2Int(10,10)
				}; 
				EditorView.AddNode(newNode);
			}
		}
		
		public void FocusOnNode()
		{
			var nodeList = nodes.ToList();
			if (nodeList != null && nodeList.Count > 0)
			{
				AddToSelection(nodeList[0]);
			}
		}
		
		public override List<Port> GetCompatiblePorts(Port startAnchor, NodeAdapter nodeAdapter)
		{
			var compatibleAnchors = new List<Port>();
			var startPort = (LogicPortView) startAnchor;
			var startSlot = ((LogicPortView) startAnchor).PortSlot;
			
			if (startPort.ConnectedEdge.Count > 0 && !startSlot.AllowMultiEdge)
				return compatibleAnchors;

			foreach (var anchor in ports.ToList())
			{
				var portView = (LogicPortView) anchor;

				if (startSlot.IsCompatibleWith(portView.PortSlot) && (portView.ConnectedEdge.Count < 1 || portView.PortSlot.AllowMultiEdge))
				{
					compatibleAnchors.Add(anchor);
				}
			}
			return compatibleAnchors;
		}

		private void OnDragUpdatedEvent(DragUpdatedEvent _)
		{
			var dragging = false;
			if (DragAndDrop.GetGenericData("DragSelection") is List<ISelectable> sel)
			{
				dragging = sel.OfType<BlackboardField>().Any();
			}
			
			if (dragging)
			{
				DragAndDrop.visualMode = DragAndDropVisualMode.Generic;
			}
		}

		private void OnDragPerformEvent(DragPerformEvent e)
		{
			var localPos = (e.currentTarget as VisualElement).ChangeCoordinatesTo(contentViewContainer, e.localMousePosition);

			if (DragAndDrop.GetGenericData("DragSelection") is List<ISelectable> sel)
			{
				if (sel.OfType<BlackboardField>().Any())
				{
					var fields = sel.OfType<BlackboardField>();
					foreach (var field in fields)
					{
						var prop = (GraphProperty) field.userData;
						var node = new LogicVariableNode(LogicGraphSettings.GetVariableNodePath(), prop.type, prop.name)
						{
							position = new Vector2Int((int) localPos.x, (int) localPos.y),
							GraphObject = GraphObject,
						};
						EditorView.AddNode(node);
					}
				}
			}
		}

		public IEnumerable<LogicNodeView> GetNodeLeaf(LogicNodeView nodeView)
		{
			var childs = nodeView.GetConnectedNodes(Direction.Output);
			var list = new List<LogicNodeView>();
			var q = new Queue<LogicNodeView>();
			foreach (var child in childs)
			{
				list.Add(child);
				q.Enqueue(child);
			}
			while (q.Count > 0)
			{
				var cur = q.Dequeue();
				var ins = cur.GetConnectedNodes(Direction.Input);
				var outs = cur.GetConnectedNodes(Direction.Output);
				ins.AddRange(outs);
				foreach (var node in ins)
				{
					if (node == nodeView || list.Contains(node))
					{
						continue;
					}
					list.Add(node);
					q.Enqueue(node);
				}
			}
			return list;
		}

		/*public override void OnPersistentDataReady()
		{
			base.OnPersistentDataReady();
			UpdateViewTransform(graphObject.lastClosePosition, graphObject.lastCloseScale);
		}*/
		
		
	}
}