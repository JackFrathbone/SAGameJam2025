using DG.Tweening;
using UnityEngine;

public class BossOrb : MonoBehaviour
{
    [Header("Settings")]
    [SerializeField] float _rotationSpeed = 1f;
    [SerializeField] float _chargeWindup = 0.1f;
    [SerializeField] float _chargeSpeed = 8f;
    [SerializeField] float _returnSpeed = 5f;
    [SerializeField] LayerMask _chargeLayerMask;

    private PlayerController _playerController;

    [Header("References")]
    [SerializeField] GameObject _projectilePrefab;
    [SerializeField] Transform _projectileParent;

    [Header("Data")]
    private Vector3 _startPos;
    private Material _material;
    private bool _attacking;
    private bool _rangedPhase;

    private void Start()
    {
        _playerController = FindAnyObjectByType<PlayerController>();
        _material = GetComponent<MeshRenderer>().material;

        _startPos = transform.position;
    }

    private void OnDisable()
    {
        DOTween.Kill(transform);
        DOTween.Kill(_material);
    }

    private void Update()
    {
        if (_attacking)
            return;

        Quaternion targetRotation = Quaternion.LookRotation(_playerController.transform.position - transform.position);

        transform.rotation = Quaternion.Lerp(transform.rotation, targetRotation, Time.deltaTime * _rotationSpeed);

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

        SpawnProjectile();
        Invoke("SpawnProjectile", 0.05f);
        Invoke("SpawnProjectile", 0.1f);

        Invoke("EndShoot", 1.5f);
    }

    private void SpawnProjectile()
    {
        EnemyProjectile projectile = Instantiate(_projectilePrefab, _projectileParent.position, _projectileParent.rotation).GetComponent<EnemyProjectile>();
        projectile.SetDamage(10);
    }

    private void EndShoot()
    {
        _attacking = false;
    }
}
