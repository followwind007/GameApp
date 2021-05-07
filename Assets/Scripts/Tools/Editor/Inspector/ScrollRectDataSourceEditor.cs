using UnityEditor;

namespace Pangu.GUIs
{
    [CustomEditor(typeof(ScrollRectDataSource))]
    public class ScrollRectDataSourceEditor : Editor
    {
        private SerializedObject _serObj;
        private SerializedProperty _loadCellFromPrefabProp;
        private SerializedProperty _cellPathProp;
        private SerializedProperty _cellViewProp;

        private SerializedProperty _useGridLayoutProp;
        private SerializedProperty _lineGridCellCountProp;
        private SerializedProperty _loadGridCellFromPrefabProp;
        private SerializedProperty _gridCellPathProp;
        private SerializedProperty _gridCellViewProp;

        private void OnEnable()
        {
            _serObj = new SerializedObject(target as ScrollRectDataSource);
            _loadCellFromPrefabProp = _serObj.FindProperty("loadCellFromPrefab");
            _cellPathProp = _serObj.FindProperty("cellPath");
            _cellViewProp = _serObj.FindProperty("cellView");

            _useGridLayoutProp = _serObj.FindProperty("useGridLayout");
            _lineGridCellCountProp = _serObj.FindProperty("lineGridCellCount");
            _loadGridCellFromPrefabProp = _serObj.FindProperty("loadGridCellFromPrefab");
            _gridCellPathProp = _serObj.FindProperty("gridCellPath");
            _gridCellViewProp = _serObj.FindProperty("gridCellView");
        }

        public override void OnInspectorGUI()
        {
            _serObj.Update();

            EditorGUILayout.PropertyField(_loadCellFromPrefabProp);
            EditorGUILayout.PropertyField(_loadCellFromPrefabProp.boolValue ? _cellPathProp : _cellViewProp);

            EditorGUILayout.PropertyField(_useGridLayoutProp);
            if (_useGridLayoutProp.boolValue)
            {
                EditorGUILayout.PropertyField(_lineGridCellCountProp);
                EditorGUILayout.PropertyField(_loadGridCellFromPrefabProp);
                EditorGUILayout.PropertyField(_loadGridCellFromPrefabProp.boolValue
                    ? _gridCellPathProp
                    : _gridCellViewProp);
            }

            _serObj.ApplyModifiedProperties();
        }

    }
}