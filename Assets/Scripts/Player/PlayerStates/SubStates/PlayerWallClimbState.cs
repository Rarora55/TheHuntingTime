using UnityEngine;

public class PlayerWallClimbState : PlayerTouchingWallState
{
    public PlayerWallClimbState(Player player, PlayerStateMachine stateMachine, PlayerData playerData, string animBoolName) : base(player, stateMachine, playerData, animBoolName)
    {
    }

    public override void DoChecks()
    {
        base.DoChecks();


    }

    public override void LogicUpdate()
    {
        base.LogicUpdate();
        
        if (!grabInput)
        {
            stateMachine.ChangeState(player.AirState);
            return;
        }

        player.SetVelocityY(playerData.WallClimbVelocity);

        if (yInput != 1)
        {
            stateMachine.ChangeState(player.WallGrapState);
        }
    }
}
