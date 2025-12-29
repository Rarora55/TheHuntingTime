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
            Debug.LogWarning("<color=red>[WALLSLIDE] No hay superficie deslizable, cancelando slide</color>");
            stateMachine.ChangeState(player.AirState);
            return;
        }

        currentSlideable = player.GetCurrentSlideable();
        
        if (!currentSlideable.CanSlide(player))
        {
            Debug.LogWarning("<color=red>[WALLSLIDE] La superficie no permite deslizamiento, cancelando</color>");
            stateMachine.ChangeState(player.AirState);
            return;
        }

        slideSpeed = currentSlideable.GetSlideSpeed();
        player.SetVelocityX(0);
        
        Debug.Log($"<color=magenta>[WALLSLIDE] Enter - Deslizando a velocidad {slideSpeed} (ángulo: {currentSlideable.GetSurfaceAngle()}°)</color>");
    }

   

    public override void LogicUpdate()
    {
        xInput = player.InputHandler.NormInputX;
        yInput = player.InputHandler.NormInputY;
        grabInput = player.InputHandler.GrabInput;

        if (!player.CanSlideHere())
        {
            Debug.LogWarning("<color=red>[WALLSLIDE] -> AirState (perdió contacto con superficie deslizable)</color>");
            stateMachine.ChangeState(player.AirState);
            return;
        }

        currentSlideable = player.GetCurrentSlideable();
        slideSpeed = currentSlideable.GetSlideSpeed();
        
        Debug.Log($"[WALLSLIDE] Ground:{isGrounded} | Wall:{isTouchingWall} | Ledge:{isTouchingLedge} | xIn:{xInput} | FacingRight:{player.FacingRight} | Grab:{grabInput} | Pressing:{xInput == player.FacingRight} | Vel.y:{player.CurrentVelocity.y:F2}");
        
        if (!isTouchingWall)
        {
            Debug.Log("[WALLSLIDE] -> AirState (dejó de tocar pared)");
            stateMachine.ChangeState(player.AirState);
        }
        else if (xInput == 0 || xInput != player.FacingRight)
        {
            Debug.Log($"[WALLSLIDE] -> AirState (dejó de presionar hacia pared, xInput:{xInput}, FacingRight:{player.FacingRight})");
            stateMachine.ChangeState(player.AirState);
        }
        else if (grabInput)
        {
            Debug.Log("[WALLSLIDE] -> WallGrapState (agarrando)");
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
