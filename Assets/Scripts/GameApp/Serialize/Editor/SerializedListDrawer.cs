using System;
using GameApp.Util;
using UnityEditor;
using UnityEngine;
using UnityEngine.UIElements;

namespace GameApp.Serialize
{
    [CustomPropertyDrawer(typeof(SerlizedList))]
    public class SerializedListDrawer : PropertyDrawer
    {
        public class SerlizedListView : VisualElement
        {
            public readonly ReorderableListView rl;
            public readonly SerializedProperty property;
            public SerlizedListView(SerializedProperty property)
            {
                this.property = property;
                
                var addMenu = new GenericMenu();

                DrawerTypesUtil.SupportedTypes.ForEach(t =>
                {
                    addMenu.AddItem(new GUIContent(t.FullName), false, data => { AddNewItem(t); }, t);
                });
                
                rl = new ReorderableListView(property.FindPropertyRelative("list")) {addMenu = addMenu};
                Add(rl);
            }

            public void AddNewItem(Type t)
            {
                rl.property.arraySize++;
                
                var item = rl.property.GetArrayElementAtIndex(rl.property.arraySize - 1);
                //do some essential initialize work
                item.FindPropertyRelative(SerializedJsonObject.IdType).stringValue = t.FullName;
                item.FindPropertyRelative(SerializedJsonObject.IdData).stringValue = null;
                item.FindPropertyRelative(SerializedJsonObject.IdObj).objectReferenceValue = null;
                
                property.serializedObject.ApplyModifiedProperties();
                rl.Refresh();
            }
        }
        
        public override VisualElement CreatePropertyGUI(SerializedProperty property)
        {
            return new SerlizedListView(property);
        }
        
    }
}