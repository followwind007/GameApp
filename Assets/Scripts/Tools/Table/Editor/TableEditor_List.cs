using System;
using System.Collections.Generic;
using System.Text.RegularExpressions;
using UnityEditor;
using UnityEngine;

namespace Tools.Table
{
    public partial class TableEditor
    {
        private static readonly Vector2 ListMenuSize = new Vector2(220, 150);
        public class ListMenu : PopupWindowContent
        {
            public Action<int> onChangeIndex;
            public Action onDelete;
            public Action onInsert;

            public int index;
            public PropertyItem item;
            
            public override Vector2 GetWindowSize()
            {
                return ListMenuSize;
            }

            public override void OnGUI(Rect rect)
            {
                GUILayout.Space(5);
                GUILayout.BeginHorizontal();
                GUILayout.Space(7);

                GUILayout.BeginVertical();

                GUILayout.BeginHorizontal();
                GUILayout.Space(5);
                if (GUILayout.Button("Insert Item", EditorStyles.miniButtonMid))
                {
                    onInsert?.Invoke();
                    EditorWindow.focusedWindow.Close();
                }
                if (GUILayout.Button("Delete Item", EditorStyles.miniButtonMid))
                {
                    onDelete?.Invoke();
                    EditorWindow.focusedWindow.Close();
                }
                GUILayout.EndHorizontal();
                
                GUILayout.Space(2);
                
                index = EditorGUILayout.IntSlider(index, 0, item.prop.arraySize - 1);

                GUILayout.EndVertical();
                
                GUILayout.Space(5);
                GUILayout.EndHorizontal();
                GUILayout.Space(5);
            }

            public override void OnClose()
            {
                onChangeIndex?.Invoke(index);
            }
        }
        
        public class ArrayOperation
        {
            public enum Operation
            {
                None, Insert, Delete, Move
            }

            public Operation op;
            public SerializedProperty p;
            public int index;
            public int index2;

            public void SetInsertOperation(SerializedProperty item, int insertIndex)
            {
                op = Operation.Insert;
                p = item;
                index = insertIndex;
            }

            public void SetDeleteOperation(SerializedProperty item, int deleteIndex)
            {
                op = Operation.Delete;
                p = item;
                index = deleteIndex;
            }

            public void SetMoveOperation(SerializedProperty item, int sourceIndex, int targetIndex)
            {
                op = Operation.Move;
                p = item;
                index = sourceIndex;
                index2 = targetIndex;
            }

            public void TryOperation()
            {
                switch (op)
                {
                    case Operation.None:
                        break;
                    case Operation.Insert:
                        p.InsertArrayElementAtIndex(index);
                        break;
                    case Operation.Delete:
                        p.DeleteArrayElementAtIndex(index);
                        break;
                    case Operation.Move:
                        p.MoveArrayElement(index, index2);
                        break;
                }

                op = Operation.None;
            }
        }
        
        private ArrayOperation _arrayOperation = new ArrayOperation();
        
        private readonly Dictionary<string, int> _arrLength = new Dictionary<string, int>();
        
        private ListMenu _listMenu = new ListMenu();

