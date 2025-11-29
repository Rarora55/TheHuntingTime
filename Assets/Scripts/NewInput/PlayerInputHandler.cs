using UnityEngine;
using UnityEngine.InputSystem;
using TheHunt.Interaction;

public class PlayerInputHandler : MonoBehaviour
{
    public Vector2 RamMovementInput { get; private set; }
    public int NormInputX { get; private set; }
    public int NormInputY { get; private set; }
    public bool JumpInput { get; private set; }
    public bool RunInput { get; private set; }
    public bool GrabInput { get; private set; }

    [SerializeField] private float inputHoldTime = 0.2f;
    [SerializeField] private float jumpInputStartTime;

    private PlayerInteractionController interactionController;

    private void Awake()
    {
        interactionController = GetComponent<PlayerInteractionController>();
    }

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

    public void OnRunInput(InputAction.CallbackContext context)
    {
        if (context.started)
            RunInput = true;

        if (context.canceled)
            RunInput = false;
    }

    public void OnInteractInput(InputAction.CallbackContext context)
    {
        if (context.performed && interactionController != null)
        {
            interactionController.TryInteract();
        }
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
