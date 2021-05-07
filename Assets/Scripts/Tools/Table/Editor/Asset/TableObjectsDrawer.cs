using UnityEngine;
using UnityEditor;
using UnityEditorInternal;

namespace Tools.Table.Asset
{
    [CustomPropertyDrawer(typeof(TableConfigs))]
    public class TableObjectsDrawer : PropertyDrawer
    {
        private ReorderableList _list;

        readonly float _headWidth = 34f;
        readonly float _isUseWidth = 50f;

        public override void OnGUI(Rect position, SerializedProperty property, GUIContent label)
        {
            Init(property);
            _list.DoList(position);
            property.serializedObject.ApplyModifiedProperties();
        }

        public override float GetPropertyHeight(SerializedProperty property, GUIContent label)
        {
            Init(property);
            return _list.GetHeight();
        }

        private void Init(SerializedProperty property)
        {
            if (_list != null) return;
            var listProp = property.FindPropertyRelative("list");
            _list = new ReorderableList(property.serializedObject, listProp)
            {
                drawElementCallback = DrawData,
                drawHeaderCallback = DrawHeader,
                elementHeight = EditorGUIUtility.singleLineHeight * 1 + 4,
            };
        }

        private void DrawData(Rect rect, int index, bool isActive, bool isFocused)
        {
            var data = _list.serializedProperty.GetArrayElementAtIndex(index);
            var tableNameProp = data.FindPropertyRelative("table");
            var sheetNameProp = data.FindPropertyRelative("sheet");
            var exportNameProp = data.FindPropertyRelative("export");
            var isUseProp = data.FindPropertyRelative("isUse");

            rect.height = EditorGUIUtility.singleLineHeight;
            var rects = GetCellRect(rect);
            EditorGUI.PropertyField(rects[0], tableNameProp, GUIContent.none);
            EditorGUI.PropertyField(rects[1], sheetNameProp, GUIContent.none);
            EditorGUI.PropertyField(rects[2], exportNameProp, GUIContent.none);
            EditorGUI.PropertyField(rects[3], isUseProp, GUIContent.none);
        }

        private void DrawHeader(Rect rect)
        {
            var rects = GetCellRect(rect);
            GUI.Label(rects[0], "Table");
            GUI.Label(rects[1], "Sheet");
            GUI.Label(rects[2], "Export");
            GUI.Label(rects[3], "In Use");
        }

        private Rect[] GetCellRect(Rect rect)
        {
            var textWidth = (rect.width + rect.x - _headWidth - _isUseWidth) / 3f - 2f;

            var rectTable = new Rect(_headWidth, rect.y, textWidth, rect.height);
            var rectSheet = new Rect(_headWidth + textWidth + 2f, rect.y, textWidth, rect.height);
            var rectExport = new Rect(_headWidth + textWidth * 2 + 4f, rect.y, textWidth, rect.height);
            var rectInUse = new Rect(_headWidth + textWidth * 3 + 6f, rect.y, _isUseWidth, rect.height);

            Rect[] rects = { rectTable, rectSheet, rectExport, rectInUse};
            return rects;
        }

    }
}