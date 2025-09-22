using DG.Tweening;
using UnityEngine;

public class BossOrb : MonoBehaviour
{
    [Header("Settings")]
    [SerializeField] int _bossHealth = 100;
    [SerializeField] float _rotationSpeed = 1f;
    [SerializeField] float _chargeWindup = 0.1f;
    [SerializeField] float _chargeSpeed = 8f;
    [SerializeField] float _returnSpeed = 5f;
    [SerializeField] LayerMask _chargeLayerMask;

    private PlayerController _playerController;

    [Header("References")]
    [SerializeField] GameObject _projectilePrefab;
    [SerializeField] Transform _projectileParent;

    [SerializeField] GameObject _eye;
    private Rigidbody _rigidbody;

    [Header("Data")]
    private int _currentHealth;

    private Vector3 _startPos;
    private Material _material;
    private bool _attacking;
    private bool _shooting;

    private void Start()
    {
        _playerController = FindAnyObjectByType<PlayerController>();
        _material = GetComponent<MeshRenderer>().material;
        _rigidbody = GetComponent<Rigidbody>();

        _startPos = transform.position;

        _currentHealth = _bossHealth;
    }

    private void OnDisable()
    {
        DOTween.Kill(transform);
        DOTween.Kill(_material);
    }

    private void Update()
    {

        if (!_attacking || _shooting)
        {
            Quaternion targetRotation = Quaternion.LookRotation(_playerController.transform.position - transform.position);

            transform.rotation = Quaternion.Lerp(transform.rotation, targetRotation, Time.deltaTime * _rotationSpeed);
        }

        if (_attacking)
            return;

        if (Physics.Raycast(transform.position, transform.forward, out RaycastHit hit, 50f))
        {
            if (hit.collider.CompareTag("Player"))
            {
                int chance = Random.Range(0, 2);

                if (chance == 0)
                {
                    Charge(hit.point);
                }
                else
                {
                    Shoot();
                }
            }
        }
    }

    private void Charge(Vector3 target)
    {
        _attacking = true;

        _material.DOColor(Color.red, _chargeWindup).SetEase(Ease.Flash)
        .OnComplete(() =>
        {
            _material.DORewind();

            transform.DOMove(target, _chargeSpeed)
            .SetSpeedBased(true)
            .SetEase(Ease.OutBounce)
            .OnComplete(() =>
            {
                transform.DOMove(_startPos, _returnSpeed)
                .SetSpeedBased(true)
                .OnComplete(() =>
                {
                    _attacking = false;
                });
            });
        });
    }

    private void Shoot()
    {
        _attacking = true;
        _shooting = true;

        float shootTime = Random.Range(1f, 5f);

        InvokeRepeating("SpawnProjectile", 0f, 0.25f);

        Invoke("EndShoot", shootTime);
    }

    private void SpawnProjectile()
    {
        EnemyProjectile projectile = Instantiate(_projectilePrefab, _projectileParent.position, _projectileParent.rotation).GetComponent<EnemyProjectile>();
        projectile.SetDamage(10);
    }

    private void EndShoot()
    {
        CancelInvoke();

        _shooting = false;
        _attacking = false;
    }

    public void TakeDamage(int i)
    {
        _currentHealth -= i;

        if(_currentHealth <= 0)
        {
            gameObject.tag = "Untagged";
            _eye.SetActive(false);
            _rigidbody.isKinematic = false;
            _rigidbody.useGravity = true;
            Destroy(this);
        }
    }
}
