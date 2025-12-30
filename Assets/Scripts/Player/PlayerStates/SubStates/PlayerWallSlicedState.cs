using UnityEngine;
using TheHunt.Environment;

public class PlayerWallSlicedState : PlayerTouchingWallState
{
    private ISlideable currentSlideable;
    private float slideSpeed;
    
    public PlayerWallSlicedState(Player player, PlayerStateMachine stateMachine, PlayerData playerData, string animBoolName) : base(player, stateMachine, playerData, animBoolName)
    {
    }

    public override void DoChecks()
    {
        base.DoChecks();
    }

    public override void Enter()
    {
        base.Enter();

        if (!player.CanSlideHere())
        {
            stateMachine.ChangeState(player.AirState);
            return;
        }

        currentSlideable = player.GetCurrentSlideable();
        
        if (!currentSlideable.CanSlide(player))
        {
            stateMachine.ChangeState(player.AirState);
            return;
        }

        slideSpeed = currentSlideable.GetSlideSpeed();
        player.SetVelocityX(0);
    }

   

    public override void LogicUpdate()
    {
        xInput = player.InputHandler.NormInputX;
        yInput = player.InputHandler.NormInputY;
        grabInput = player.InputHandler.GrabInput;

        if (!player.CanSlideHere())
        {
            stateMachine.ChangeState(player.AirState);
            return;
        }

        currentSlideable = player.GetCurrentSlideable();
        slideSpeed = currentSlideable.GetSlideSpeed();
        
        if (!isTouchingWall)
        {
            stateMachine.ChangeState(player.AirState);
        }
        else if (xInput == 0 || xInput != player.FacingRight)
        {
            stateMachine.ChangeState(player.AirState);
        }
        else if (grabInput)
        {
            stateMachine.ChangeState(player.WallGrapState);
        }
        else
        {
            player.SetVelocityY(-slideSpeed);
            player.SetVelocityX(0);
        }
    }

    public override void Exit()
    {
        base.Exit();
       /* if(hasFlipped)
            player.Flip();*/
    }
}
