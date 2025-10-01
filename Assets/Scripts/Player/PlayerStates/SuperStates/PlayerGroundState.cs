using UnityEngine;
using System.Collections;
using System.Collections.Generic;

public class PlayerGroundState : PlayerState
{
    protected int xInput;

    private bool JumpInput;
    private bool isTouchingWall;
    private bool GrabInput;

    public PlayerGroundState(Player player, PlayerStateMachine stateMachine, PlayerData playerData, string animBoolName) : base(player, stateMachine, playerData, animBoolName)
    {

    }

    public override void DoChecks()
    {
        base.DoChecks();

        isTouchingWall = player.CheckIfTouchingWall();
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
        JumpInput = player.InputHandler.JumpInput;
        GrabInput = player.InputHandler.GrabInput;

        if (JumpInput)
        {
            player.InputHandler.JumpEnded();
            stateMachine.ChangeState(player.JumpState);
        } else if (isTouchingWall && GrabInput)
        {
            stateMachine.ChangeState(player.WallGrapState);
        }
    }

    public override void PhysicsUpdate()
    {
        base.PhysicsUpdate();
    }
  
}
