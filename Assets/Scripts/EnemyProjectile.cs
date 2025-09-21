using RenderHeads.Services;
using System.Collections.Generic;
using UnityEngine;

public class EnemyProjectile : MonoBehaviour
{
    [Header("Settings")]
    [SerializeField] float _speed = 10f;

    [SerializeField] List<AudioClip> _launchClips;
    [SerializeField] AudioClip _hitClip;
    public void SetDamage(int d) => _damage = d;
    private int _damage = 5;
    private Rigidbody _rigidbody;

    private LazyService<GameManager> _gameManager;

    private void Start()
    {
        _rigidbody = GetComponent<Rigidbody>();

        Destroy(gameObject ,5f);
    }

    private void FixedUpdate()
    {
        if (_rigidbody.linearVelocity.sqrMagnitude < _speed * _speed)
        {
            Vector3 forceDirection = transform.forward;

            _rigidbody.AddForce(forceDirection * _speed, ForceMode.Force);
        }
    }

    private void OnCollisionEnter(Collision collision)
    {
        if (collision.collider.CompareTag("Enemy"))
            return;


        if (collision.collider.CompareTag("Player"))
        {
            if (collision.collider.TryGetComponent<PlayerController>(out PlayerController player))
            {
                player.TakeDamage(_damage);
            }
        }

        _gameManager.Value.PlayAudioClip(_hitClip);
        Destroy(gameObject);
    }
}
