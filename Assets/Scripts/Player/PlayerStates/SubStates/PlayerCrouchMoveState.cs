using UnityEngine;
using System.Collections;
using System.Collections.Generic;

public class PlayerCrouchMoveState : PlayerGroundState
{
    public PlayerCrouchMoveState(Player player, PlayerStateMachine stateMachine, PlayerData playerData, string animBoolName) : base(player, stateMachine, playerData, animBoolName)
    {
    }

    public override void Enter()
    {
        base.Enter();
        player.SetColliderHeight(playerData.crouchColliderHeight);
    }

    public override void Exit()
    {
        base.Exit();
        player.SetColliderHeight(playerData.standColliderHeight);
    }

    public override void LogicUpdate()
    {
        base.LogicUpdate();

        player.SetVelocityX(playerData.crouchMovementVelocity * player.FacingRight);
        player.CheckFlip(xInput);

        if (xInput == 0)
            stateMachine.ChangeState(player.CrouchIdleState);
        else if (yInput != -1 && !isTouchingCeiling)
            stateMachine.ChangeState(player.MoveState);
    }
}
