using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Framework.Manager;

public class TestTimeline : MonoBehaviour {

    private string _event = "test";

    //private float _radius = 5f;

    private void OnGUI()
    {
        _event = GUILayout.TextField(_event, GUILayout.Width(100), GUILayout.Height(30));
        
        if (GUILayout.Button("trigger event", GUILayout.Width(100), GUILayout.Height(50)))
        {
            ScenePlayableManager.Instance.OnReceiveEventPlayable(_event);
        }
        if (GUILayout.Button("trigger nearby", GUILayout.Width(100), GUILayout.Height(50)))
        {
            ScenePlayableManager.Instance.TriggerEventNearby(gameObject, _event, 10f, true);
        }

    }

    private void Start()
    {
        
        
    }

    private void OnDrawGizmos()
    {
        
    }


}
