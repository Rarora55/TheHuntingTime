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
        
        Debug.Log($"<color=magenta>[IDLE] ENTER | JustFinishedLedgeClimb: {player.JustFinishedLedgeClimb}</color>");
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
            Debug.Log("<color=yellow>[IDLE] Reseteando JustFinishedLedgeClimb flag AL INICIO</color>");
            player.JustFinishedLedgeClimb = false;
        }

        if (xInput != 0)
        {
            stateMachine.ChangeState(player.MoveState);
        }
        else if (yInput == -1 && !player.CheckIfTouchingWall())
        {
            Debug.Log("[IDLE] → CrouchIdleState (yInput detectado)");
            stateMachine.ChangeState(player.CrouchIdleState);
        }
    }

    public override void PhysicsUpdate()
    {
        base.PhysicsUpdate();
    }

   
}
