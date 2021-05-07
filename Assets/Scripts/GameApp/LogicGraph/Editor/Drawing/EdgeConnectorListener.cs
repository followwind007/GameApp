using UnityEditor.Experimental.GraphView;
using UnityEngine;

namespace GameApp.LogicGraph
{
    public class EdgeConnectorListener : IEdgeConnectorListener
    {
        private readonly LogicGraphEditorView _logicGraphEditorView;
        private readonly SearchWindowProvider _searchWindowProvider;

        public EdgeConnectorListener(LogicGraphEditorView logicGraphEditorView, SearchWindowProvider searchWindowProvider)
        {
            _logicGraphEditorView = logicGraphEditorView;
            _searchWindowProvider = searchWindowProvider;
        }

        public void OnDropOutsidePort(Edge edge, Vector2 position)
        {
            var draggedPort = edge.output?.edgeConnector.edgeDragHelper.draggedPort ??
                              edge.input?.edgeConnector.edgeDragHelper.draggedPort;
            SearchWindow.Open(new SearchWindowContext(GUIUtility.GUIToScreenPoint(Event.current.mousePosition)),
                _searchWindowProvider);
        }

        public void OnDrop(GraphView graphView, Edge edge)
        {
            _logicGraphEditorView.AddEdge(edge);
        }
    }
}