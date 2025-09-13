using UnityEngine;
using System.Collections;
using System.Collections.Generic;

public class PlayerMoveState : PlayerGroundState
{
    public PlayerMoveState(Player player, PlayerStateMachine stateMachine, PlayerData playerData, string animBoolName) : base(player, stateMachine, playerData, animBoolName)
    {
    }

    public override void DoChecks()
    {
        base.DoChecks();
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

        player.SetVelocityX(playerData.movementVelocity * input.x);

        if(input.x == 0)
        {
            stateMachine.ChangeState(player.IdleState); 
        }
    }

    public override void PhysicsUpdate()
    {
        base.PhysicsUpdate();
    }
}
