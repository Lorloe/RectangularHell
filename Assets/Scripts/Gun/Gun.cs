using System;
using UnityEngine;
using UnityEngine.InputSystem;

public class Gun : MonoBehaviour
{
    // public static Action OnShoot;
    public event Action OnShoot;
    public Transform BulletSpawnPoint => _bulletSpawnPoint;

    [SerializeField] private Transform _bulletSpawnPoint;
    [SerializeField] private Bullet _bulletPrefab;
    [SerializeField] private float _gunFireCD = 0.3f;

    private static readonly int FIRE_HASH = Animator.StringToHash("Fire");

    private InputAction _shootAction;
    private float _lastFireTime = 0f;
    private Camera _mainCamera;

    private Animator _animator;

    public void Awake()
    {
        _shootAction = InputSystem.actions.FindAction("Shoot");
        _mainCamera = Camera.main;
        _animator = GetComponent<Animator>();
    }

    private void Update()
    {
        Shoot();
        RotateGunTowardCursor();
    }

    private void OnEnable()
    {
        OnShoot += ShootProjectile;
        OnShoot += ResetLastFireTime;
        OnShoot += FireAnimation;
        // Debug.Log($"[SUB] {name} ({GetInstanceID()}) listeners: {OnShoot?.GetInvocationList().Length}");
    }

    private void OnDisable()
    {
        OnShoot -= ShootProjectile;
        OnShoot -= ResetLastFireTime;
        OnShoot -= FireAnimation;
        // Debug.Log($"[UNSUB] {name} ({GetInstanceID()}) listeners: {OnShoot?.GetInvocationList().Length}");
    }

    private void Shoot()
    {
        if (_shootAction.WasPressedThisFrame() && Time.time >= _lastFireTime + _gunFireCD) 
        {
            // ShootProjectile();
            // ResetLastFireTime();
            OnShoot?.Invoke();
            // Optional: Add shooting sound or animation or screen shake here
        }
    }

    private void ShootProjectile()
    {
        Bullet newBullet = Instantiate(_bulletPrefab, _bulletSpawnPoint.position, Quaternion.identity);
        Vector3 mouseWorld = _mainCamera.ScreenToWorldPoint(Mouse.current.position.ReadValue());
        mouseWorld.z = 0f;
        newBullet.Init(_bulletSpawnPoint.position, mouseWorld);
    }

    private void ResetLastFireTime()
    {
        _lastFireTime = Time.time;
    }

    #region ANIMATION

    private void FireAnimation()
    {
        _animator.Play(FIRE_HASH, 0, 0.5f);
    }

    #endregion

    #region GUN ROTATION
    
    private void RotateGunTowardCursor()
    {
        if (_mainCamera == null)
        {
            _mainCamera = Camera.main;
            if (_mainCamera == null) return;
        }

        Vector3 mouseWorld = _mainCamera.ScreenToWorldPoint(Mouse.current.position.ReadValue());
        mouseWorld.z = 0f;

        Vector2 worldDirection = (Vector2)(mouseWorld - transform.position);
        Vector2 localDirection = transform.parent != null
            ? transform.parent.InverseTransformDirection(worldDirection)
            : worldDirection;

        float angle = Mathf.Clamp(Mathf.Atan2(localDirection.y, localDirection.x) * Mathf.Rad2Deg, -75f, 75f);
        transform.localRotation = Quaternion.Euler(0, 0, angle);
    }

    #endregion
}
