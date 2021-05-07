using UnityEditor;

namespace GameApp.Timeline
{
    [CustomEditor(typeof(MessagePlayableAsset))]
    public class MessagePlayableAssetEditor : Editor
    {
        private SerializedProperty _name;
        private SerializedProperty _exposes;
        private SerializedProperty _values;

        private void OnEnable()
        {
            _name = serializedObject.FindProperty("messageName");
            _exposes = serializedObject.FindProperty("exposes");
            _values = serializedObject.FindProperty("values");
        }

        public override void OnInspectorGUI()
        {
            serializedObject.Update();

            EditorGUILayout.PropertyField(_name, true);
            EditorGUILayout.PropertyField(_exposes, true);
            EditorGUILayout.PropertyField(_values, true);

            serializedObject.ApplyModifiedProperties();
        }

    }

}