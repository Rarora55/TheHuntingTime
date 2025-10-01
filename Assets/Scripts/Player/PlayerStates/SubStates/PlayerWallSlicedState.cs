using UnityEngine;

public class PlayerWallSlicedState : PlayerTouchingWallState
{
    public PlayerWallSlicedState(Player player, PlayerStateMachine stateMachine, PlayerData playerData, string animBoolName) : base(player, stateMachine, playerData, animBoolName)
    {
    }

    public override void LogicUpdate()
    {
        base.LogicUpdate();
        player.SetVelocityY(playerData.WallSlicedVelocity);

        if(grabInput && yInput == 0)
        {
            stateMachine.ChangeState(player.WallGrapState);
        }
    }
}
