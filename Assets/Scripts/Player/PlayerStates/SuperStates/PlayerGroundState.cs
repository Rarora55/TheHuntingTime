using UnityEngine;
using System.Collections;
using System.Collections.Generic;

public class PlayerGroundState : PlayerState
{
    protected Vector2 input;

    public PlayerGroundState(Player player, PlayerStateMachine stateMachine, PlayerData playerData, string animBoolName) : base(player, stateMachine, playerData, animBoolName)
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

        input = player.InputHandler.MovementInput;
    }

    public override void PhysicsUpdate()
    {
        base.PhysicsUpdate();
    }
  
}
