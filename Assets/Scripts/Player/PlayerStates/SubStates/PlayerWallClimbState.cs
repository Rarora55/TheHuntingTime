using UnityEngine;

public class PlayerWallClimbState : PlayerTouchingWallState
{
    private bool hasTriggeredLedge;
    
    public PlayerWallClimbState(Player player, PlayerStateMachine stateMachine, PlayerData playerData, string animBoolName) : base(player, stateMachine, playerData, animBoolName)
    {
    }

    public override void DoChecks()
    {
        base.DoChecks();
    }

    public override void Enter()
    {
        base.Enter();
        hasTriggeredLedge = false;
        Debug.Log("<color=lime>[WALLCLIMB] Enter - Escalando pared</color>");
    }

    public override void LogicUpdate()
    {
        xInput = player.InputHandler.NormInputX;
        yInput = player.InputHandler.NormInputY;
        grabInput = player.InputHandler.GrabInput;

        Debug.Log($"[WALLCLIMB] Ground:{isGrounded} | Wall:{isTouchingWall} | Ledge:{isTouchingLedge} | xIn:{xInput} | yIn:{yInput} | Grab:{grabInput} | hasTriggered:{hasTriggeredLedge}");

        if (isTouchingWall && !isTouchingLedge && yInput == 1 && grabInput && !hasTriggeredLedge)
        {
            Debug.Log("[WALLCLIMB] -> WallLedgeState (llegó al borde, trepa automáticamente)");
            hasTriggeredLedge = true;
            stateMachine.ChangeState(player.WallLedgeState);
            return;
        }
        
        if (!isTouchingWall)
        {
            Debug.Log("[WALLCLIMB] -> AirState (no toca pared)");
            stateMachine.ChangeState(player.AirState);
            return;
        }
        
        if (!grabInput)
        {
            if (xInput != 0 && xInput == player.FacingRight)
            {
                Debug.Log("[WALLCLIMB] -> WallSlicedState (soltó Grab pero presiona hacia pared)");
                stateMachine.ChangeState(player.WallSlicedState);
            }
            else
            {
                Debug.Log("[WALLCLIMB] -> AirState (soltó Grab sin presionar hacia pared)");
                stateMachine.ChangeState(player.AirState);
            }
            return;
        }
        
        if (yInput != 1)
        {
            Debug.Log("[WALLCLIMB] -> WallGrapState (dejó de subir pero mantiene Grab)");
            stateMachine.ChangeState(player.WallGrapState);
            return;
        }

        player.SetVelocityY(playerData.WallClimbVelocity);
    }
}
