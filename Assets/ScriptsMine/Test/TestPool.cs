using UnityEngine;
using GameApp.Pool;

public class TestPool : MonoBehaviour
{
    public GameObject prefab;

    public Transform newRoot;

    private GameObject _test;
    private void OnGUI()
    {
        if (GUILayout.Button("CreatePool", GUILayout.Width(100)))
        {
            prefab.SetActive(false);
            Pool.Create("Test", prefab, 2);
        }
        
        if (GUILayout.Button("Fetct", GUILayout.Width(100)))
        {
            _test = Pool.Fetch("Test");
            //_test.SetActive(true);
            _test.transform.SetParent(newRoot);
        }
        
        if (GUILayout.Button("Recycle", GUILayout.Width(100)))
        {
            Pool.Recycle("Test", _test);
        }

        if (GUILayout.Button("Prefab", GUILayout.Width(100)))
        {
            prefab.SetActive(false);
            Debug.Log(prefab);
        }
        
    }
}
