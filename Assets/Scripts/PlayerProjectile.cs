using UnityEngine;

public class PlayerProjectile : MonoBehaviour
{
    [SerializeField] int _damage = 5;
    [SerializeField] float _speed = 10f;

    private Rigidbody _rigidbody;


    private void Start()
    {
        _rigidbody = GetComponent<Rigidbody>();
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

        Destroy(gameObject);
    }
}
