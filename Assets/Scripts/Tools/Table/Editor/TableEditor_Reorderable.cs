using System.Collections.Generic;
using UnityEditor;
using UnityEditorInternal;
using UnityEngine;

namespace Tools.Table
{
    public partial class TableEditor
    {
        private Dictionary<string, ReorderableList> _reorderableLists = new Dictionary<string, ReorderableList>();

        private void DrawReorderablePropertyItem(PropertyItem p, ReorderableItemAttribute attr, int indent)
        {
            var preIndent = EditorGUI.indentLevel;
            var prop = p.prop;
            if (!prop.isArray) return;
            
            EditorGUI.indentLevel = indent;
            prop.isExpanded = LayoutUtil.Foldout(prop.isExpanded, 
                IsDebug ? $"[{indent}][L] {p.DisplayName}" : p.DisplayName, true, indent);
            
            if (prop.isExpanded)
            {
                var rl = GetReorderableList(p, attr);
                LayoutUtil.IndentLayout(indent, () => rl.DoLayoutList());
            }

            EditorGUI.indentLevel = preIndent;
        }

        private ReorderableList GetReorderableList(PropertyItem p, ReorderableItemAttribute attr)
        {
            _reorderableLists.TryGetValue(p.Path, out var rl);
            
            if (rl == null)
            {
                rl = new ReorderableList(serializedObject, p.prop, attr.dragable, attr.displayHeader, attr.displayAddButton, attr.displayRemoveButton)
                {
                    drawElementCallback = delegate(Rect rect, int index, bool active, bool focused)
                    {
                        var element = p.prop.GetArrayElementAtIndex(index);
                        var isExpanded = element.isExpanded;
                        
                        rect.height = EditorGUI.GetPropertyHeight(element, GUIContent.none, isExpanded);
                        if (element.hasVisibleChildren) rect.xMin += 10;
                        
                        EditorGUI.PropertyField(rect, element, GUIContent.none, isExpanded);
                    },
                    elementHeightCallback = index => ElementHeightCallback(p.prop, index),
                    drawElementBackgroundCallback = (rect, index, active, focused) =>
                    {
                        if (focused == false) return;
                        rect.height = ElementHeightCallback(p.prop, index);
                        EditorGUI.LabelField(rect, GUIContent.none, EditorStyles.helpBox);
                    }
                };
                if (!attr.displayHeader) rl.headerHeight = 2;
                _reorderableLists[p.Path] = rl;
            }
            
            return rl;
        }
        
        private float ElementHeightCallback(SerializedProperty property, int index)
        {
            var arrayElement = property.GetArrayElementAtIndex(index);
            var calculatedHeight = EditorGUI.GetPropertyHeight(arrayElement, GUIContent.none, arrayElement.isExpanded);
            calculatedHeight += 3;
            return calculatedHeight;
        }
        
    }
}