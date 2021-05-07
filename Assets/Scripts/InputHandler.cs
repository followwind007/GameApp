using UnityEngine;
using Controller;

[RequireComponent(typeof(StateManager))]
[RequireComponent(typeof(HandleMovement))]
public class InputHandler : MonoBehaviour {
    private StateManager _states;

    [HideInInspector]
    public Transform camHolder;

    private void Start ()
    {
        _states = GetComponent<StateManager>();
        if (Camera.main != null) camHolder = Camera.main.transform;
    }

    private void Update () 
    {
        HandleAxis();
    }

    private void HandleAxis()
    {
        _states.horizontal = Input.GetAxis("Horizontal");
        _states.vertical = Input.GetAxis("Vertical");
    }
}
