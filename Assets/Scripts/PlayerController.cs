using DG.Tweening;
using RenderHeads.Services;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

[RequireComponent(typeof(CharacterController))]
public class PlayerController : MonoBehaviour
{
    #region PlayerMovement
    [Header("Settings")]
    [SerializeField] private float _mouseSensitivity = 5f;
    [SerializeField] private float _walkingSpeed = 8f;
    [SerializeField] private float _jumpSpeed = 8f;
    [SerializeField] private float _gravity = 30f;
    [SerializeField] private float _lookXLimit = 90f;
    [SerializeField] private float _slopeSpeed = 5;

    [Header("States")]
    [SerializeField] private bool _cantJump;
    [SerializeField] private bool _cantMove;
    [SerializeField] private bool _isCrouching;

    [Header("Data")]
    private float _originalSpeed;

    [Header("References")]
    private CharacterController _characterController;
    private Camera _playerCamera;
    private Vector3 _moveDirection;
    private float _rotationX;

    [SerializeField] private float _dashTime = 1.5f;
    [SerializeField] private float _dashSpeed = 12f;
    private Vector3 _dashVector;
    private bool isDashing = false;

    //For slopes
    private Vector3 _hitNormal;
    #endregion

    private Vector3 _knockbackVector;

    [SerializeField] GameObject _projectilePrefab;

    [SerializeField] int _totalHealth = 100;
    [SerializeField] int _totalMana = 100;
    [SerializeField, Tooltip("in seconds")] float _regenSpeed = 1f;
    [SerializeField] float _damageInvulnerability = 1.5f;

    private int _currentHealth = 100;
    private int _currentMana = 0;

    private bool _updatingBars;

    private float _regenTimer;
    private float _chargeTime;

    private float _damageCooldown;

    [SerializeField] Image _healthBar;
    [SerializeField] Image _manaBar;

    [Header("Audio")]
    [SerializeField] private AudioClip _syphonClip;
    [SerializeField] private AudioClip _finishSyphon;
    [SerializeField] private List<AudioClip> _dashClips;

    [Header("Animation")]
    private Animator _animator;

    private LazyService<GameManager> _gameManager;

    private void Start()
    {
        _characterController = GetComponent<CharacterController>();
        _animator = GetComponentInChildren<Animator>();

        _playerCamera = Camera.main;

        _originalSpeed = _walkingSpeed;

        AllowMovement();
        UpdateBars();
    }

    private void Update()
    {
        ToggleCrouch();
        MovePlayer();
        CheckDash();
        //UpdateBars();
        CheckAttack();
        RegenHealth();

        _damageCooldown -= Time.deltaTime;
    }

    private void OnControllerColliderHit(ControllerColliderHit hit)
    {
        if (hit.gameObject.CompareTag("Spike"))
        {
            TakeDamage(35, hit.transform);
        }
    }

    private void OnDisable()
    {
        DOTween.Kill(_healthBar);
        DOTween.Kill(_manaBar);
    }

    public void StopMovement()
    {
        _cantJump = true;
        _cantMove = true;

        Cursor.lockState = CursorLockMode.Locked;
        Cursor.visible = false;
    }

    public void AllowMovement()
    {
        _cantJump = false;
        _cantMove = false;

        Cursor.lockState = CursorLockMode.Locked;
        Cursor.visible = false;
    }

    public void SetSlowMovement()
    {
        if (_walkingSpeed == _originalSpeed)
        {
            _walkingSpeed = 2f;
        }
    }

    //To return to normal  after being slowed
    public void SetNormalMovement()
    {
        _walkingSpeed = _originalSpeed;
    }

    private void ToggleCrouch()
    {
        //Press left ctrl to crouch
        if (Input.GetButtonDown("Crouch"))
        {
            _isCrouching = !_isCrouching;
        }
    }

    private void MovePlayer()
    {
        Vector3 forward = _characterController.transform.TransformDirection(Vector3.forward);
        Vector3 right = _characterController.transform.TransformDirection(Vector3.right);

        if (_isCrouching)
        {
            _characterController.height = 1f;
        }
        else
        {
            _characterController.height = 2f;
        }

        float curSpeedX = !_cantMove ? (_walkingSpeed) * Input.GetAxis("Vertical") : 0;
        float curSpeedY = !_cantMove ? (_walkingSpeed) * Input.GetAxis("Horizontal") : 0;
        float movementDirectionY = _moveDirection.y;

        if (_isCrouching)
        {
            curSpeedX /= 2;
            curSpeedY /= 2;
        }

        _moveDirection = (forward * curSpeedX) + (right * curSpeedY);

        if (Input.GetButton("Jump") && !_cantMove && _characterController.isGrounded && !_cantJump)
        {
            _moveDirection.y = _jumpSpeed;
        }
        else
        {
            _moveDirection.y = movementDirectionY;
        }

        if (!_characterController.isGrounded)
        {
            _moveDirection.y -= _gravity * Time.deltaTime;
        }

        //Checks slide
        if (!_cantMove && CheckSlide())
        {
            _moveDirection += new Vector3(_hitNormal.x, -_hitNormal.y, _hitNormal.z) * _slopeSpeed;
        }

        // Move the controller
        if (!isDashing)
        {
            _characterController.Move((_moveDirection + _knockbackVector) * Time.deltaTime);
        }
        else
        {
            _characterController.Move((_dashVector) * Time.deltaTime);
        }

        _knockbackVector = Vector3.Lerp(_knockbackVector, Vector3.zero, 8f * Time.deltaTime);

        // Player and Camera rotation
        if (!_cantMove)
        {
            _rotationX += -Input.GetAxis("Mouse Y") * _mouseSensitivity;
            _rotationX = Mathf.Clamp(_rotationX, -_lookXLimit, _lookXLimit);
            _playerCamera.transform.localRotation = Quaternion.Euler(_rotationX, 0, 0);
            _characterController.gameObject.transform.rotation *= Quaternion.Euler(0, Input.GetAxis("Mouse X") * _mouseSensitivity, 0);
        }
    }

