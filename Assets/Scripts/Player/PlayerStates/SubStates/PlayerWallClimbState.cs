using UnityEngine;
using TheHunt.Environment;

public class PlayerWallClimbState : PlayerTouchingWallState
{
    private bool hasTriggeredLedge;
    private float startYPosition;
    private const float MIN_CLIMB_DISTANCE = 0.3f;
    private const float MIN_LEDGE_HEIGHT = 0.2f;

    private IClimbable currentClimbable;
    private float climbSpeed;
    private PlayerStaminaIntegration staminaIntegration;
    private StaminaData staminaData;
    
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

        if (!player.CanClimbHere())
        {
            Debug.LogWarning("<color=red>[WALLCLIMB] No hay objeto escalable aquí, cancelando escalamiento</color>");
            stateMachine.ChangeState(player.AirState);
            return;
        }

        currentClimbable = player.GetCurrentClimbable();
        climbSpeed = currentClimbable.GetClimbSpeed();
        
        hasTriggeredLedge = false;
        startYPosition = player.transform.position.y;
        
        staminaIntegration = player.GetComponent<PlayerStaminaIntegration>();
        StaminaController staminaController = player.GetComponent<StaminaController>();
        
        if (staminaController != null)
        {
            System.Reflection.FieldInfo field = typeof(StaminaController).GetField("staminaData", 
                System.Reflection.BindingFlags.NonPublic | System.Reflection.BindingFlags.Instance);
            if (field != null)
            {
                staminaData = field.GetValue(staminaController) as StaminaData;
            }
        }
        
        if (staminaIntegration != null && staminaData != null)
        {
            staminaIntegration.StartClimbing(staminaData);
        }
        
        Debug.Log($"<color=lime>[WALLCLIMB] Enter - Escalando {currentClimbable.GetClimbType()} desde Y:{startYPosition:F2} a velocidad {climbSpeed}</color>");
    }

    public override void LogicUpdate()
    {
        xInput = player.InputHandler.NormInputX;
        yInput = player.InputHandler.NormInputY;
        grabInput = player.InputHandler.GrabInput;

        if (!player.CanClimbHere())
        {
            Debug.LogWarning("<color=red>[WALLCLIMB] -> AirState (perdió contacto con objeto escalable)</color>");
            if (staminaIntegration != null)
            {
                staminaIntegration.StopClimbing();
            }
            stateMachine.ChangeState(player.AirState);
            return;
        }

        currentClimbable = player.GetCurrentClimbable();
        climbSpeed = currentClimbable.GetClimbSpeed();

        float distanceClimbed = player.transform.position.y - startYPosition;
        bool canTriggerLedge = distanceClimbed >= MIN_CLIMB_DISTANCE;
        bool isValidLedge = player.Collision.IsValidLedge(MIN_LEDGE_HEIGHT);

        Debug.Log($"[WALLCLIMB] Type:{currentClimbable.GetClimbType()} | Ground:{isGrounded} | Wall:{isTouchingWall} | Ledge:{isTouchingLedge} | Climbed:{distanceClimbed:F2} | CanTrigger:{canTriggerLedge} | ValidLedge:{isValidLedge} | hasTriggered:{hasTriggeredLedge}");

        if (isTouchingWall && !isTouchingLedge && yInput == 1 && grabInput && !hasTriggeredLedge && canTriggerLedge && isValidLedge)
        {
            Debug.Log("<color=yellow>[WALLCLIMB] -> WallLedgeState (llegó al borde válido después de escalar suficiente)</color>");
            hasTriggeredLedge = true;
            if (staminaIntegration != null)
            {
                staminaIntegration.StopClimbing();
            }
            stateMachine.ChangeState(player.WallLedgeState);
            return;
        }
        
        if (!isTouchingWall)
        {
            Debug.Log("[WALLCLIMB] -> AirState (no toca pared)");
            if (staminaIntegration != null)
            {
                staminaIntegration.StopClimbing();
            }
            stateMachine.ChangeState(player.AirState);
            return;
        }
        
        if (!grabInput)
        {
            if (staminaIntegration != null)
            {
                staminaIntegration.StopClimbing();
            }
            
            if (currentClimbable.AllowsWallSlide() && xInput != 0 && xInput == player.FacingRight)
            {
                Debug.Log("[WALLCLIMB] -> WallSlicedState (soltó Grab pero el objeto permite deslizamiento)");
                stateMachine.ChangeState(player.WallSlicedState);
            }
            else
            {
                Debug.Log("[WALLCLIMB] -> AirState (soltó Grab y el objeto no permite deslizamiento)");
                stateMachine.ChangeState(player.AirState);
            }
            return;
        }
        
        if (yInput != 1)
        {
            Debug.Log("[WALLCLIMB] -> WallGrapState (dejó de subir pero mantiene Grab)");
            if (staminaIntegration != null)
            {
                staminaIntegration.StopClimbing();
            }
            stateMachine.ChangeState(player.WallGrapState);
            return;
        }
        
        if (staminaIntegration != null && !staminaIntegration.CanClimb())
        {
            Debug.Log("<color=red>[WALLCLIMB] -> WallGrapState (sin estamina para continuar escalando)</color>");
            staminaIntegration.StopClimbing();
            stateMachine.ChangeState(player.WallGrapState);
            return;
        }

        player.SetVelocityY(climbSpeed);
    }
}
