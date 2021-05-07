using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Pangu.SceneObject;

public class TestAreaLock : MonoBehaviour {

    public AreaWallObject aw;

    private string _areaId = "";

    private void OnGUI()
    {
        _areaId = GUILayout.TextField(_areaId, GUILayout.Width(100));
        
        if (GUILayout.Button("Lock", GUILayout.Width(100), GUILayout.Height(50)))
        {
            SceneAreaManager.Instance.SetAreaLockState(System.Convert.ToInt32(_areaId), true);
            if (aw)
            {
                Debug.Log(aw.CanDissolve);
            }
        }
        if (GUILayout.Button("UnLock", GUILayout.Width(100), GUILayout.Height(50)))
        {
            SceneAreaManager.Instance.SetAreaLockState(System.Convert.ToInt32(_areaId), false);
            if (aw)
            {
                Debug.Log(aw.CanDissolve);
            }
        }
    }

}
