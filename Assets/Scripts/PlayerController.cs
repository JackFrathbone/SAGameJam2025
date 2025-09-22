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
    [SerializeField, Tooltip("How much health for each mana")] int _regenRatio = 2;
    [SerializeField] float _regenSpeed = 1.5f;
    [SerializeField] float _damageInvulnerability = 1.5f;

    private int _currentHealth = 100;
    private int _currentMana = 0;

    private bool _updatingHealthBar;
    private bool _updatingManaBar;
    private bool _regeningHealth;

    private float _chargeTime;
    private int _chargeDamage;
    private float _regenTimer;

    private float _damageCooldown;

    [SerializeField] Image _healthBar;
    [SerializeField] Image _manaBar;

    [Header("Audio")]
    [SerializeField] private AudioSource _audioSourceHealthBar;
    [SerializeField] private AudioSource _audioSourceManaBar;

    [SerializeField] private AudioClip _finishSyphon;
    [SerializeField] private List<AudioClip> _dashClips;
    [SerializeField] private List<AudioClip> _hurtClips;

    [SerializeField] private AudioClip _youDiedClip;
    [SerializeField] private AudioClip _giggleClip;

    [Header("Animation")]
    private Animator _animator;
    [SerializeField] GameObject _bloodBall;


    [Header("Death")]
    [SerializeField] GameObject _deathMenu;
    [SerializeField] Image _deathFade;

    public void SetLastCrystal(Transform t) => _lastUsedCrystal = t;
    private Transform _lastUsedCrystal;

    private bool _paused;
    [SerializeField] GameObject _pauseCanvas;

    private LazyService<GameManager> _gameManager;

    private void Start()
    {
        _characterController = GetComponent<CharacterController>();
        _animator = GetComponentInChildren<Animator>();

        _deathMenu.SetActive(false);

        _pauseCanvas.SetActive(false);

        _playerCamera = Camera.main;

        _originalSpeed = _walkingSpeed;

        _bloodBall.SetActive(false);

        AllowMovement();
        UpdateBars();
    }

    private void Update()
    {
        RefreshMouseSensitivity();

        ToggleCrouch();
        MovePlayer();
        CheckDash();
        CheckAttack();
        CheckRegenHealth();
        CheckBarAudio();

        if (Input.GetButtonDown("Cancel"))
        {
            _paused = !_paused;

            if (_paused)
            {
                StopMovement();
                _pauseCanvas.SetActive(true);
                _gameManager.Value.PauseGame();
            }
            else
            {
                AllowMovement();
                _pauseCanvas.SetActive(false);
                _gameManager.Value.UnPauseGame();
            }
        }

        _damageCooldown -= Time.deltaTime;
    }

    private void OnControllerColliderHit(ControllerColliderHit hit)
    {
        if (hit.gameObject.CompareTag("Spike"))
        {
            TakeDamage(35, hit.transform);
        }
        else if (hit.gameObject.CompareTag("Boss"))
        {
            TakeDamage(50, hit.transform);
        }
    }

    private void OnDisable()
    {
        DOTween.Kill(_healthBar);
        DOTween.Kill(_manaBar);

        _gameManager.Value.UnPauseGame();
    }

    public void StopMovement()
    {
        _cantJump = true;
        _cantMove = true;

        Cursor.lockState = CursorLockMode.None;
        Cursor.visible = true;
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

    private void CheckRegenHealth()
    {
        if (_updatingManaBar)
            return;

        if (Input.GetButton("Fire2"))
        {
            _regenTimer += _regenSpeed * Time.deltaTime;

            if (_regenTimer >= 1f && _currentMana > 1f && _currentHealth < _totalHealth)
            {
                _regeningHealth = true;

                _regenTimer = 0f;

                AddMana(-1);
                AddHealth(_regenRatio);
            }
        }
        else
        {
            _regeningHealth = false;
        }
    }

    private void CheckBarAudio()
    {
        if (_updatingHealthBar && !_audioSourceHealthBar.isPlaying)
        {
            _audioSourceHealthBar.Play();
        }
        else if (!_updatingHealthBar && !_regeningHealth)
        {
            _audioSourceHealthBar.Stop();
        }

        if (_updatingManaBar && !_audioSourceManaBar.isPlaying)
        {
            _audioSourceManaBar.Play();
        }
        else if (!_updatingManaBar)
        {
            _audioSourceManaBar.Stop();
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

        _updatingHealthBar = true;

        _healthBar.DOFillAmount(GetPercentage(_currentHealth, _totalHealth) / 100, 1f)
        .SetEase(Ease.OutBounce)
        .SetSpeedBased(true)
        .OnComplete(() =>
        {
            _updatingHealthBar = false;
        })
        .OnKill(() =>
        {
            UpdateBars();
        });
    }

    private void AddMana(int i)
    {
        _currentMana = (int)Mathf.Clamp(_currentMana += i, 0f, _totalMana);

        _manaBar.DOKill();

        _updatingManaBar = true;

        _manaBar.DOFillAmount(GetPercentage(_currentMana, _totalMana) / 100, 1f)
            .SetSpeedBased(true)
            .SetEase(Ease.OutBounce)
            .OnComplete(() =>
            {
                if (i > 0)
                {
                    _gameManager.Value.PlayAudioClip(_finishSyphon, 0.2f);
                }

                _updatingManaBar = false;
            })
            .OnKill(() =>
            {
                if (i > 0)
                {
                    _gameManager.Value.PlayAudioClip(_finishSyphon, 0.5f);
                }

                UpdateBars();
            });
    }

    public void TakeDamage(int i, Transform hitSource = null, bool ignoreCooldown = false)
    {
        if (_cantMove)
            return;

        if (_damageCooldown <= 0 || ignoreCooldown == true)
        {
            if (hitSource != null)
                ApplyKnockback(hitSource);

            _gameManager.Value.PlayAudioClip(_hurtClips[Random.Range(0, _hurtClips.Count)], 0.85f);

            AddHealth(-i);
            AddMana(i);

            _damageCooldown = _damageInvulnerability;

            if (_currentHealth <= 0)
            {
                PlayerDeath();
            }
        }
    }

    private void CheckDash()
    {
        if (Input.GetButtonDown("Sprint") && !isDashing && _chargeTime <= 0)
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
        if (Input.GetButton("Fire1") && _currentMana >= 5)
        {
            _animator.SetBool("Charging", true);

            _bloodBall.SetActive(true);

            _chargeTime += Time.deltaTime;

            _chargeDamage = 5;

            if (_chargeTime >= 1f && _chargeTime < 3f && _currentMana > 10)
            {
                _chargeDamage = 10;

                Vector3 mediumScale = new Vector3(0.13f, 0.13f, 0.13f);
                _bloodBall.transform.DOScale(mediumScale, 0.5f).SetEase(Ease.OutBounce);
            }
            else if (_chargeTime >= 3f && _currentMana > 15)
            {
                _chargeDamage = 15;

                Vector3 largeScale = new Vector3(0.2f, 0.2f, 0.2f);
                _bloodBall.transform.DOScale(largeScale, 0.5f).SetEase(Ease.OutBounce);
            }
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
        int manaCost = _chargeDamage;

        _chargeDamage = 0;

        if (_currentMana >= manaCost)
        {
            AddMana(-manaCost);

            _bloodBall.transform.DOKill();
            _bloodBall.transform.localScale = new Vector3(0.08f, 0.08f, 0.08f);
            _bloodBall.SetActive(false);

            Vector3 finalPosition = _playerCamera.transform.position + _playerCamera.transform.forward * 1f;
            PlayerProjectile projectile = Instantiate(_projectilePrefab, finalPosition, _playerCamera.transform.rotation).GetComponent<PlayerProjectile>();
            projectile.SetDamage(manaCost);
            projectile.SetPlayer(this);
        }

    }

    private void PlayerDeath()
    {
        StopMovement();

        _deathFade.DOKill(true);
        _deathFade.DOFade(1f, 2f)
        .OnComplete(() =>
        {
            _gameManager.Value.PlayAudioClip(_youDiedClip, 0.9f);
            _gameManager.Value.PauseGame();
            _deathMenu.SetActive(true);
        });
    }

    public void LoadCheckpoint()
    {
        _deathFade.DOKill(true);
        _deathFade.DOFade(0f, 0.1f);

        _deathMenu.SetActive(false);

        AddHealth(50);
        AddMana(25);

        _gameManager.Value.UnPauseGame();
        AllowMovement();

        _gameManager.Value.PlayAudioClip(_giggleClip, 0.9f);

        if (_lastUsedCrystal != null)
        {
            _characterController.enabled = false;
            transform.position = _lastUsedCrystal.position;
            _characterController.enabled = true;
        }
        else
        {
            Restart();
        }
    }

    public void Restart()
    {
        _gameManager.Value.UnPauseGame();
        _gameManager.Value.RestartLevel();
    }

    public void MainMenu()
    {
        _gameManager.Value.LoadLevel(0);
    }

    public void RefreshMouseSensitivity()
    {
        _mouseSensitivity = PlayerPrefs.GetFloat("MouseSensitivity", 4);
    }
}
