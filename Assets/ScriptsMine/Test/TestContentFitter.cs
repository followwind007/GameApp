using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class TestContentFitter : MonoBehaviour {

    public Text text;

    private string _str;

    private void OnGUI()
    {
        _str = GUILayout.TextField(_str, GUILayout.Width(500), GUILayout.Height(50));
        if (GUILayout.Button("Add", GUILayout.Width(100), GUILayout.Height(50)))
        {
            text.text = text.text + _str;
        }
        if (GUILayout.Button("Set", GUILayout.Width(100), GUILayout.Height(50)))
        {
            text.text = _str;
        }
    }

}
