using UnityEngine;
using UnityEngine.InputSystem.XR;
using UnityEngine.Windows;

public class PlayerController : MonoBehaviour
{
    [SerializeField] private CharacterController _controller;
    [SerializeField] private PlayerInputs _input;
    [SerializeField] private Transform _mainCamera;

    [Header("Player")]
    [SerializeField] private float _walkSpeed = 3f;
    [SerializeField] private float _sprintSpeed = 5f;
    [SerializeField] private float _rotationSmoothTime = 0.12f;
    [SerializeField] private float _speedChangeRate = 10.0f;

    [Header("Cinemachine")]
    [SerializeField] private GameObject _cinemachineCameraTarget;
    [SerializeField] private bool _LockCameraPosition = false;
    [SerializeField] private float _topClamp = 70f;
    [SerializeField] private float _bottomClmap = -30f;
    
    // player
    private float _speed = 3.0f;
    private float _moveAnimBlend;
    private float _targetRotation = 0.0f;
    private float _rotationVelocity;
    private float _verticalVelocity;

    // cinemachine
    private const float _lookDeltaThreshold = 0.01f;
    private float _cinemachineTargetYaw;
    private float _cinemachineTargetPitch;

    private void FixedUpdate()
    {
        //// Vector3 moveVec = _mainCamera.right * _input.MoveDir.x + _mainCamera.forward * _input.MoveDir.y;
        //// moveVec.y = 0.0f;
        //Vector3 moveVec = new Vector3(_input.MoveDir.x, 0.0f, _input.MoveDir.y);

        //_controller.Move(moveVec * _speed * Time.deltaTime);

        Move();
    }

    private void LateUpdate()
    {
        CameraRotation();
    }

    private void Move()
    {
        float targetSpeed = _input.Sprint ? _sprintSpeed : _walkSpeed;

        if (_input.MoveDirection == Vector2.zero)
        {
            targetSpeed = 0.0f;
        }

        float curHorizontalSpeed = new Vector3(_controller.velocity.x, 0.0f, _controller.velocity.z).magnitude;

        float speedOffset = 0.1f;
        if (curHorizontalSpeed < targetSpeed - speedOffset ||
            targetSpeed + speedOffset < curHorizontalSpeed)
        {
            float inputMagnitude = _input.AnalogMovment ? _input.MoveDirection.magnitude : 1f;
            _speed = Mathf.Lerp(curHorizontalSpeed, targetSpeed * inputMagnitude, _speedChangeRate * Time.deltaTime);
            _speed = Mathf.Round(_speed * 1000f) / 1000f;
        }
        else
        {
            _speed = targetSpeed;
        }

        Vector3 inputDirection = new Vector3(_input.MoveDirection.x, 0f, _input.MoveDirection.y);
        if (_input.MoveDirection != Vector2.zero)
        {
            _targetRotation = Mathf.Atan2(inputDirection.x, inputDirection.z) * Mathf.Rad2Deg;

            float rotation = Mathf.SmoothDampAngle(transform.eulerAngles.y, _targetRotation, ref _rotationVelocity, _rotationSmoothTime);
            transform.rotation = Quaternion.Euler(0f, rotation, 0f);
        }

        Vector3 targetDirection = Quaternion.Euler(0f,  _targetRotation, 0f) * Vector3.forward;

        _controller.Move(targetDirection.normalized * (_speed * Time.deltaTime));
    }

    private void CameraRotation()
    {
        if (_input.LookDirection.sqrMagnitude >= _lookDeltaThreshold && !_LockCameraPosition)
        {
            _cinemachineTargetYaw += _input.LookDirection.x;
            _cinemachineTargetPitch += _input.LookDirection.y;
        }

        _cinemachineTargetYaw = ClampAngle(_cinemachineTargetYaw, float.MinValue, float.MaxValue);
        _cinemachineTargetPitch = ClampAngle(_cinemachineTargetPitch, _bottomClmap, _topClamp);

        _cinemachineCameraTarget.transform.rotation = Quaternion.Euler(_cinemachineTargetYaw, _cinemachineTargetPitch, 0f);
    }

    private static float ClampAngle(float lfAngle, float lfMin, float lfMax)
    {
        if (lfAngle < -360f)
            lfAngle += 360f;

        if (lfAngle > 360f)
            lfAngle -= 360f;

        return Mathf.Clamp(lfAngle, lfMin, lfMax);
    }
}
