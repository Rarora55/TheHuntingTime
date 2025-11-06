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
    }

    public override void Enter()
    {
        base.Enter();
        player.anim.SetBool("isRunning", false);
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

        Debug.Log($"[WALL] Wall:{isTouchingWall} | Ledge:{isTouchingLedge} | Ground:{isGrounded} | xIn:{xInput} | Grab:{grabInput} | Vel.y:{player.CurrentVelocity.y:F2}");

        if (!isTouchingWall)
        {
            Debug.Log("[WALL] -> AirState (no toca pared)");
            stateMachine.ChangeState(player.AirState);
        }
        else if (xInput != player.FacingRight && xInput != 0 && !grabInput)
        {
            Debug.Log("[WALL] -> AirState (empujando afuera)");
            stateMachine.ChangeState(player.AirState);
        }
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
