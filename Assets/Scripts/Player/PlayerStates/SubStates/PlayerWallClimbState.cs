using UnityEngine;

public class PlayerWallClimbState : PlayerTouchingWallState
{
    private bool hasTriggeredLedge;
    private float startYPosition;
    private const float MIN_CLIMB_DISTANCE = 0.3f;
    private const float MIN_LEDGE_HEIGHT = 0.2f;
    
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
        startYPosition = player.transform.position.y;
        Debug.Log($"<color=lime>[WALLCLIMB] Enter - Escalando pared desde Y:{startYPosition:F2}</color>");
    }

    public override void LogicUpdate()
    {
        xInput = player.InputHandler.NormInputX;
        yInput = player.InputHandler.NormInputY;
        grabInput = player.InputHandler.GrabInput;

        float distanceClimbed = player.transform.position.y - startYPosition;
        bool canTriggerLedge = distanceClimbed >= MIN_CLIMB_DISTANCE;
        bool isValidLedge = player.Collision.IsValidLedge(MIN_LEDGE_HEIGHT);

        Debug.Log($"[WALLCLIMB] Ground:{isGrounded} | Wall:{isTouchingWall} | Ledge:{isTouchingLedge} | Climbed:{distanceClimbed:F2} | CanTrigger:{canTriggerLedge} | ValidLedge:{isValidLedge} | hasTriggered:{hasTriggeredLedge}");

        if (isTouchingWall && !isTouchingLedge && yInput == 1 && grabInput && !hasTriggeredLedge && canTriggerLedge && isValidLedge)
        {
            Debug.Log("<color=yellow>[WALLCLIMB] -> WallLedgeState (llegó al borde válido después de escalar suficiente)</color>");
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
