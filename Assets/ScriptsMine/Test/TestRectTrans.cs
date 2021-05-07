using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class TestRectTrans : MonoBehaviour {

    public Transform itemTrans;
    public RectTransform rectTrans;
    public Canvas canvas;

    private void OnGUI()
    {
        if (GUILayout.Button("InverseTransformPoint"))
        {
            Debug.Log(rectTrans.position);
            Debug.Log(rectTrans.InverseTransformPoint(itemTrans.position));
        }

        if (GUILayout.Button("Pixel  Location"))
        {
            var rect = RectTransformUtility.PixelAdjustRect(rectTrans, canvas);
            Debug.Log($"{rect}");
        }
    }

}
