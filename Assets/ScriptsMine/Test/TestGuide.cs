using GameApp.Guide;
using UnityEngine;

public class TestGuide : MonoBehaviour
{   
    private string _maskItemId = "btn";
    private string _maskItemParams = "";
    
    private void OnGUI()
    {
        _maskItemId = GUILayout.TextField(_maskItemId);
        _maskItemParams = GUILayout.TextField(_maskItemParams);

        if (GUILayout.Button("Retain"))
        {
            GuideMaskManager.Instance.RetainMaskItem(_maskItemId, _maskItemParams);
        }

        if (GUILayout.Button("Release"))
        {
            GuideMaskManager.Instance.ReleaseMaskItem(_maskItemId, _maskItemParams);
        }

        if (GUILayout.Button("Analyse"))
        {
            foreach (var kv in GuideMaskManager.Instance.targetItems)
            {
                Debug.Log(kv.Key + ": " + kv.Value.Count);
            }
        }
        
    }
}
