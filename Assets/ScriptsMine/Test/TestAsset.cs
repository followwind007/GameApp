using UnityEngine;
using UnityEngine.Serialization;
using UnityEngine.UI;

public class TestAsset : MonoBehaviour
{
    [PathRef(typeof(GameObject))]
    public string path;

    [FormerlySerializedAs("_prefab")] public GameObject prefab;
    
    private void OnGUI()
    {
        #if UNITY_EDITOR 
        if (GUILayout.Button("Load", GUILayout.Width(100)))
        {
            prefab = UnityEditor.AssetDatabase.LoadAssetAtPath<GameObject>(path);
        }
        #endif

        if (GUILayout.Button("Instantiate", GUILayout.Width(100)))
        {
            var go = Instantiate(prefab);
            go.GetComponent<RectTransform>().SetParentFull(GameObject.Find("Canvas").GetComponent<RectTransform>());
        }
    }
}
