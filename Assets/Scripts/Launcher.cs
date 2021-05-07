
using Framework.Manager;
using UnityEngine;

public class Launcher : MonoBehaviour
{
    // Start is called before the first frame update
    private void Start()
    {
        var state = LuaManager.Instance.State;
        if (state == null)
        {
            Debug.LogWarning("null lua state");
        }
    }

    
}
