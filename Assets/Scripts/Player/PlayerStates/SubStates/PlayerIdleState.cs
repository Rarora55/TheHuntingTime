using UnityEngine;
using System.Collections;
using System.Collections.Generic;

public class PlayerIdleState : PlayerGroundState
{
   

    public PlayerIdleState(Player player, PlayerStateMachine stateMachine, PlayerData playerData, string animBoolName) : base(player, stateMachine, playerData, animBoolName)
    {
    }

    public override void DoChecks()
    {
        base.DoChecks();
        
    }
    public override void Enter()
    {
        base.Enter();
        player.SetVelocityX(0);
        player.anim.SetBool("isRunning", false);
    }

    public override void Exit()
    {
        base.Exit();
    }

    public override void LogicUpdate()
    {
        base.LogicUpdate();
        
        if (player.JustFinishedLedgeClimb)
        {
            player.JustFinishedLedgeClimb = false;
        }

        if (xInput != 0)
        {
            stateMachine.ChangeState(player.MoveState);
        }
        else if (yInput == -1 && !player.CheckIfTouchingWall())
        {
            stateMachine.ChangeState(player.CrouchIdleState);
        }
    }

    public override void PhysicsUpdate()
    {
        base.PhysicsUpdate();
    }

   
}
