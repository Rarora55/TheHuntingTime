using UnityEngine;
using System.Collections.Generic;
using System.Collections;
using UnityEngine.InputSystem;

public class PlayerInputHandler: MonoBehaviour
{
    public Vector2 MovementInput {  get; private set; }

    public void OnMoveInput(InputAction.CallbackContext context)
    {
        MovementInput = context.ReadValue<Vector2>();
        Debug.Log($"OnMoveInput llamado! Vector: {MovementInput}, Context: {context.phase}");
    }

    public void OnJumpInput(InputAction.CallbackContext context)
    {
     
    }
}
