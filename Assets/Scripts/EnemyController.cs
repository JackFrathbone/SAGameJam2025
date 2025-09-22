using DG.Tweening;
using RenderHeads.Services;
using System.Collections.Generic;
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
    [SerializeField] float _rotationSpeed = 5f;

    [SerializeField] private bool _rangedAttack = false;
    [SerializeField] private Transform _projectileParent;
    [SerializeField] private GameObject _projectilePrefab;

    private int _currentHealth;

    private NavMeshAgent _agent;
    private PlayerController _playerController;
    private Material _mainMat;

    public bool GetAlertedStatus() => _alerted;
    private bool _alerted;
    private bool _attacking;

    private float _attackCooldown;

    [Header("Animations")]
    [SerializeField] List<AudioClip> _hurtSounds;
    [SerializeField] List<AudioClip> _gibSounds;
    [SerializeField] List<AudioClip> _alertSounds;

    [Header("Animations")]
    private Animator _animator;
    [SerializeField] private Transform _gibParent;
    [SerializeField] private GameObject _gibPrefab;

    private LazyService<GameManager> _gameManager;

    private void Start()
    {
        _currentHealth = _totalHealth;

        _agent = GetComponent<NavMeshAgent>();
        _playerController = FindAnyObjectByType<PlayerController>();
        _mainMat = GetComponentInChildren<SkinnedMeshRenderer>().material;

        _animator = GetComponentInChildren<Animator>();

        _agent.stoppingDistance = _attackDistance;

        InvokeRepeating("CheckForPlayer", 1f, 0.5f);
    }

    private void OnDisable()
    {
        DOTween.Kill(_mainMat);
    }

    private void Update()
    {
        RotateTowardsPlayer();
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

        if (_agent.velocity.magnitude > 0.25f)
        {
            _animator.SetBool("Running", true);
        }
        else
        {
            _animator.SetBool("Running", false);
        }
    }

    public void CheckForPlayer()
    {
        if (_alerted)
            return;

        Vector3 rayDirection = (_playerController.transform.position - _projectileParent.transform.position).normalized;
        if (Physics.Raycast(_projectileParent.transform.position, rayDirection, out var hit, _detectionRadius))
        {
            if (hit.collider.CompareTag("Player"))
            {
                _alerted = true;
                _gameManager.Value.PlayAudioClip(_alertSounds[Random.Range(0, _alertSounds.Count)], 0.85f);
                return;
            }
        }
    }

    void RotateTowardsPlayer()
    {
        Vector3 direction = _playerController.transform.position - _projectileParent.transform.position;

        if (direction != Vector3.zero)
        {
            Quaternion targetRotation = Quaternion.LookRotation(direction);

            transform.rotation = Quaternion.Slerp(_projectileParent.transform.rotation, targetRotation, Time.deltaTime * _rotationSpeed);
        }
    }

    public void TakeDamage(int i)
    {
        _animator.SetTrigger("Hurt");

        _gameManager.Value.PlayAudioClip(_hurtSounds[Random.Range(0, _hurtSounds.Count)]);

        _alerted = true;

        _currentHealth -= i;

        _attacking = true;
        _agent.enabled = false;

        if (_currentHealth <= 0)
        {
            _gameManager.Value.PlayAudioClip(_gibSounds[Random.Range(0, _gibSounds.Count)], 0.85f);
            Kill();

            return;
        }

        Invoke("EndTakeDamage", 0.85f);
    }

    private void EndTakeDamage()
    {
        _attacking = false;
        _agent.enabled = true;
    }

    private void DoAttack()
    {
        _attacking = true;
        _agent.enabled = false;

        if (!_rangedAttack)
            _animator.SetTrigger("Attack");

        //_mainMat.DOKill();
        _mainMat.DOColor(Color.red, _attackChargeSpeed).SetEase(Ease.Flash).SetId(this).OnComplete(EndAttack);
    }

    private void EndAttack()
    {
        _mainMat.DORewind();

        if (_rangedAttack)
            _animator.SetTrigger("Attack");

        _attacking = false;
        _agent.enabled = true;

        _attackCooldown = _attackCooldownTime;

        if (!_rangedAttack)
        {
            if (Vector3.Distance(transform.position, _playerController.transform.position) <= _agent.stoppingDistance)
            {
                _playerController.TakeDamage(_damage, this.transform);
            }
        }
        else
        {
            EnemyProjectile projectile = Instantiate(_projectilePrefab, _projectileParent.position, _projectileParent.rotation).GetComponent<EnemyProjectile>();
            projectile.SetDamage(_damage);
        }
    }

    private void Kill()
    {
        Destroy(Instantiate(_gibPrefab, _gibParent.position, _gibParent.rotation), 10f);

        Destroy(gameObject);
    }
}
