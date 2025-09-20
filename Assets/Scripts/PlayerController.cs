using UnityEngine;

[RequireComponent(typeof(CharacterController))]
public class PlayerController : MonoBehaviour
{
    [Header("Settings")]
    [SerializeField] private float _mouseSensitivity = 5f;
    [SerializeField] private float _walkingSpeed = 6f;
    [SerializeField] private float _runningSpeed = 10f;
    [SerializeField] private float _jumpSpeed = 8f;
    [SerializeField] private float _gravity = 30f;
    [SerializeField] private float _lookXLimit = 90f;
    [SerializeField] private float _slopeSpeed = 5;

    [Header("States")]
    [SerializeField] private bool _cantJump;
    [SerializeField] private bool _cantSprint;
    [SerializeField] private bool _cantMove;
    [SerializeField] private bool _isCrouching;

    [Header("Data")]
    private float _originalSpeed;

    [Header("References")]
    private CharacterController _characterController;
    private Camera _playerCamera;
    private Vector3 _moveDirection;
    private float _rotationX;

    private bool isRunning = false;

    //For slopes
    private Vector3 _hitNormal;


    private void Start()
    {
        _characterController = GetComponent<CharacterController>();

        _playerCamera = Camera.main;

        _originalSpeed = _walkingSpeed;
    }

    private void Update()
    {
        ToggleCrouch();
        MovePlayer();
    }

    public void StopMovement()
    {
        _cantJump = true;
        _cantMove = true;
    }

    public void AllowMovement()
    {
        _cantJump = false;
        _cantMove = false;
    }

    public void SetSlowMovement()
    {
        if (_walkingSpeed == _originalSpeed)
        {
            _cantSprint = true;
            _walkingSpeed = 2f;
        }
    }

    //To return to normal  after being slowed
    public void SetNormalMovement()
    {
        _cantSprint = false;
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

        // Press Left Shift to run
        isRunning = Input.GetButton("Sprint");

        if (_cantSprint)
        {
            isRunning = false;
        }


        if (_isCrouching)
        {
            isRunning = false;
            _characterController.height = 1f;
        }
        else
        {
            _characterController.height = 2f;
        }

        float curSpeedX = !_cantMove ? (isRunning ? _runningSpeed : _walkingSpeed) * Input.GetAxis("Vertical") : 0;
        float curSpeedY = !_cantMove ? (isRunning ? _runningSpeed : _walkingSpeed) * Input.GetAxis("Horizontal") : 0;
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
        _characterController.Move(_moveDirection * Time.deltaTime);

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
}
