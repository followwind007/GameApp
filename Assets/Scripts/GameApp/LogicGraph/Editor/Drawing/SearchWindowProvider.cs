using System;
using System.Collections.Generic;
using System.Linq;
using UnityEditor;
using UnityEditor.Experimental.GraphView;
using UnityEngine;
using UnityEngine.UIElements;

namespace GameApp.LogicGraph
{
    public class SearchWindowProvider : ScriptableObject, ISearchWindowProvider
    {
        private EditorWindow _editorWindow;
        private LogicGraphEditorView _logicGraphEditorView;
        private LogicGraphView GraphView => _logicGraphEditorView.GraphView;

        private LogicGraphObject GraphObject => _logicGraphEditorView.GraphObject;

        private Texture2D _icon;

        public void Initialize(EditorWindow editorWindow, 
            LogicGraphEditorView logicGraphEditorView)
        {
            _editorWindow = editorWindow;
            _logicGraphEditorView = logicGraphEditorView;

            // Transparent icon to trick search window into indenting items
            _icon = new Texture2D(1, 1);
            _icon.SetPixel(0, 0, new Color(0, 0, 0, 0));
            _icon.Apply();
        }

        private void OnDestroy()
        {
            if (_icon != null)
            {
                DestroyImmediate(_icon);
                _icon = null;
            }
        }

        private struct NodeEntry
        {
            public string[] title;
            public LogicNode nodeEditor;
            //public string CompatibleSlotId;
        }

        public List<SearchTreeEntry> CreateSearchTree(SearchWindowContext context)
        {
            var nodeEntries = new List<NodeEntry>();
            var nt = LuaNodeTree.Instance;
            
            foreach (var node in nt.tree)
            {
                var editor = node.classId == LogicGraphSettings.LuaClass.Property ? 
                    new LogicPropertyNode(node.classId, node.functionId) : 
                    new LogicNode(node.classId, node.functionId);
                editor.GraphObject = GraphObject; 
                editor.path = node.FilePath;

                var entry = new NodeEntry {title = node.parts, nodeEditor = editor};
                nodeEntries.Add(entry);
            }

            // Sort the entries lexicographically by group then title with the requirement that items always comes before sub-groups in the same group.
            // Example result:
            // - Art/BlendMode
            // - Art/Adjustments/ColorBalance
            // - Art/Adjustments/Contrast
            nodeEntries.Sort((entry1, entry2) =>
            {
                for (var i = 0; i < entry1.title.Length; i++)
                {
                    if (i >= entry2.title.Length)
                        return 1;
                    var value = string.Compare(entry1.title[i], entry2.title[i], StringComparison.Ordinal);
                    if (value != 0)
                    {
                        // Make sure that leaves go before nodes
                        if (entry1.title.Length != entry2.title.Length && (i == entry1.title.Length - 1 || i == entry2.title.Length - 1))
                            return entry1.title.Length < entry2.title.Length ? -1 : 1;
                        return value;
                    }
                }
                return 0;
            });
            
            

            //* Build up the data structure needed by SearchWindow.

            // `groups` contains the current group path we're in.
            var groups = new List<string>();

            // First item in the tree is the title of the window.
            var tree = new List<SearchTreeEntry>
            {
                new SearchTreeGroupEntry(new GUIContent("Create Node")),
            };
            
            foreach (var nodeEntry in nodeEntries)
            {
                // `createIndex` represents from where we should add new group entries from the current entry's group path.
                var createIndex = int.MaxValue;

                // Compare the group path of the current entry to the current group path.
                for (var i = 0; i < nodeEntry.title.Length - 1; i++)
                {
                    var group = nodeEntry.title[i];
                    if (i >= groups.Count)
                    {
                        // The current group path matches a prefix of the current entry's group path, so we add the
                        // rest of the group path from the currrent entry.
                        createIndex = i;
                        break;
                    }
                    if (groups[i] != group)
                    {
                        // A prefix of the current group path matches a prefix of the current entry's group path,
                        // so we remove everyfrom from the point where it doesn't match anymore, and then add the rest
                        // of the group path from the current entry.
                        groups.RemoveRange(i, groups.Count - i);
                        createIndex = i;
                        break;
                    }
                }

                // Create new group entries as needed.
                // If we don't need to modify the group path, `createIndex` will be `int.MaxValue` and thus the loop won't run.
                for (var i = createIndex; i < nodeEntry.title.Length - 1; i++)
                {
                    var group = nodeEntry.title[i];
                    groups.Add(group);
                    tree.Add(new SearchTreeGroupEntry(new GUIContent(group)) { level = i + 1 });
                }

                // Finally, add the actual entry.
                tree.Add(new SearchTreeEntry(new GUIContent(nodeEntry.title.Last(), _icon))
                {
                    level = nodeEntry.title.Length, 
                    userData = nodeEntry
                });
            }

            return tree;
        }

        public bool OnSelectEntry(SearchTreeEntry entry, SearchWindowContext context)
        {
            var nodeEntry = (NodeEntry)entry.userData;
            var nodeEditor = nodeEntry.nodeEditor;
            
            var windowMousePosition = _editorWindow.rootVisualElement.ChangeCoordinatesTo(_editorWindow.rootVisualElement.parent, context.screenMousePosition - _editorWindow.position.position);
            var graphMousePosition = GraphView.contentViewContainer.WorldToLocal(windowMousePosition);
            nodeEditor.position = new Vector2Int((int)graphMousePosition.x, (int)graphMousePosition.y);
            
            _logicGraphEditorView.AddNode(nodeEditor);

            return true;
        }
    }
}
