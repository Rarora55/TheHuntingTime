using UnityEngine;
using System.Collections;
using System.Collections.Generic;
public class PlayerFallState : PlayerState
{
    private bool isGrounded;
    private int xInput;
    private bool isTouchingWall;
    private bool jumpInput;
    private bool GrabInput;
    private bool isTouchingLedge;
    public PlayerFallState(Player player, PlayerStateMachine stateMachine, PlayerData playerData, string animBoolName) : base(player, stateMachine, playerData, animBoolName)
    {
    }

    public override void AnimationFinishTrigger()
    {
        base.AnimationFinishTrigger();
    }

    public override void AnimationTrigger()
    {
        base.AnimationTrigger();
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
    }

    public override void Exit()
    {
        base.Exit();
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
        }
    
    }

    public override void PhysicsUpdate()
    {
        base.PhysicsUpdate();
    }
}
