using System.Collections.Generic;
using System.Linq;
using UnityEditor;
using UnityEngine;

namespace Tools.Table
{
    [CustomEditor(typeof(TableMonoBehaviour), true)]
    public class TableMonoBehaviourEditor : TableEditor { }

    [CustomEditor(typeof(TableScriptableObject), true)]
    public class TableScriptableObjectEditor : TableEditor { }
    
    public partial class TableEditor : Editor
    {
        private static readonly Dictionary<string, int> SelTab = new Dictionary<string, int>();
        private static readonly Dictionary<string, string> SearchField = new Dictionary<string, string>();
        
        private static readonly HashSet<SerializedPropertyType> DirectShowTypes = new HashSet<SerializedPropertyType>
        {
            SerializedPropertyType.Bounds,
            SerializedPropertyType.Color,
            SerializedPropertyType.Gradient,
            SerializedPropertyType.Quaternion,
            SerializedPropertyType.Rect,
            SerializedPropertyType.Vector2,
            SerializedPropertyType.Vector3,
            SerializedPropertyType.Vector4,
            SerializedPropertyType.BoundsInt,
            SerializedPropertyType.LayerMask,
            SerializedPropertyType.RectInt,
            SerializedPropertyType.Vector2Int,
            SerializedPropertyType.Vector3Int
        };

        private readonly bool IsDebug = false;

        public struct PropertyItem
        {
            public readonly SerializedProperty prop;
            public readonly object obj;
            public readonly object parent;
            public TableAttrs ParentAttrs => TableAttrUtil.GetTableAttrs(parent.GetType());
            public TableAttrs Attrs => TableAttrUtil.GetTableAttrs(obj.GetType());
            public string Path => prop != null ? prop.propertyPath : "Root";
            public string DisplayName => GetDisplayName();
            public string displayName;

            public PropertyItem(SerializedProperty prop, object obj, object parent)
            {
                this.prop = prop;
                this.obj = obj;
                this.parent = parent;
                displayName = null;
            }
            
            public PropertyItem(SerializedProperty prop, object obj, object parent, string displayName) : 
                this(prop, obj, parent)
            {
                this.displayName = displayName;
            }

            private string GetDisplayName()
            {
                if (displayName != null)
                {
                    return displayName;
                }

                if (ParentAttrs.customNames.TryGetValue(prop.name, out var customName))
                {
                    return customName.name;
                }

                return prop.displayName;
            }
            
        }
        
        public override void OnInspectorGUI()
        {
            var props = GetPropertyItems(serializedObject, target);
            var root = new PropertyItem(null, target, null);
            DrawProperty(props, root, 0);
            serializedObject.ApplyModifiedProperties();
        }

        private List<PropertyItem> GetPropertyItems(SerializedObject serializedObj, object val)
        {
            var props = new List<PropertyItem>();
            var iter = serializedObj.GetIterator();
            for (var enterChildren = true; iter.NextVisible(enterChildren); enterChildren = false)
            {
                var prop = serializedObj.FindProperty(iter.name);
                var obj = val.GetType().GetField(prop.name)?.GetValue(val);
                props.Add(new PropertyItem(prop, obj,val));
            }

            return props;
        }
        
        private List<PropertyItem> GetPropertyItems(SerializedProperty serializedProp, object val)
        {
            var props = new List<PropertyItem>();
            
            var childs = GetPropChilds(serializedProp.Copy());
            foreach (var child in childs)
            {
                var prop = serializedProp.FindPropertyRelative(child);
                var obj = val.GetType().GetField(prop?.name)?.GetValue(val);
                props.Add(new PropertyItem (prop, obj, val));
            }

            return props;
        }

        private List<string> GetPropChilds(SerializedProperty property)
        {
            var cp = property.Copy();
            var names = new List<string>();
            if (DirectShowTypes.Contains(property.propertyType)) return names;
            
            for (var enterChildren = true; property.NextVisible(enterChildren); enterChildren = false)
            {
                if (cp.FindPropertyRelative(property.name) == null) break;
                names.Add(property.name);
            }

            return names;
        }

