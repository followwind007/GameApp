using GameApp.Util;
using LuaInterface;
using GameApp.DebugConsole;
using UnityEditor;
using UnityEngine;

namespace GameApp.LogicGraph
{
    public partial class LogicGraphEditorView
    {
        private const string EventNodeEnter = "GRAPH_NODE_ENTER";
        private const string EventNodeExit = "GRAPH_NODE_EXIT";
        private const string EventGraphPlay = "GRAPH_PLAY";
        
        public void RegisterDebug()
        {
            EventDispatcher.Instance.AddListener<string>(EventNodeEnter, OnNodeEnter);
            EventDispatcher.Instance.AddListener<string>(EventNodeExit, OnNodeExit);
            EventDispatcher.Instance.AddListener<LuaTable>(EventGraphPlay, OnGraphPlay);
        }

        public void DeregisterDebug()
        {
            EventDispatcher.Instance.RemoveListener<string>(EventNodeEnter, OnNodeEnter);
            EventDispatcher.Instance.RemoveListener<string>(EventNodeExit, OnNodeExit);
            EventDispatcher.Instance.RemoveListener<LuaTable>(EventGraphPlay, OnGraphPlay);
        }

        public void ShowGraphAndDebug(string graphId, LuaTable tbl)
        {
            var path = $"{LogicGraphSettings.Instance.graphAssetPath}{graphId}.{LogicGraphImporter.Extension}.asset";
            var guid = AssetDatabase.AssetPathToGUID(path);
            
            EditorWindow.SetGraph(guid);
            if (tbl != null)
            {
                var debugWindow = DebugConsoleWindow.Open();
                debugWindow.Table = tbl;
            }
            
            Debug.Log($"select {graphId}, guid {guid}");
        }
        
        private void OnNodeEnter(string nodeId)
        {
            GraphView.nodes.ForEach(node =>
            {
                var n = (LogicNodeView) node;
                
                if (n.NodeEditor.nodeGuid == nodeId)
                {
                    n.titleContainer.style.backgroundImage = Resources.Load<Texture2D>("LogicGraph/bg_title_green");
                    if (n.NodeEditor.NodeName == LogicGraphSettings.Instance.stateGraph.nodeState)
                    {
                        var en = GraphView.nodes.ToList();
                        foreach (var sNode in en)
                        {
                            var sn = (LogicNodeView) sNode;
                            if (sn.NodeEditor.NodeName == LogicGraphSettings.Instance.stateGraph.nodeState && sn.NodeEditor.nodeGuid != nodeId)
                            {
                                sn.titleContainer.style.backgroundImage = null;
                            }
                        }
                    }
                    else
                    {
                        Timer.Add(LogicGraphSettings.Instance.graphDebug.notifyStayDura, () => n.titleContainer.style.backgroundImage = null);
                    }
                }
            });
        }

        private void OnNodeExit(string nodeId)
        {
            GraphView.nodes.ForEach(node =>
            {
                var n = (LogicNodeView) node;
                if (n.NodeEditor.nodeGuid == nodeId)
                {
                    Timer.Add(2, () => n.titleContainer.style.backgroundImage = null);
                }
            });
        }

        private void OnGraphPlay(LuaTable tbl)
        {
            if (LogicGraphPrefs.Instance.autoActive)
            {
                ShowGraphAndDebug(tbl.GetStringField("graphId"), tbl);
            }
        }
        
    }
}