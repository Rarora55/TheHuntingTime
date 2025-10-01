using UnityEngine;
using UnityEngine.InputSystem;

public class PlayerInputHandler : MonoBehaviour
{
    public Vector2 RamMovementInput { get; private set; }
    public int NormInputX { get; private set; }
    public int NormInputY { get; private set; }
    public bool JumpInput { get; private set; }

    [SerializeField] private float inputHoldTime = 0.2f;
    [SerializeField] private float jumpInputStartTime;

    public bool GrabInput { get; private set; }

    private void Update()
    {
        CheckJumpInputHoldTime();
    }

    public void OnMoveInput(InputAction.CallbackContext context)
    {
        RamMovementInput = context.ReadValue<Vector2>();
        if(Mathf.Abs(RamMovementInput.x) > 0.5f)
        {
        NormInputX = (int)(RamMovementInput * Vector2.right).normalized.x;
        }else
        {
            NormInputX = 0;
        }
        if (Mathf.Abs(RamMovementInput.y) > 0.5f)
        {
            NormInputY = (int)(RamMovementInput * Vector2.up).normalized.y;
        }
        else
        {
            NormInputY= 0;
        }

    }

    public void OnJumpInput(InputAction.CallbackContext context)
    {

        if (context.started)
        {
            JumpInput = true;
            jumpInputStartTime = Time.time;
        }

    }

    public void OnGrabInput(InputAction.CallbackContext context)
    {
        if (context.started)
            GrabInput = true;

        if(context.canceled)
            GrabInput = false;

    }

    public void JumpEnded() => JumpInput = false;

    private void CheckJumpInputHoldTime()
    {
        if (Time.time >= jumpInputStartTime + inputHoldTime)
        {
            JumpInput = false;
        }
    }


}
