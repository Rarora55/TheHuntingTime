using UnityEngine;

public class PlayerWallSlicedState : PlayerTouchingWallState
{
    
    public PlayerWallSlicedState(Player player, PlayerStateMachine stateMachine, PlayerData playerData, string animBoolName) : base(player, stateMachine, playerData, animBoolName)
    {
    }

    public override void Enter()
    {
        base.Enter();
        player.SetVelocityX(0);
   
    }

   

    public override void LogicUpdate()
    {
         base.LogicUpdate();
        player.SetVelocityY(-playerData.WallSlicedVelocity);
    }

    public override void Exit()
    {
        base.Exit();
       /* if(hasFlipped)
            player.Flip();*/
    }
}
