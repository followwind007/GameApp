using System;
using System.Collections.Generic;
using UnityEditor.Experimental.GraphView;
using UnityEngine;
using UnityEngine.UIElements;

namespace GameApp.AnimatorBehaviour
{
    public class AnimatorSearchProvider : ScriptableObject, ISearchWindowProvider
    {
        private struct NodeEntry
        {
            public Type type;
        }

        public List<SearchTreeEntry> CreateSearchTree(SearchWindowContext context)
        {
            var tree = new List<SearchTreeEntry>
            {
                new SearchTreeGroupEntry(new GUIContent("Create State")),
            };

            foreach (var t in AnimatorTypeUtil.StateTypes)
            {
                var entry = new NodeEntry { type = t };
                tree.Add(new SearchTreeEntry(new GUIContent(t.Name))
                {
                    level = 1,
                    userData = entry
                });
            }

            return tree;
        }

        public bool OnSelectEntry(SearchTreeEntry searchTreeEntry, SearchWindowContext context)
        {
            var window = AnimatorWindow.Instance;
            if (!window) return false;
            
            var entry = (NodeEntry)searchTreeEntry.userData;

            var mousePos = window.rootVisualElement.ChangeCoordinatesTo(window.rootVisualElement.parent,
                context.screenMousePosition - window.position.position);
            var pos = window.View.Graph.contentViewContainer.WorldToLocal(mousePos);

            var s = window.Data.InternalCreateState(entry.type);
            s.position = pos;
            window.View.AddState(s);
            
            return true;
        }
    }
}