        private void DrawProperty(List<PropertyItem> props, PropertyItem item, int indent)
        {
            var preIndent = EditorGUI.indentLevel;
            var show = true;
            var property = item.prop;
            
            if (property != null)
            {
                EditorGUI.indentLevel = indent;
                property.isExpanded = LayoutUtil.Foldout(property.isExpanded, 
                    IsDebug ? $"[{indent}][P] {item.DisplayName}" : item.DisplayName, true);
                show = property.isExpanded;
            }

            if (!show) return;
            
            var attrs = item.Attrs;
            
            var nonTabProps = props.Where(p => attrs.nonTabFields.Contains(p.prop.name)).ToList();
            DrawPropertyBlock(nonTabProps, item, indent + 1);

            if (attrs.tabs.Count > 0)
            {
                var path = item.Path;
                if (!SelTab.ContainsKey(path)) SelTab[path] = 0;
                
                var tabNames = attrs.tabs.ToArray();
                SelTab[path] = LayoutUtil.Toolbar(SelTab[path], tabNames, indent + 1);
                
                attrs.tabFields.TryGetValue(tabNames[SelTab[path]], out var fs);
                if (fs != null)
                {
                    var tabProps = props.Where((p) => fs.Contains(p.prop.name)).ToList();
                    DrawPropertyBlock(tabProps, item, indent + 1);
                }
            }

            if (attrs.exposedFuncs.Count > 0)
            {
                DrawExposedFuncs(item, indent);
            }

            EditorGUI.indentLevel = preIndent;
        }

        private void DrawPropertyBlock(List<PropertyItem> props, PropertyItem item, int indent)
        {
            var preIndent = EditorGUI.indentLevel;
            
            var drawnLine = new HashSet<string>();
            var attr = item.Attrs;
            
            var propDict = new Dictionary<string, PropertyItem>();
            props.ForEach(p=> propDict[p.prop.name] = p);
            
            foreach (var p in props)
            {
                var pName = p.prop.name;
                if (attr.allLineFields.TryGetValue(pName, out var line))
                {
                    if (drawnLine.Contains(line)) continue;
                    drawnLine.Add(line);
                    EditorGUI.indentLevel = indent;
                    EditorGUILayout.BeginHorizontal();

                    var fs = attr.lineFields[line];
                    EditorGUILayout.LabelField(line, GUILayout.Width(200));
                    
                    var width = EditorGUIUtility.currentViewWidth - 200 - LayoutUtil.GetIndent(indent);
                    var count = fs.Count;
                    for (var i = 0; i < count; i++)
                    {
                        var fp = propDict[fs[i]].prop;
                        EditorGUILayout.PropertyField(fp, GUIContent.none, true, GUILayout.Width(width / count));
                    }
                    
                    EditorGUILayout.EndHorizontal();
                }
                else
                {
                    DrawPropertyItem(p, indent);
                }
            }

            EditorGUI.indentLevel = preIndent;
        }

        private void DrawPropertyItem(PropertyItem p, int indent)
        {
            var preIndent = EditorGUI.indentLevel;
            EditorGUILayout.BeginVertical();
            List<PropertyItem> subProps = null;
                
            if (p.obj != null) subProps = GetPropertyItems(p.prop, p.obj);
                
            if (subProps != null && subProps.Count > 0)
            {
                DrawProperty(subProps, p, indent);
            }
            else
            {
                EditorGUI.indentLevel = indent;

                if (p.ParentAttrs.listItemAttrs.TryGetValue(p.prop.name, out var listAttr))
                {
                    DrawListPropertyItem(p, listAttr, indent);
                }
                else if (p.ParentAttrs.reorderableItemAttrs.TryGetValue(p.prop.name, out var reorderableAttr))
                {
                    DrawReorderablePropertyItem(p, reorderableAttr, indent);
                }
                else
                {
                    EditorGUILayout.PropertyField(p.prop, 
                        new GUIContent(IsDebug ? $"[{indent}][D] {p.DisplayName}" : p.DisplayName), true);
                }
            }
            EditorGUILayout.EndVertical();
            EditorGUI.indentLevel = preIndent;
        }

    }
}