        private void DrawListPropertyItem(PropertyItem p, ListItemAttribute attr, int indent)
        {
            var preIndent = EditorGUI.indentLevel;
            var prop = p.prop;
            if (!prop.isArray) return;
            
            var elements = GetPropertyListItems(p);

            EditorGUI.indentLevel = indent;
            prop.isExpanded = LayoutUtil.Foldout(prop.isExpanded, 
                IsDebug ? $"[{indent}][L] {p.DisplayName}" : p.DisplayName, true);
            
            if (prop.isExpanded)
            {
                var path = p.Path;
                var filter = "";
                if (attr.useSearch)
                {
                    if (!SearchField.ContainsKey(path)) SearchField[path] = "";
                    EditorGUILayout.BeginHorizontal();

                    SearchField[path] = EditorGUILayout.TextField(SearchField[path], EditorStyles.toolbarSearchField);
                    if (GUILayout.Button("", GUI.skin.FindStyle("ToolbarSeachCancelButton")))
                    {
                        SearchField[path] = "";
                        GUI.FocusControl(null);
                    }
                    
                    filter = SearchField[path];
                    EditorGUILayout.EndHorizontal();
                }

                if (!_arrLength.ContainsKey(path)) _arrLength[path] = prop.arraySize;
                _arrLength[path] = EditorGUILayout.IntField("Size", _arrLength[path]);
                _arrLength[path] = Mathf.Clamp(_arrLength[path], 0, 10000);
                if (Event.current.type == EventType.KeyUp && Event.current.keyCode == KeyCode.Return)
                {
                    prop.arraySize = _arrLength[path];
                }
                
                var regex = IsValidRegexPattern(filter) ? new Regex(filter) : null;
                
                var count = Mathf.Min(prop.arraySize, elements.Count);
                for (var i = 0; i < count; i++)
                {
                    if (i >= prop.arraySize || prop.GetArrayElementAtIndex(i) == null) break;
                    
                    var ele = elements[i];
                    
                    if (regex != null)
                    {
                        var desc = ele.DisplayName;
                        if (!regex.IsMatch(desc)) continue;
                    }
                    
                    EditorGUILayout.BeginHorizontal();
                    DrawPropertyItem(ele, indent + 1);
                    if (GUILayout.Button(">", EditorStyles.miniButtonMid, GUILayout.Width(15)))
                    {
                        var curIdx = i;
                        
                        var rt = Event.current.mousePosition;
                        var rect = new Rect(new Vector2(rt.x - ListMenuSize.x, rt.y - ListMenuSize.y), ListMenuSize);
                        
                        _listMenu.index = curIdx;
                        _listMenu.item = p;
                        _listMenu.onDelete = () => { _arrayOperation.SetDeleteOperation(prop, curIdx); };
                        _listMenu.onChangeIndex = idx =>
                        {
                            if (idx != curIdx)
                            {
                                _arrayOperation.SetMoveOperation(prop, curIdx, idx);
                            }
                        };
                        _listMenu.onInsert = () => { _arrayOperation.SetInsertOperation(prop, curIdx); };
                        
                        PopupWindow.Show(rect, _listMenu);
                    }
                    
                    EditorGUILayout.EndHorizontal();
                    
                    if (Event.current.type != EventType.Layout && _arrayOperation.op != ArrayOperation.Operation.None)
                    {
                        _arrayOperation.TryOperation();
                        _arrLength[path] = prop.arraySize;
                    }
                }
            }

            EditorGUI.indentLevel = preIndent;
        }
        
        private static List<PropertyItem> GetPropertyListItems(PropertyItem p)
        {
            var items = new List<PropertyItem>();
            var prop = p.prop.Copy();
            var count = prop.arraySize;

            var field = p.parent.GetType().GetField(prop.name);
            if (p.obj == null)
            {
                return items;
            }
            var objCount = field.FieldType.GetProperty("Count")?.GetValue(p.obj);

            if (objCount != null)
            {
                count = Mathf.Min(count, (int) objCount);
            }

            for (var i = 0; i < count; i++)
            {
                var eleProp = prop.GetArrayElementAtIndex(i);
                var eleObj = field.FieldType.GetProperty("Item")?.GetValue(p.obj, new object[] {i});
                
                if (eleProp != null && eleObj != null)
                {
                    var item = new PropertyItem(eleProp, eleObj, p.obj);
                    var descAttr = item.Attrs.customDesc;
                    if (descAttr != null)
                    {
                        item.displayName = descAttr.showIndex ? $"index:{i + descAttr.indexBase} {eleObj}" : $"{eleObj}";
                    }
                    items.Add(item);
                }
            }
            return items;
        }
        
        private static bool IsValidRegexPattern(string pattern)
        {
            if (pattern == null || pattern.Trim().Length == 0) return false;
            
            try
            {
                var unused = Regex.Match("", pattern);
            }
            catch (ArgumentException)
            {
                return false;
            }
            
            return true;
        }
        
    }
}