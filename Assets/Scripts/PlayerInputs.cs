using UnityEngine;
#if ENABLE_INPUT_SYSTEM
using UnityEngine.InputSystem;
using static UnityEngine.Rendering.DebugUI;
#endif

public class PlayerInputs : MonoBehaviour
{
    public Vector2 MoveDirection { get => _moveDir; }
    public Vector2 LookDirection { get => _lookDir; }
    public bool Jump { get => _jump; }
    public bool Sprint { get => _sprint; }

    public bool AnalogMovment { get => _analogMovement; }

    public bool CursorLocked { get => _cursorLocked; }
    public bool CursorInputForLook { get => _cursorInputForLook; }

    [Header("Character Input Value")]
    [SerializeField] private Vector2 _moveDir;
    [SerializeField] private Vector2 _lookDir;
    [SerializeField] private bool _jump;
    [SerializeField] private bool _sprint;

    [Header("Movement Setting")]
    [SerializeField] private bool _analogMovement;

    [Header("Mouse Cursor Setting")]
    [SerializeField] private bool _cursorLocked = true;
    [SerializeField] private bool _cursorInputForLook = true;

#if ENABLE_INPUT_SYSTEM
    public void OnMove(InputValue value)
    {
        MoveInput(value.Get<Vector2>());
    }

    public void OnLook(InputValue value)
    {
        if (_cursorInputForLook)
        {
            LookInput(value.Get<Vector2>());
        }
    }

    public void OnJump(InputValue value)
    {
        JumpInput(value.isPressed);
    }

    public void OnSprint(InputValue value)
    {
        SprintInput(value.isPressed);
    }
#endif

    public void MoveInput(Vector2 newMoveDirection)
    {
        _moveDir = newMoveDirection;
    }

    public void LookInput(Vector2 newLookDirection)
    {
        _lookDir = newLookDirection;
    }

    public void JumpInput(bool newJumpState)
    {
        _jump = newJumpState;
    }

    public void SprintInput(bool newSprintState)
    {
        _sprint = newSprintState;
    }

    private void OnApplicationFocus(bool hasFocus)
    {
        SetCursorState(_cursorLocked);
    }

    private void SetCursorState(bool newCursorState)
    {
        Cursor.lockState = newCursorState ? CursorLockMode.Locked : CursorLockMode.None;
    }
}
