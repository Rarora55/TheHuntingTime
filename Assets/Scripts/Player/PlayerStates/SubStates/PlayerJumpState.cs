using UnityEngine;
using System.Collections;
using System.Collections.Generic;
using TheHunt.Environment;

public class PlayerJumpState : PlayerAbilityState
{
    private int xInput;
    private int yInput;
    private float jumpStartTime;
    private IJumpZone usedJumpZone;
    private JumpDirection jumpDirection;
    private bool isContextualJump;
    
    public PlayerJumpState(Player player, PlayerStateMachine stateMachine, PlayerData playerData, string animBoolName) : base(player, stateMachine, playerData, animBoolName)
    {
    }

    public override void Enter()
    {
        base.Enter();
        
        xInput = player.InputHandler.NormInputX;
        yInput = player.InputHandler.NormInputY;
        jumpStartTime = Time.time;
        isContextualJump = false;
        
        PlayerStaminaIntegration staminaIntegration = player.GetComponent<PlayerStaminaIntegration>();
        StaminaController staminaController = player.GetComponent<StaminaController>();
        
        if (staminaIntegration != null && staminaController != null)
        {
            System.Reflection.FieldInfo field = typeof(StaminaController).GetField("staminaData", 
                System.Reflection.BindingFlags.NonPublic | System.Reflection.BindingFlags.Instance);
            if (field != null)
            {
                StaminaData staminaData = field.GetValue(staminaController) as StaminaData;
                if (staminaData != null)
                {
                    if (!staminaIntegration.TryConsumeJumpStamina(staminaData))
                    {
                        stateMachine.ChangeState(player.IdleState);
                        return;
                    }
                }
            }
        }

        if (player.IsInJumpZone())
        {
            usedJumpZone = player.GetCurrentJumpZone();
            jumpDirection = DetermineJumpDirection(xInput, yInput);

            if (usedJumpZone.CanJumpInDirection(jumpDirection))
            {
                Vector2 jumpForce = usedJumpZone.GetJumpForce(jumpDirection);
                player.SetVelocityX(jumpForce.x);
                player.SetVelocityY(jumpForce.y);
                isContextualJump = true;
            }
            else
            {
                PerformNormalJump();
            }
        }
        else
        {
            PerformNormalJump();
        }
    }

    private void PerformNormalJump()
    {
        player.SetVelocityY(playerData.JumpVelocity);
        player.RB.gravityScale = playerData.jumpGravityScale;
        isContextualJump = false;
    }

    private JumpDirection DetermineJumpDirection(int xInput, int yInput)
    {
        if (yInput > 0)
            return JumpDirection.Up;
        else if (yInput < 0)
            return JumpDirection.Down;
        else if (xInput > 0)
            return JumpDirection.Right;
        else if (xInput < 0)
            return JumpDirection.Left;
        else
            return JumpDirection.Up;
    }

    public override void LogicUpdate()
    {
        base.LogicUpdate();
        int xInput = player.InputHandler.NormInputX;
        
        if (!isContextualJump)
        {
            player.CheckFlip(xInput);
            player.SetVelocityX(playerData.movementVelocity * xInput);
        }
        
        float timeSinceJump = Time.time - jumpStartTime;

        if(timeSinceJump > 0.3f)
        {
            if(player.CurrentVelocity.y <= 0f || Time.time >= startTime + 0.1f)
            isAbilityDone = true;
        }
       
        if (timeSinceJump > 1f && player.CurrentVelocity.y <= 0f)
        {
            isAbilityDone = true;
        }
    }
}
