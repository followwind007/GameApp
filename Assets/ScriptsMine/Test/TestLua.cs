
using UnityEngine;
using Framework.Manager;
using GameApp.DataBinder;
using UnityEngine.UI;

public class TestLua : MonoBehaviour {
    public BehaviourBinder binder;

    private void Start()
    {
        var manager = LuaManager.Instance;
        Debug.Log(manager.State);
    }

    private void OnGUI()
    {
        if (GUILayout.Button("Init", GUILayout.Width(100), GUILayout.Height(50)))
        {
            var manager = LuaManager.Instance;
            Debug.Log(manager.State);
        }
        if (GUILayout.Button("Dump", GUILayout.Width(100), GUILayout.Height(50)))
        {
            Debug.Log(binder["img"]);
            Debug.Log(((Text) binder["text"]).text);
        }
        
    }


}
