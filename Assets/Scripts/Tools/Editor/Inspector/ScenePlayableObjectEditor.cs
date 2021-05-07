#if UNITY_EDITOR
using UnityEngine;
using UnityEditor;

namespace GameApp.ScenePlayable
{
    [CustomEditor(typeof(ScenePlayableObject), true)]
    public class ScenePlayableObjectEditor : Editor
    {
        public override void OnInspectorGUI()
        {
            base.OnInspectorGUI();
            var sp = target as ScenePlayableObject;

            if (sp.isTrigger && sp.GetComponent<Collider>() == null)
            {
                var col = sp.gameObject.AddComponent<SphereCollider>();
                col.isTrigger = true;
                col.radius = ScenePlayableUtil.DEFAULT_TRIGGER_RADIUS;
                sp.gameObject.layer = ScenePlayableUtil.RECEIVER_LAYER;
            }
            else if (!sp.isTrigger && sp.GetComponent<Collider>() != null)
            {
                var cols = sp.GetComponents<Collider>();
                foreach (var col in cols)
                {
                    DestroyImmediate(col);
                }
                sp.gameObject.layer = 0;
            }

            Color bfColor = GUI.color;
            GUILayout.BeginHorizontal();
            
            if (Application.isPlaying)
            {
                GUI.color = Color.green;
                if (GUILayout.Button("Play", GUILayout.Height(20), GUILayout.Width(60)))
                {
                    sp.Play();
                }

                GUI.color = Color.gray;
                if (GUILayout.Button("Pause", GUILayout.Height(20), GUILayout.Width(60)))
                {
                    sp.Pause();
                }

                GUI.color = Color.red;
                if (GUILayout.Button("Stop", GUILayout.Height(20), GUILayout.Width(60)))
                {
                    sp.Stop();
                }
            }

            GUI.color = bfColor;
            GUILayout.EndHorizontal();
        }

    }
}
#endif