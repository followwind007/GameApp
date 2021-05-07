using System;
using UnityEngine;
using System.Collections;

public class TestKinematic : MonoBehaviour
{
    public PlayerKinmeticMove move;

    private string _pos;

    private void OnGUI()
    {
        _pos = GUILayout.TextField(_pos, GUILayout.Width(200), GUILayout.Height(30));
        
        if (GUILayout.Button("move", GUILayout.Width(100), GUILayout.Height(50)))
        {
            string[] posStrs = _pos.Split(',');
            Vector3 pos = new Vector3(Convert.ToSingle(posStrs[0]), Convert.ToSingle(posStrs[1]), Convert.ToSingle(posStrs[2]));
            Debug.Log("start time:" + Time.time);
            move.StartMoveToPosition(pos, 3, () => { Debug.Log("finish time:" + Time.time); });
        }
    }

}
