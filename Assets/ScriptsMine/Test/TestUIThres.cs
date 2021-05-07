using UnityEngine;
using UnityEngine.UI;
using System.Collections;

public class TestUIThres : MonoBehaviour
{
    public UIRawImage img;

    private void OnGUI()
    {
        if (GUILayout.Button("change color", GUILayout.Width(100), GUILayout.Height(50)))
        {
            img.color = Color.black;
        }

        GUILayout.Label(Screen.width + ":" + Screen.height);
    }

}
