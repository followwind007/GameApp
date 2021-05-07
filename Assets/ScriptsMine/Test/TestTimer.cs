
using GameApp.Util;
using UnityEngine;

public class TestTimer : MonoBehaviour
{
    private void Start()
    {
        Timer.Add(1, () => Debug.Log("do"));
        Timer.Add(2, () => Debug.Log("infinite"), -1);
    }
}
