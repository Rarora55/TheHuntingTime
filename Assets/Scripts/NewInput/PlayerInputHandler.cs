using UnityEngine;
using System.Collections.Generic;
using System.Collections;
using UnityEngine.InputSystem;

public class PlayerInputHandler: MonoBehaviour
{
    public Vector2 MoveInput {  get; private set; } 

    public void OnMoveInput(InputAction.CallbackContext context)
    {
        MoveInput = context.ReadValue<Vector2>();
        
    }

    public void OnJumpInput(InputAction.CallbackContext context)
    {
       
    }



}
