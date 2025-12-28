using UnityEngine;

public class PlayerCrouchIdleState : PlayerGroundState
{
    public PlayerCrouchIdleState(Player player, PlayerStateMachine stateMachine, PlayerData playerData, string animBoolName) : base(player, stateMachine, playerData, animBoolName)
    {
    }

    public override void Enter()
    {
        base.Enter();
        player.SetVelocityZero();
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
        
        if (player.JustFinishedLedgeClimb)
        {
            player.JustFinishedLedgeClimb = false;
        }
        
        if (xInput != 0)
            stateMachine.ChangeState(player.CrouchMoveState);
        else if (yInput != -1 && !isTouchingCeiling)
            stateMachine.ChangeState(player.IdleState);
    }
}
