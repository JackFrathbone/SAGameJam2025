using DG.Tweening;
using UnityEngine;
using UnityEngine.AI;

public class EnemyController : MonoBehaviour
{
    [Header("Settings")]
    [SerializeField] int _totalHealth = 100;
    [SerializeField] float _detectionRadius = 10f;
    [SerializeField] int _damage = 10;
    [SerializeField] float _attackChargeSpeed = 0.5f;
    [SerializeField] float _attackCooldownTime = 0.25f;
    [SerializeField] float _attackDistance = 3f;

    private int _currentHealth;

    private NavMeshAgent _agent;
    private PlayerController _playerController;
    private Material _mainMat;

    public bool GetAlertedStatus() => _alerted;
    private bool _alerted;
    private bool _attacking;

    private float _attackCooldown;

    private void Start()
    {
        _currentHealth = _totalHealth;

        _agent = GetComponent<NavMeshAgent>();
        _playerController = FindAnyObjectByType<PlayerController>();
        _mainMat = GetComponentInChildren<MeshRenderer>().material;

        _agent.stoppingDistance = _attackDistance;

        InvokeRepeating("CheckForPlayer", 1f, 1f);
    }

    private void OnDisable()
    {
        DOTween.Kill(_mainMat);
    }

    private void FixedUpdate()
    {
        if (_alerted && !_attacking)
        {
            _attackCooldown -= Time.deltaTime;

            _agent.SetDestination(_playerController.transform.position);

            if (Vector3.Distance(transform.position, _playerController.transform.position) <= _agent.stoppingDistance)
            {
                if (_attackCooldown <= 0)
                {
                    DoAttack();
                }
            }
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

    private void DoAttack()
    {
        _attacking = true;
        _agent.enabled = false;

        _mainMat.DOKill();
        _mainMat.DOColor(Color.red, _attackChargeSpeed).SetEase(Ease.Flash).SetId(this).OnComplete(EndAttack);
    }

    private void EndAttack()
    {
        _mainMat.DORewind();

        _attacking = false;
        _agent.enabled = true;

        _attackCooldown = _attackCooldownTime;

        if (Vector3.Distance(transform.position, _playerController.transform.position) <= _agent.stoppingDistance)
        {
            _playerController.TakeDamage(_damage, this.transform);
        }
    }

    private void Kill()
    {
        Destroy(gameObject);
    }
}
