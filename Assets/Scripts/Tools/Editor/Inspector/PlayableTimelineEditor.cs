#if UNITY_EDITOR
using UnityEngine;
using UnityEditor;
using UnityEngine.Playables;

namespace GameApp.ScenePlayable
{
    [CustomEditor(typeof(PlayableTimeline), true)]
    public class PlayableTimelineEditor : ScenePlayableObjectEditor
    {
        private PlayableDirector _director;

        private void OnEnable()
        {
            _director = (target as PlayableTimeline).gameObject.GetComponent<PlayableDirector>();
        }

        public override void OnInspectorGUI()
        {
            base.OnInspectorGUI();

            Color bfColor = GUI.color;
            GUILayout.BeginHorizontal();
            GUI.color = Color.green;
            if (GUILayout.Button("Load", GUILayout.Height(20), GUILayout.Width(60)))
            {
                PlayableAsset asset = PlayableLoader.LoadAssetAtPath<PlayableAsset>((target as PlayableTimeline).timelinePath);
                _director.playableAsset = asset;
            }

            GUI.color = Color.gray;
            if (GUILayout.Button("Clear", GUILayout.Height(20), GUILayout.Width(60)))
            {
                _director.playableAsset = null;
            }

            GUI.color = bfColor;
            GUILayout.EndHorizontal();
        }
    }
}
#endif