using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Pangu.Manager;

public class TestEventManager : MonoBehaviour {

    private void Awake()
    {
        //EventManager.Instance.Add("test", Test1);
        //EventManager.Instance.Add("test", Test1);
        //EventManager.Instance.Add("test", Test2);
    }

    private void OnGUI()
    {
        if (GUILayout.Button("test", GUILayout.Width(100), GUILayout.Height(50)))
        {
            //EventManager.Instance.Dispatch("test");
        }   
    }

    private void Test1(Dictionary<string, object> dict)
    {
        Debug.Log("in test1 " + dict);
    }

    private void Test2(Dictionary<string, object> dict)
    {
        Debug.Log("in test2 " + dict);
    }

}
