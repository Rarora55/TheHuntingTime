using UnityEngine;
using System.Collections.Generic;
using System.Collections;

public class PlayerTouchingWallState : PlayerState
{

    protected bool isGrounded;
    protected bool isTouchingWall;
    protected bool grabInput;
    protected bool isTouchingLedge;
    protected int xInput;
    protected int yInput;
    
    public PlayerTouchingWallState(Player player, PlayerStateMachine stateMachine, PlayerData playerData, string animBoolName) : base(player, stateMachine, playerData, animBoolName)
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

       /* if(isTouchingWall && !isTouchingLedge)
        {
            player.WallLedgeState.SetDetectedPosition(player.transform.position);
        }
       */
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
        yInput = player.InputHandler.NormInputY;
        grabInput = player.InputHandler.GrabInput;

        if (isGrounded && !grabInput)
        {
            stateMachine.ChangeState(player.IdleState);
        }
        else if(!isTouchingWall || (xInput != player.FacingRight && !grabInput))
        {
         
            stateMachine.ChangeState(player.AirState);
        }
       /* else if (xInput == -player.FacingRight && !grabInput)
        {
            stateMachine.ChangeState(player.AirState);
        }
        else if (isTouchingWall && !isTouchingLedge && grabInput)
        {
            stateMachine.ChangeState(player.WallLedgeState);
        }*/
    }

    public override void PhysicsUpdate()
    {
        base.PhysicsUpdate();
    }

    public override string ToString()
    {
        return base.ToString();
    }
}
