using RenderHeads.Services;
using UnityEngine;

public class PlayerProjectile : MonoBehaviour
{
    [Header("Settings")]
    [SerializeField] float _speed = 10f;

    [SerializeField] AudioClip _launchClip;
    [SerializeField] AudioClip _hitClip;

    public void SetDamage(int d) => _damage = d;
    private int _damage = 5;
    private Rigidbody _rigidbody;

    private LazyService<GameManager> _gameManager;

    private void Start()
    {
        _rigidbody = GetComponent<Rigidbody>();

        Destroy(gameObject, 5f);

        _gameManager.Value.PlayAudioClip(_launchClip, 0.5f, true);
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
        if (collision.collider.CompareTag("Player"))
            return;

        if (collision.collider.CompareTag("Enemy"))
        {
            if (collision.collider.TryGetComponent<EnemyController>(out EnemyController enemy))
            {
                enemy.TakeDamage(_damage);
            }
        }
        else if (collision.collider.CompareTag("Door"))
        {
            if (collision.collider.TryGetComponent<Door>(out Door door))
            {
                door.TryBreak();
            }
        }
        else if (collision.collider.CompareTag("Switch"))
        {
            if (collision.collider.TryGetComponent<DoorSwitch>(out DoorSwitch doorSwitch))
            {
                doorSwitch.HitSwitch();
            }
        }

        _gameManager.Value.PlayAudioClip(_hitClip);
        Destroy(gameObject);
    }
}
