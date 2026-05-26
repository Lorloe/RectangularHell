using UnityEngine;
using UnityEngine.InputSystem;

public class PlayerController : MonoBehaviour
{
    public static PlayerController Instance;

    [Header("Movement Settings")]
    [SerializeField] private float _moveSpeed = 10f;
    [SerializeField] private float _jumpStrength = 10f;

    private InputAction _moveAction, _jumpAction;

    private bool _isGrounded = false;
    private Vector2 _movement;

    private Rigidbody2D _rb;

    public void Awake() 
    {
        if (Instance == null) { Instance = this; }
        _moveAction = InputSystem.actions.FindAction("Move");
        _jumpAction = InputSystem.actions.FindAction("Jump");
        _rb = GetComponent<Rigidbody2D>();
    }

    private void OnEnable() 
    {
        _moveAction.Enable();
        _jumpAction.Enable();
    }

    private void OnDisable() 
    {
        _moveAction.Disable();
        _jumpAction.Disable();
    }

    private void Update()
    {
        GatherInput();
        HandleSpriteFlip();
        OnJump();
    }

    private void FixedUpdate() 
    {
        Move();
    }

    private void GatherInput()
    {
        Vector2 moveInput = _moveAction.ReadValue<Vector2>();

        _movement = new Vector2 (moveInput.x * _moveSpeed, _rb.linearVelocity.y);
    }

    private void Move() 
    {
        _rb.linearVelocity = _movement;
    }

    private void OnJump()
    {
        if (_jumpAction.WasPressedThisFrame() && _isGrounded) {
            _rb.AddForce(Vector2.up * _jumpStrength, ForceMode2D.Impulse);
        }
    }

    private void HandleSpriteFlip()
    {
        Vector3 mousePosition = Camera.main.ScreenToWorldPoint(Mouse.current.position.ReadValue());

        if (mousePosition.x < transform.position.x)
        {
            transform.eulerAngles = new Vector3(0f, -180f, 0f);
        }
        else
        {
            transform.eulerAngles = new Vector3(0f, 0f, 0f);
        }
    }

    private void OnCollisionEnter2D(Collision2D other)
    {
        if (other.gameObject.layer == LayerMask.NameToLayer("Ground"))
        {
            _isGrounded = true;
        }
    }

    private void OnCollisionExit2D(Collision2D other)
    {
        if (other.gameObject.layer == LayerMask.NameToLayer("Ground"))
        {
            _isGrounded = false;
        }
    }

    public bool IsFacingRight()
    {
        return transform.eulerAngles.y == 0;
    } 
}
