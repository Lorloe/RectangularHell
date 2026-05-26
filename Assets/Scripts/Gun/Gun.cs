using UnityEngine;
using UnityEngine.InputSystem;

public class Gun : MonoBehaviour
{
    public Transform BulletSpawnPoint => _bulletSpawnPoint;

    [SerializeField] private Transform _bulletSpawnPoint;
    [SerializeField] private Bullet _bulletPrefab;

    private InputAction _shootAction;
    private CursorManager _cursorManager;
    private Vector2 _mousePosition;

    public void Awake()
    {
        _shootAction = InputSystem.actions.FindAction("Shoot");
        _cursorManager = FindFirstObjectByType<CursorManager>();
    }

    private void Update()
    {
        Shoot();
        RotateGunTowardCursor();
    }

    private void Shoot()
    {
        if (_shootAction.WasPressedThisFrame()) 
        {
            ShootProjectile();
        }
    }

    private void ShootProjectile()
    {
        Bullet newBullet = Instantiate(_bulletPrefab, _bulletSpawnPoint.position, Quaternion.identity);
        newBullet.Init(_bulletSpawnPoint.position, Mouse.current.position.ReadValue());
    }

    private void RotateGunTowardCursor()
    {
        _mousePosition = Mouse.current.position.ReadValue();
        // Vector2 direction = _mousePosition - (Vector2) PlayerController.Instance.transform.position;
        Vector2 direction = PlayerController.Instance.transform.InverseTransformPoint(_mousePosition);

        float angle = Mathf.Clamp(Mathf.Atan2(direction.y, direction.x) * Mathf.Rad2Deg, -75f, 75f);
        transform.localRotation = Quaternion.Euler(0, 0, angle);
    }
}
