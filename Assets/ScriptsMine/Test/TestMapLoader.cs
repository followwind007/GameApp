using UnityEngine;
using System.Collections;

public class TestMapLoader : MonoBehaviour
{
    public MiniMapLoader loader;

    public Transform playerTrans;

    private void Start()
    {
        if (loader)
        {
            loader.SetMapConfig(1001, playerTrans);
        }
    }

    private void OnGUI()
    {
        
    }

}
