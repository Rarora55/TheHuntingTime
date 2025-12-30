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
    }

    public override void LogicUpdate()
    {
        xInput = player.InputHandler.NormInputX;
        yInput = player.InputHandler.NormInputY;
        grabInput = player.InputHandler.GrabInput;

        if (!player.CanClimbHere())
        {
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

        if (isTouchingWall && !isTouchingLedge && yInput == 1 && grabInput && !hasTriggeredLedge && canTriggerLedge && isValidLedge)
        {
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
                stateMachine.ChangeState(player.WallSlicedState);
            }
            else
            {
                stateMachine.ChangeState(player.AirState);
            }
            return;
        }
        
        if (yInput != 1)
        {
            if (staminaIntegration != null)
            {
                staminaIntegration.StopClimbing();
            }
            stateMachine.ChangeState(player.WallGrapState);
            return;
        }
        
        if (staminaIntegration != null && !staminaIntegration.CanClimb())
        {
            staminaIntegration.StopClimbing();
            stateMachine.ChangeState(player.WallGrapState);
            return;
        }

        player.SetVelocityY(climbSpeed);
    }
}
