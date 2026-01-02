using UnityEngine;
using System.Collections;
using System.Collections.Generic;

public class PlayerAirState : PlayerState
{
    private bool isGrounded;
    private int xInput;
    private int yInput;
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
        
        if (player.CurrentVelocity.y <= 0)
        {
            player.RB.gravityScale = playerData.fallGravityScale;
        }
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
        yInput = player.InputHandler.NormInputY;
        jumpInput = player.InputHandler.JumpInput;
        GrabInput = player.InputHandler.GrabInput;

        if (player.IsOnLadder() && GrabInput && (yInput == 1 || yInput == -1))
        {
            stateMachine.ChangeState(player.LadderClimbState);
        }
        else if (isGrounded && player.CurrentVelocity.y < 0.01f)
        {
            stateMachine.ChangeState(player.LandState);
        }
        else if (isTouchingWall && !isTouchingLedge && GrabInput)
        {
            bool isValidLedge = player.Collision.IsValidLedge(0.2f);
            
            if (isValidLedge)
            {
                stateMachine.ChangeState(player.WallLedgeState);
            }
            else
            {
                stateMachine.ChangeState(player.WallGrapState);
            }
        }
        else if (isTouchingWall && GrabInput)
        {
            stateMachine.ChangeState(player.WallGrapState);
        }
        else if (isTouchingWall && xInput != 0 && xInput == player.FacingRight && !GrabInput)
        {
            if (player.CanSlideHere())
            {
                stateMachine.ChangeState(player.WallSlicedState);
            }
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
        ApplyGravityModifiers();
    }

    private void ApplyGravityModifiers()
    {
        if (player.CurrentVelocity.y < 0)
        {
            if (player.RB.gravityScale != playerData.fallGravityScale)
            {
                player.RB.gravityScale = playerData.fallGravityScale;
            }
            
            if (player.CurrentVelocity.y < -playerData.maxFallSpeed)
            {
                player.SetVelocityY(-playerData.maxFallSpeed);
            }
        }
        else if (player.CurrentVelocity.y > 0)
        {
            if (player.RB.gravityScale != playerData.jumpGravityScale)
            {
                player.RB.gravityScale = playerData.jumpGravityScale;
            }
        }
    }


}
