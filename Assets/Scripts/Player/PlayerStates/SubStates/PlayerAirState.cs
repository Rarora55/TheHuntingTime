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

        if(isTouchingWall && !isTouchingLedge)
        {
            player.WallLedgeState.SetDetectedPosition(player.transform.position);
        }

   
    }

    public override void Enter()
    {
        base.Enter();
    }

    public override void Exit()
    {
        base.Exit();
        player.anim.SetBool("ledge", false);
    }

    public override void LogicUpdate()
    {
        base.LogicUpdate();

        xInput = player.InputHandler.NormInputX;
        jumpInput = player.InputHandler.JumpInput;
        GrabInput = player.InputHandler.GrabInput;

        if (isGrounded && player.CurrentVelocity.y < 0.01f)
        {
            stateMachine.ChangeState(player.LandState);

        }
        else if (isTouchingWall && !isTouchingLedge)
        {

            stateMachine.ChangeState(player.WallLedgeState);

        }
        else if (isTouchingWall && GrabInput)
        {
            stateMachine.ChangeState(player.WallGrapState);
        }

        else if (isTouchingWall && xInput == player.FacingRight && player.CurrentVelocity.y <= 0)
        {
            stateMachine.ChangeState(player.WallSlicedState);
        }
        else
        {
            player.CheckFlip(xInput);
            player.SetVelocityX(playerData.movementVelocity * xInput);

            player.anim.SetFloat("yVelocity", player.CurrentVelocity.y);
            player.anim.SetFloat("xVelocity", Mathf.Abs(player.CurrentVelocity.x));

            bool isRising = player.CurrentVelocity.y > 1f;
            bool isFalling = player.CurrentVelocity.y < -3f;

            player.anim.SetBool("isRising", isRising);
            player.anim.SetBool("isFalling", isFalling);
        }
    }

    public override void PhysicsUpdate()
    {
        base.PhysicsUpdate();
    }


}
