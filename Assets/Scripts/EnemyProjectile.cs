using UnityEngine;

public class EnemyProjectile : MonoBehaviour
{
    [SerializeField] float _speed = 10f;

    public void SetDamage(int d) => d = _damage;
    private int _damage = 5;
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
        if (collision.collider.CompareTag("Enemy"))
            return;


        if (collision.collider.CompareTag("Player"))
        {
            if (collision.collider.TryGetComponent<PlayerController>(out PlayerController player))
            {
                player.TakeDamage(_damage);
            }
        }

        Destroy(gameObject);
    }
}
