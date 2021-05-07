using UnityEngine;

public class TestComponent : MonoBehaviour
{
    private void Awake()
    {
        Debug.Log($"Comp Awake: {gameObject.name}");
    }
}
