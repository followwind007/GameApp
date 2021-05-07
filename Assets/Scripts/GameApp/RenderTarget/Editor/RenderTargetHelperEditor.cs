using UnityEditor;

namespace GameApp.RenderTarget
{
    //[CustomEditor(typeof(RenderTargetHelper))]
    public class RenderTargetHelperEditor : Editor
    {
        private SerializedProperty _loadResourceProp;
        private SerializedProperty _isUniqueProp;
        private SerializedProperty _renderTargetProp;
        private SerializedProperty _targetProp;

        private void OnEnable()
        {
            var rh = target as RenderTargetHelper;
            _loadResourceProp = serializedObject.FindProperty("loadResource");
            _isUniqueProp = serializedObject.FindProperty("isUnique");
            _renderTargetProp = serializedObject.FindProperty("renderTarget");
            _targetProp = serializedObject.FindProperty("target");
        }

        public override void OnInspectorGUI()
        {
            serializedObject.Update();

            EditorGUILayout.PropertyField(_isUniqueProp);

            EditorGUILayout.PropertyField(_loadResourceProp);
            EditorGUILayout.PropertyField(!_loadResourceProp.boolValue ? _renderTargetProp : _targetProp);
            serializedObject.ApplyModifiedProperties();
        }

    }
}