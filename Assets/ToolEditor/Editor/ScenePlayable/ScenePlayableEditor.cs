#if UNITY_EDITOR
using UnityEngine;
using UnityEditor;

namespace Pangu.ScenePlayable
{
    public class ScenePlayableEditor : ScriptableObject
    {
        [MenuItem("Tools/ScenePlayable/Preview")]
        private static void ShowPreviewWindow()
        {
            EditorWindow.GetWindow<SpPreviewWindow>("Scene Playable Preview");
        }
    }
}
#endif