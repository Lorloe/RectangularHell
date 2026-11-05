using UnityEngine;
using UnityEngine.InputSystem;

public class PlayerController : MonoBehaviour
{
    public static PlayerController Instance;

    [Header("Movement Settings")]
    [SerializeField] private float _moveSpeed = 10f;
    [SerializeField] private float _jumpForce = 20f;

    [Header("Ground Check Settings")]
    [SerializeField] private Transform _feetCheck;
    [SerializeField] private Vector2 _groundCheck;
    [SerializeField] private LayerMask _groundLayer;

    private InputAction _moveAction, _jumpAction;
    private Vector2 _movement;

    private Rigidbody2D _rb;

    // private void Start()
    // {
    //     InputSystem.actions.FindAction("Jump").performed += ctx => OnJump();
    // }

    private void Awake() 
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
        OnMove();
    }

    private void GatherInput()
    {
        Vector2 moveInput = _moveAction.ReadValue<Vector2>();

        _movement = new Vector2 (moveInput.x * _moveSpeed, _rb.linearVelocity.y);
    }

    #region ACTION

    private void OnMove() 
    {
        _rb.linearVelocity = _movement;
    }

    private void OnJump()
    {
        if (_jumpAction.WasPressedThisFrame() && CheckGrounded()) {
            _rb.AddForce(Vector2.up * _jumpForce, ForceMode2D.Impulse);
        }
    }

    #endregion

    #region SPRITE FLIP

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

    public bool IsFacingRight()
    {
        return transform.eulerAngles.y < 90f || transform.eulerAngles.y > 270f;
    }

    #endregion

    #region GROUND CHECK PHYSICS
    
    private bool CheckGrounded()
    {
        Collider2D isGrounded = Physics2D.OverlapBox(_feetCheck.position, _groundCheck, 0f, _groundLayer);
        return isGrounded != null;
    }

    private void OnDrawGizmos()
    {
        if (_feetCheck == null) return;

        Gizmos.color = Color.red;
        Gizmos.DrawWireCube(_feetCheck.position, _groundCheck);
    }

    #endregion
}
