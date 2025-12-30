using UnityEngine;
using TheHunt.Environment;

public class PlayerWallGrapState : PlayerTouchingWallState
{
    private Vector2 holdPosition;
    private PlayerStaminaIntegration staminaIntegration;
    private StaminaData staminaData;
    
    public PlayerWallGrapState(Player player, PlayerStateMachine stateMachine, PlayerData playerData, string animBoolName) : base(player, stateMachine, playerData, animBoolName)
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
            Debug.LogWarning("<color=red>[WALLGRAB] No hay objeto escalable, no se puede agarrar</color>");
            stateMachine.ChangeState(player.AirState);
            return;
        }

        holdPosition = player.transform.position;
        HoldPosition();
        
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
            staminaIntegration.StartGrappingWall(staminaData);
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
                staminaIntegration.StopGrappingWall();
            }
            stateMachine.ChangeState(player.AirState);
            return;
        }
        
        HoldPosition();
        
        if (!isTouchingWall)
        {
            if (staminaIntegration != null)
            {
                staminaIntegration.StopGrappingWall();
            }
            stateMachine.ChangeState(player.AirState);
        }
        else if (yInput > 0)
        {
            if (staminaIntegration != null)
            {
                staminaIntegration.StopGrappingWall();
            }
            stateMachine.ChangeState(player.WallClimbState);
        }
        else if (yInput < 0 && grabInput)
        {
            IClimbable climbable = player.GetCurrentClimbable();
            if (climbable.AllowsWallSlide())
            {
                if (staminaIntegration != null)
                {
                    staminaIntegration.StopGrappingWall();
                }
                stateMachine.ChangeState(player.WallSlicedState);
            }
        }
        else if (!grabInput)
        {
            IClimbable climbable = player.GetCurrentClimbable();
            if (staminaIntegration != null)
            {
                staminaIntegration.StopGrappingWall();
            }
            
            if (climbable.AllowsWallSlide() && xInput != 0 && xInput == player.FacingRight)
            {
                stateMachine.ChangeState(player.WallSlicedState);
            }
            else
            {
                stateMachine.ChangeState(player.AirState);
            }
        }
        else if (staminaIntegration != null && !staminaIntegration.CanGrapWall())
        {
            staminaIntegration.StopGrappingWall();
            stateMachine.ChangeState(player.AirState);
        }
    }

    private void HoldPosition()
    {
        player.transform.position = holdPosition;
        player.SetVelocityX(0);
        player.SetVelocityY(0);
    }
}
