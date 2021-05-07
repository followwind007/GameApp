using System;
using System.Collections.Generic;
using System.Linq;
using GameApp.Util;
using UnityEditor;
using UnityEngine;

namespace GameApp.AssetProcessor.Drawer.Property
{
    [CustomPropertyDrawer(typeof(AssetProcessorSetting.ComponentProcessorItem))]
    public class ComponentProcessorItemDrawer : PropertyDrawer, ICheckerSelector
    {
        private float LineHeight => EditorGUIUtility.singleLineHeight + 2;
        private float FieldHeight => LineHeight - 2;

        private string _checkerIds;
        
        public override float GetPropertyHeight(SerializedProperty property, GUIContent label)
        {
            return LineHeight * 3;
        }

        public override void OnGUI(Rect position, SerializedProperty property, GUIContent label)
        {
            var rect1 = new Rect(position.x, position.y, position.width - LineHeight, FieldHeight);
            var prop1 = property.FindPropertyRelative("type");
            prop1.stringValue = EditorGUI.TextField(rect1,"Type",prop1.stringValue);
            
            var rect12 = new Rect(rect1.x + rect1.width, rect1.y, LineHeight, FieldHeight);
            if (GUI.Button(rect12, ">"))
            {
                GetTypeMenu((t) =>
                {
                    prop1.stringValue = t;
                    prop1.serializedObject.ApplyModifiedProperties();
                }).ShowAsContext();
            }
            
            var rect2 = new Rect(position.x, position.y + LineHeight, position.width, FieldHeight);
            var prop2 = property.FindPropertyRelative("path");
            prop2.stringValue = EditorGUI.TextField(rect2, "Path", prop2.stringValue);

            var rect3 = new Rect(position.x, position.y + LineHeight * 2, position.width - LineHeight, FieldHeight);
            var prop3 = property.FindPropertyRelative("checkerIds");

            var rect32 = new Rect(rect3.x + rect3.width, rect3.y, LineHeight, FieldHeight);
            if (GUI.Button(rect32, ">"))
            {
                this.GetCheckerMenu(prop3, () =>
                {
                    var list = new List<string>();
                    var t = TypeUtil.GetType(prop1.stringValue);
                    if (t != null && CompileEntry.ComponentCheckers.TryGetValue(t, out var cks))
                    {
                        list = cks.Select(item => item.attribute.id).ToList();
                    }
                    return list;
                }).ShowAsContext();
            }
            
            EditorGUI.BeginChangeCheck();
            _checkerIds = this.ArrayToString(prop3);
            _checkerIds = EditorGUI.TextField(rect3, "Checkers", _checkerIds);
            if (EditorGUI.EndChangeCheck()) this.StringToArray(prop3, _checkerIds);
        }
        
        private GenericMenu GetTypeMenu(Action<string> onSelect)
        {
            var gm = new GenericMenu();
            var cks = CompileEntry.ComponentCheckers;
            foreach (var kv in cks)
            {
                gm.AddItem(new GUIContent(kv.Key.ToString()),false, () => { onSelect?.Invoke(kv.Key.ToString()); });
            }

            return gm;
        }
        
    }
}