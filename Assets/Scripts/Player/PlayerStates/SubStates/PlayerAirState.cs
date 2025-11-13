using UnityEngine;
using System.Collections;
using System.Collections.Generic;

public class PlayerAirState : PlayerState
{
    private bool isGrounded;
    private int xInput;
    private bool isTouchingWall;
    private bool jumpInput;
    private bool GrabInput;
    private bool isTouchingLedge;
   
    public PlayerAirState(Player player, PlayerStateMachine stateMachine, PlayerData playerData, string animBoolName) : base(player, stateMachine, playerData, animBoolName)
    {
    }

    public override void DoChecks()
    {
        base.DoChecks();
        isGrounded = player.CheckIsGrounded();
        isTouchingWall = player.CheckIfTouchingWall();
        isTouchingLedge = player.CheckTouchingLedge();
    }

    public override void Enter()
    {
        base.Enter();
        player.anim.SetBool("inAir", true);
        player.anim.SetBool("isRunning", false);
        Debug.Log("<color=cyan>[AIR] Enter - En el aire</color>");
    }

    public override void Exit()
    {
        base.Exit();
        player.anim.SetBool("inAir", false);
        player.anim.SetBool("ledge", false);
    
    }

    public override void LogicUpdate()
    {
        base.LogicUpdate();

        xInput = player.InputHandler.NormInputX;
        jumpInput = player.InputHandler.JumpInput;
        GrabInput = player.InputHandler.GrabInput;

        Debug.Log($"<color=cyan>[AIR] Ground:{isGrounded} | Wall:{isTouchingWall} | Ledge:{isTouchingLedge} | xIn:{xInput} | FacingRight:{player.FacingRight} | Grab:{GrabInput} | Vel.y:{player.CurrentVelocity.y:F2}</color>");

        if (isGrounded && player.CurrentVelocity.y < 0.01f)
        {
            Debug.Log("[AIR] -> LandState (aterrizó)");
            stateMachine.ChangeState(player.LandState);
        }
        else if (isTouchingWall && !isTouchingLedge && GrabInput)
        {
            bool isValidLedge = player.Collision.IsValidLedge(0.2f);
            Debug.Log($"<color=orange>[AIR] Detectó esquina | ValidLedge: {isValidLedge}</color>");
            
            if (isValidLedge)
            {
                Debug.Log("<color=green>[AIR] -> WallLedgeState (esquina válida en el aire con Grab)</color>");
                stateMachine.ChangeState(player.WallLedgeState);
            }
            else
            {
                Debug.Log("<color=yellow>[AIR] -> WallGrapState (esquina inválida, agarrando pared)</color>");
                stateMachine.ChangeState(player.WallGrapState);
            }
        }
        else if (isTouchingWall && GrabInput)
        {
            Debug.Log("[AIR] -> WallGrapState (agarrando pared desde aire)");
            stateMachine.ChangeState(player.WallGrapState);
        }
        else if (isTouchingWall && xInput != 0 && xInput == player.FacingRight && !GrabInput)
        {
            Debug.Log($"<color=green>[AIR] -> WallSlicedState (presionando hacia pared, xInput:{xInput}, FacingRight:{player.FacingRight})</color>");
            stateMachine.ChangeState(player.WallSlicedState);
        }
        else
        {
            player.CheckFlip(xInput);
            player.SetVelocityX(playerData.movementVelocity * xInput);

            player.anim.SetFloat("yVelocity", player.CurrentVelocity.y);
            player.anim.SetFloat("xVelocity", Mathf.Abs(player.CurrentVelocity.x));
        }
    }

    public override void PhysicsUpdate()
    {
        base.PhysicsUpdate();
    }


}
