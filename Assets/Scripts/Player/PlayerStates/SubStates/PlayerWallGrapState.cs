using UnityEngine;

public class PlayerWallGrapState : PlayerTouchingWallState
{
    private Vector2 holdPosition;
    public PlayerWallGrapState(Player player, PlayerStateMachine stateMachine, PlayerData playerData, string animBoolName) : base(player, stateMachine, playerData, animBoolName)
    {
    }

    public override void Enter()
    {
        base.Enter();

        holdPosition = player.transform.position;
        HoldPosition();
    }

    public override void LogicUpdate()
    {
        base.LogicUpdate();
        HoldPosition();
       
        if (yInput > 0)
        {
            stateMachine.ChangeState(player.WallClimbState);
        }
        else if (yInput < 0 || !grabInput)
        {
            stateMachine.ChangeState(player.WallSlicedState);
        }
    }

    private void HoldPosition()
    {
        player.transform.position = holdPosition;

        player.SetVelocityX(0);
        player.SetVelocityY(0);

    }
}
