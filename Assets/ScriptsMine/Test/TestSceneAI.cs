using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Framework.Manager;
using GameApp.ScenePlayable;

public class TestSceneAI : MonoBehaviour {

    private void OnGUI()
    {
        if (GUILayout.Button("Event Play", GUILayout.Width(80), GUILayout.Height(40)))
        {
            ScenePlayableManager.Instance.OnReceiveEventPlayable("test", ScenePlayableUtil.EVENT_OP_PLAY);
        }
        if (GUILayout.Button("Event Pause", GUILayout.Width(80), GUILayout.Height(40)))
        {
            ScenePlayableManager.Instance.OnReceiveEventPlayable("test", ScenePlayableUtil.EVENT_OP_PAUSE);
        }
        if (GUILayout.Button("Send Stop", GUILayout.Width(80), GUILayout.Height(40)))
        {
            ScenePlayableManager.Instance.OnReceiveEventPlayable("test", ScenePlayableUtil.EVENT_OP_STOP);
        }
    }

}
