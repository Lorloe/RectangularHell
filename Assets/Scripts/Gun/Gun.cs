using System;
using UnityEngine;
using UnityEngine.InputSystem;
using UnityEngine.Pool;
using Unity.Cinemachine;

public class Gun : MonoBehaviour
{
    // public static Action OnShoot;
    public event Action OnShoot;
    // public Transform BulletSpawnPoint => _bulletSpawnPoint;

    [SerializeField] private Transform _bulletSpawnPoint;
    [SerializeField] private Bullet _bulletPrefab;
    [SerializeField] private float _gunFireCD = 0.3f;

    private ObjectPool<Bullet> _bulletPool;

    private static readonly int FIRE_HASH = Animator.StringToHash("Fire");

    private InputAction _shootAction;
    private float _lastFireTime = 0f;
    private Camera _mainCamera;
    private CinemachineImpulseSource _impulseSource;
    private Animator _animator;

    public void Awake()
    {
        _shootAction = InputSystem.actions.FindAction("Shoot");
        _mainCamera = Camera.main;
        _impulseSource = GetComponent<CinemachineImpulseSource>();
        _animator = GetComponent<Animator>();
    }

    private void Start()
    {
        CreateBulletPool();
    }

    private void Update()
    {
        Shoot();
        RotateGunTowardCursor();
    }

    private void OnEnable()
    {
        _shootAction.Enable();
        OnShoot += ShootProjectile;
        OnShoot += ResetLastFireTime;
        OnShoot += FireAnimation;
        OnShoot += GunScreenShake;
    }

    private void OnDisable()
    {
        _shootAction.Disable();
        OnShoot -= ShootProjectile;
        OnShoot -= ResetLastFireTime;
        OnShoot -= FireAnimation;
        OnShoot -= GunScreenShake;
    }

    #region SHOOTING

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
        // Bullet newBullet = Instantiate(_bulletPrefab, _bulletSpawnPoint.position, Quaternion.identity);
        Bullet newBullet = _bulletPool.Get();
        Vector3 mouseWorld = _mainCamera.ScreenToWorldPoint(Mouse.current.position.ReadValue());
        mouseWorld.z = 0f;
        newBullet.Init(this,_bulletSpawnPoint.position, mouseWorld);
    }

    private void ResetLastFireTime()
    {
        _lastFireTime = Time.time;
    }

    #endregion

    #region BULLET POOL

    // private void CreateBulletPool()
    // {
    //     _bulletPool = new ObjectPool<Bullet>(
    //         // createFunc: () => Instantiate(_bulletPrefab, _bulletSpawnPoint.position, Quaternion.identity),
    //         // actionOnGet: Bullet => Bullet.gameObject.SetActive(true),
    //         // actionOnRelease: Bullet => Bullet.gameObject.SetActive(false),
    //         // actionOnDestroy: Bullet => Destroy(Bullet.gameObject),
    //         // collectionCheck: false,
    //         // defaultCapacity: 10,
    //         // maxSize: 20
    //         CreateFunc,
    //         ActiveBullet,
    //         DeactiveBullet,
    //         DestroyBullet,
    //         CollectionCheck,
    //         DefaultCapacity,
    //         MaxSize
    //     );
    // }

    private void CreateBulletPool()
    {
        _bulletPool = new ObjectPool<Bullet>(() =>
        {
            return Instantiate(_bulletPrefab, _bulletSpawnPoint.position, Quaternion.identity);
        },
        bullet =>
        {
            bullet.gameObject.SetActive(true);
        },
        bullet =>
        {
            bullet.gameObject.SetActive(false);
        },
        bullet =>
        {
            Destroy(bullet.gameObject);
        },
        false,
        10,
        20
        );
    }

    public void ReleaseBulletFromPool(Bullet bullet)
    {
        _bulletPool.Release(bullet);
    }

    // private Bullet CreateFunc()
    // {
    //     return Instantiate(_bulletPrefab, _bulletSpawnPoint.position, Quaternion.identity);
    // }

    // private void ActiveBullet(Bullet bullet)
    // {
    //     bullet.gameObject.SetActive(true);
    // }

    // private void DeactiveBullet(Bullet bullet)
    // {
    //     bullet.gameObject.SetActive(false);
    // }

    // private void DestroyBullet(Bullet bullet)
    // {
    //     Destroy(bullet.gameObject);
    // }

    // private bool CollectionCheck => false;
    // private int DefaultCapacity => 10;
    // private int MaxSize => 20;

    #endregion

    #region ANIMATION

    private void FireAnimation()
    {
        _animator.Play(FIRE_HASH, 0, 0.5f);
    }

    #endregion

    #region SCREEN SHAKE

    private void GunScreenShake()
    {
        _impulseSource.GenerateImpulse();
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
