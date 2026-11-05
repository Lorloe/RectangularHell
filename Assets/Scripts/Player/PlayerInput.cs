using UnityEngine;
using UnityEngine.InputSystem;

public class PlayerInput : MonoBehaviour
{
    private InputSystem_Actions _playerInputAction;
    private InputAction _moveAction;
    private InputAction _jumpAction;

    private void Awake()
    {
        _playerInputAction = new InputSystem_Actions();

        // _moveAction = InputSystem.actions.FindAction("Move");
        _moveAction = _playerInputAction.Player.Move;
        _jumpAction = _playerInputAction.Player.Jump;
    }

    private void OnEnable()
    {
        _playerInputAction.Enable();
    }

    private void OnDisable()
    {
        _playerInputAction.Disable();
    }

    private void Update()
    {
        // Handle input actions here
    }
}
