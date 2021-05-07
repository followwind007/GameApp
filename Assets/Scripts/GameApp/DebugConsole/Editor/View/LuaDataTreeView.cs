using System;
using System.Collections.Generic;
using GameApp.DataBinder;
using UnityEditor.IMGUI.Controls;
using UnityEngine;
using UnityEditor;
using LuaInterface;
using Object = UnityEngine.Object;

namespace GameApp.DebugConsole
{
    public class LuaDataItem : TreeViewItem
    {
        private static int _uniqId;
        public static int GetId()
        {
            return ++_uniqId;
        }
        public static void ResetId()
        {
            _uniqId = 0;
        }

        public object Data { get; set; }

        public string Name { get; set; }

        public LuaDataItem()
        {
            
        }

        public LuaDataItem(object data)
        {
            Data = data;
        }
    }

    public class LuaDataTreeView : TreeView
    {
        private readonly SearchField _searchField = new SearchField();

        private LuaDataItem _root;

        private LuaTable _tbl;

        public LuaDataTreeView(TreeViewState treeViewState) : base(treeViewState)
        {
            rowHeight = EditorGUIUtility.singleLineHeight + 4;
            Reload();
        }

        public override void OnGUI(Rect rect)
        {
            var searchRect = rect;
            searchRect.height = 20;
            searchString = _searchField.OnGUI(searchRect, searchString);
            rect.y += 20;
            rect.height -= 20;
            base.OnGUI(rect);
        }

        public void RefreshWithTable(LuaTable tbl)
        {
            LuaDataItem.ResetId();
            _tbl = tbl;
            Reload();
        }

        private void SortChildren(TreeViewItem item)
        {
            if (!item.hasChildren)
            {
                return;
            }
            item.children.Sort((a, b) => 
                string.Compare(((LuaDataItem) a).Name, ((LuaDataItem) b).Name, StringComparison.Ordinal));
            foreach (var child in item.children)
            {
                SortChildren(child);
            }
        }

        protected override TreeViewItem BuildRoot()
        {
            _root = _tbl == null ? new LuaDataItem() {depth = -1, displayName = "Root"} 
                : new LuaDataItem(_tbl) { displayName = "table", depth = -1, id = LuaDataItem.GetId() };
            return _root;
        }
        
        protected override IList<TreeViewItem> BuildRows(TreeViewItem root)
        {
            if (!root.hasChildren)
            {
                BuildChild(root as LuaDataItem);
                SortChildren(root);
            }
            
            SetupDepthsFromParentsAndChildren(root);
            return base.BuildRows(root);
        }
        
        private void BuildChild(LuaDataItem item)
        {
            var tbl = item.Data as LuaTable;
            if (tbl == null) return;
            foreach (var entry in tbl.ToDictTable())
            {
                if (item.depth > 5)
                {
                    return;
                }
                var key = entry.Key.ToString();
                if (string.IsNullOrEmpty(key) || key.Equals("__index"))
                {
                    continue;
                }
                var child = new LuaDataItem(entry.Value)
                {
                    depth = item.depth + 1,
                    Name = entry.Key.ToString(),
                    id = LuaDataItem.GetId(),
                    displayName = entry.Key.ToString(),
                };

                item.AddChild(child);
                BuildChild(child);                
            }
        }
        
        private const float WidthName = 100f;
        protected override void RowGUI(RowGUIArgs args)
        {
            if (!(args.item is LuaDataItem item)) return;
            var indent = GetContentIndent(item);
            var rect = new Rect(indent, args.rowRect.y + 2, args.rowRect.width - indent, args.rowRect.height - 4);

            var rectName = new Rect(rect.x, rect.y, WidthName, rect.height);
            EditorGUI.LabelField(rectName, item.Name);
            
            if (item.Data == null) return;

            var rectVal = new Rect(rect.x + WidthName, rect.y, rect.width - WidthName, rect.height);

            var function = item.Data as LuaFunction;
            if (function != null)
            {
                rectVal.width = 50f;
                if (GUI.Button(rectVal, "Call"))
                {
                    function.Call(_root.Data);
                }

                rectVal.x += 55f;
                if (GUI.Button(rectVal, "Use"))
                {
                    var g = BindableTargetImpl.State.DoString<LuaTable>("return _G");
                    g.RawSet("_func", item.Data);
                }
            }
            
            else if (item.Data is int data) item.Data = EditorGUI.IntField(rectVal, GUIContent.none, data);
            else if (item.Data is float itemData) item.Data = EditorGUI.FloatField(rectVal, GUIContent.none, itemData);
            else if (item.Data is double) item.Data = EditorGUI.FloatField(rectVal, GUIContent.none, Convert.ToSingle(item.Data));
            else if (item.Data is string s) item.Data = EditorGUI.TextField(rectVal, GUIContent.none, s);
            else if (item.Data is Vector2 vector2) item.Data = EditorGUI.Vector2Field(rectVal, GUIContent.none, vector2);
            else if (item.Data is Vector3 vector3) item.Data = EditorGUI.Vector3Field(rectVal, GUIContent.none, vector3);
            //else if (item.Data is Vector4) item.Data = EditorGUI.Vector4Field(rectVal, GUIContent.none, (Vector4) item.Data);
            else if (item.Data is Color color) item.Data = EditorGUI.ColorField(rectVal, GUIContent.none, color);
            //else if (item.Data is Rect) item.Data = EditorGUI.RectField(rectVal, GUIContent.none, (Rect) item.Data);
            //else if (item.Data is Bounds) item.Data = EditorGUI.BoundsField(rectVal, GUIContent.none, (Bounds) item.Data);
            else if (item.Data is AnimationCurve curve) item.Data = EditorGUI.CurveField(rectVal, GUIContent.none, curve);
            
            else if (item.Data is Object o) item.Data = EditorGUI.ObjectField(rectVal, GUIContent.none, o, typeof(Object), true);
            else EditorGUI.LabelField(rectVal, item.Data.ToString());
            
        }

    }
}