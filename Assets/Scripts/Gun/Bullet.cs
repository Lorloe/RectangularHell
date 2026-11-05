using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Bullet : MonoBehaviour
{
    [SerializeField] private float _bulletSpeed = 20f;
    [SerializeField] private int _damageAmount = 1;

    private Gun _gun;

    private Vector2 _fireDirection;

    private Rigidbody2D _rb;

    private void Awake()
    {
        _rb = GetComponent<Rigidbody2D>();
    }

    public void Init(Gun gun, Vector2 bulletSpawnPoint, Vector2 mousePosition)
    {
        _gun = gun;
        transform.position = bulletSpawnPoint;
        _fireDirection = (mousePosition - bulletSpawnPoint).normalized;
    }

    // private void Start() 
    // {
    //     if (PlayerController.Instance.IsFacingRight()) {
    //         _fireDirection = Vector2.right;
    //     } else {
    //         _fireDirection = Vector2.left;
    //     }
    // }

    private void FixedUpdate()
    {
        _rb.linearVelocity = _fireDirection * _bulletSpeed;
    }

    private void OnTriggerEnter2D(Collider2D other) 
    {
        Health health = other.gameObject.GetComponent<Health>();
        health?.TakeDamage(_damageAmount);
        _gun.ReleaseBulletFromPool(this);
    }
}