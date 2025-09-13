using UnityEngine;
using System.Collections.Generic;
using System.Collections;
using UnityEngine.InputSystem;

public class PlayerInputHandler: MonoBehaviour
{
    public Vector2 RamMovementInput {  get; private set; }
    public int NormInputX {  get; private set; }
    public int NormInputY { get; private set; }
    public bool JumpInput { get; private set; }

    public void OnMoveInput(InputAction.CallbackContext context)
    {
        RamMovementInput = context.ReadValue<Vector2>();
        NormInputX = (int)(RamMovementInput * Vector2.right).normalized.x;
        NormInputY = (int)(RamMovementInput * Vector2.up).normalized.y;
        
    }

    public void OnJumpInput(InputAction.CallbackContext context)
    {

        if (context.started)
        {
            JumpInput = true;
        }

    }

    public void JumpEnded() => JumpInput = false;
    

}
