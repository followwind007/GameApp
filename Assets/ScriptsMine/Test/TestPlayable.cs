using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class TestPlayable : MonoBehaviour {

    public GameObject target;

    private void OnGUI()
    {
        if (GUILayout.Button("SetVisible", GUILayout.Width(100), GUILayout.Height(50)))
        {
            target.SetVisible(true);
        }
        if (GUILayout.Button("SetInvisible", GUILayout.Width(100), GUILayout.Height(50)))
        {
            target.SetVisible(false);
        }
    }

}
