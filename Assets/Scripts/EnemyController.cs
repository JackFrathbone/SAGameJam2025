using UnityEngine;
using UnityEngine.AI;

public class EnemyController : MonoBehaviour
{
    [SerializeField] int _totalHealth = 100;
    private int _currentHealth;

    private NavMeshAgent _agent;
    private PlayerController _playerController;

    [SerializeField] float _detectionRadius = 10f;

    public bool GetAlertedStatus() => _alerted;
    private bool _alerted;

    private void Start()
    {
        _currentHealth = _totalHealth;

        _agent = GetComponent<NavMeshAgent>();
        _playerController = FindAnyObjectByType<PlayerController>();

        InvokeRepeating("CheckForPlayer", 1f, 1f);
    }

    private void FixedUpdate()
    {
        if (_alerted)
        {
            _agent.SetDestination(_playerController.transform.position);
        }
    }

    public void CheckForPlayer()
    {
        if (_alerted)
            return;

        Collider[] hitColliders = Physics.OverlapSphere(transform.position, _detectionRadius);
        foreach (var hitCollider in hitColliders)
        {
            if (hitCollider.CompareTag("Player"))
            {
                _alerted = true;
            }
            else if (hitCollider.CompareTag("Enemy") && hitCollider.gameObject != gameObject)
            {
                if (hitCollider.TryGetComponent<EnemyController>(out EnemyController enemy))
                {
                    _alerted |= enemy.GetAlertedStatus();
                }
            }
        }
    }

    public void TakeDamage(int i)
    {
        _currentHealth -= i;

        if (_currentHealth <= 0)
        {
            Kill();
        }
    }

    private void Kill()
    {
        Destroy(gameObject);
    }
}