    private bool CheckSlide()
    {
        if (_characterController.isGrounded && Physics.Raycast(_characterController.transform.position, Vector3.down, out RaycastHit slopeHit, 2f))
        {
            _hitNormal = slopeHit.normal;

            return (Vector3.Angle(_hitNormal, Vector3.up) > _characterController.slopeLimit);
        }
        else
        {
            return false;
        }
    }

    public void ApplyKnockback(Transform hitSource, float knockbackPower = 10f, float knockbackDuration = 0.5f)
    {
        Vector3 direction = (transform.position - hitSource.position).normalized;
        _knockbackVector = new Vector3(direction.x, 4f, direction.z) * knockbackPower;
    }

    private void RegenHealth()
    {
        if (_updatingBars)
            return;

        _regenTimer += Time.deltaTime;

        if (_regenTimer >= _regenSpeed && _currentMana > 0)
        {
            AddHealth(1);
            AddMana(-1);

            _regenTimer = 0f;
        }
    }

    private void UpdateBars()
    {
        _healthBar.fillAmount = GetPercentage(_currentHealth, _totalHealth) / 100;
        _manaBar.fillAmount = GetPercentage(_currentMana, _totalMana) / 100;
    }

    private float GetPercentage(int current, int total)
    {
        return total > 0 ? ((float)current / (float)total) * 100f : 0f;
    }

    private void AddHealth(int i)
    {
        _currentHealth = (int)Mathf.Clamp(_currentHealth += i, 0f, _totalHealth);

        _healthBar.DOKill();

        _updatingBars = true;

        _healthBar.DOFillAmount(GetPercentage(_currentHealth, _totalHealth) / 100, 1f)
        .SetEase(Ease.OutBounce)
        .SetSpeedBased(true)
        .OnComplete(() =>
        {
            _updatingBars = false;
        });
    }

    private void AddMana(int i)
    {
        if (i > 0)
        {
            _gameManager.Value.PlayAudioClip(_syphonClip, 0.25f, false, 0f, 0f, true);
        }

        _currentMana = (int)Mathf.Clamp(_currentMana += i, 0f, _totalMana);

        _manaBar.DOKill();

        _updatingBars = true;

        _manaBar.DOFillAmount(GetPercentage(_currentMana, _totalMana) / 100, 1f)
            .SetSpeedBased(true)
            .SetEase(Ease.OutBounce)
            .OnComplete(() =>
            {
                if (i > 0)
                {
                    _gameManager.Value.PlayAudioClip(_finishSyphon, 0.5f);
                }

                _updatingBars = false;
            })
            .OnKill(() =>
            {
                if (i > 0)
                {
                    _gameManager.Value.PlayAudioClip(_finishSyphon, 0.5f);
                }

                _updatingBars = false;
            });
    }

    public void TakeDamage(int i, Transform hitSource = null, bool ignoreCooldown = false)
    {
        if (_damageCooldown <= 0 || ignoreCooldown == true)
        {
            if (hitSource != null)
                ApplyKnockback(hitSource);

            AddHealth(-i);
            AddMana(i);

            _damageCooldown = _damageInvulnerability;
        }
    }

    private void CheckDash()
    {
        if (Input.GetButtonDown("Sprint") && !isDashing)
        {
            if (_currentMana >= 15f)
            {
                AddMana(-15);
                isDashing = true;

                Vector3 forward = transform.forward;

                forward.y = 0;

                _dashVector = forward.normalized * _dashSpeed;

                _gameManager.Value.PlayAudioClip(_dashClips[Random.Range(0, _dashClips.Count)]);

                Invoke("EndDash", _dashTime);
            }
        }
    }

    private void EndDash()
    {
        isDashing = false;
        _dashVector = Vector3.zero;
    }

    private void CheckAttack()
    {
        if (Input.GetButton("Fire1"))
        {
            _animator.SetBool("Charging", true);

            _chargeTime += Time.deltaTime;
        }
        else
        {

            if (_chargeTime > 0)
            {
                _animator.SetTrigger("Attack");
                Invoke("DoAttack", 0.15f);
            }
     

            _animator.SetBool("Charging", false);
            _chargeTime = 0;
        }
    }

    private void DoAttack()
    {
        int manaCost = (int)Mathf.Clamp((_chargeTime * 10), 5f, 25f);

        if (_currentMana >= manaCost)
        {
            _currentMana -= manaCost;

            Vector3 finalPosition = _playerCamera.transform.position + _playerCamera.transform.forward * 1f;
            PlayerProjectile projectile = Instantiate(_projectilePrefab, finalPosition, _playerCamera.transform.rotation).GetComponent<PlayerProjectile>();
            projectile.SetDamage(manaCost);

            Debug.Log($"Player projectile with {manaCost} damage");
        }

    }
}
