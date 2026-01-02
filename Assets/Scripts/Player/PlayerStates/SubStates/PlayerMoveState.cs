using UnityEngine;
using System.Collections;
using System.Collections.Generic;

public class PlayerMoveState : PlayerGroundState
{
    private bool isTurning;
    private bool turnStartTime;
    private bool runInput;
    private const float MAX_TURN_DURATION = 0.4f;
    
    private PlayerStaminaIntegration staminaIntegration;
    private StaminaData staminaData;
    
    public PlayerMoveState(Player player, PlayerStateMachine stateMachine, PlayerData playerData, string animBoolName) : base(player, stateMachine, playerData, animBoolName)
    {
    }

   

    public override void DoChecks()
    {
        base.DoChecks();
    }

    public override void Enter()
    {
        base.Enter();
        isTurning = false;
        player.anim.SetBool("turnIt", false);
        
        staminaIntegration = player.GetComponent<PlayerStaminaIntegration>();
        StaminaController controller = player.GetComponent<StaminaController>();
        
        if (controller != null)
        {
            System.Reflection.FieldInfo field = typeof(StaminaController).GetField("staminaData", 
                System.Reflection.BindingFlags.NonPublic | System.Reflection.BindingFlags.Instance);
            if (field != null)
            {
                staminaData = field.GetValue(controller) as StaminaData;
            }
        }
    }

    public override void Exit()
    {
        base.Exit();
        player.anim.SetBool("turnIt", false);
        player.anim.SetBool("isRunning", false);
        isTurning = false;
        
        if (staminaIntegration != null)
        {
            staminaIntegration.StopRunning();
        }
    }

    public override void LogicUpdate()
    {
        xInput = player.InputHandler.NormInputX;
        yInput = player.InputHandler.NormInputY;
        grabInput = player.InputHandler.GrabInput;
        runInput = player.InputHandler.RunInput;
        
        if (player.JustFinishedLedgeClimb)
        {
            player.JustFinishedLedgeClimb = false;
        }
        
        if (CheckGrabLedgeFromAbove())
        {
            return;
        }
        
        base.LogicUpdate();

        /*
       if (isTurning)
       {
       //Para girar de un turno y produce conflicto
           return;
       }
       */

        
        if (xInput == 0)
        {
            //
            if (xInput == 0 || xInput == player.FacingRight)
            {
                CancelTurn();
            }
            //
            stateMachine.ChangeState(player.IdleState);
        }
        else if (yInput == -1 && !player.CheckIfTouchingWall())
        {
            stateMachine.ChangeState(player.CrouchMoveState);
        }
        else if (xInput != 0 && xInput != player.FacingRight)
        {
            StartTurn();
        }
        else
        {
            bool canRun = staminaIntegration == null || staminaIntegration.CanRun();
            bool shouldRun = runInput && canRun;
            
            if (shouldRun && staminaIntegration != null && staminaData != null)
            {
                staminaIntegration.StartRunning(staminaData);
            }
            else if (!shouldRun && staminaIntegration != null)
            {
                staminaIntegration.StopRunning();
            }
            
            float currentVelocity = shouldRun ? playerData.runVelocity : playerData.movementVelocity;
            player.SetVelocityX(currentVelocity * xInput);
            player.anim.SetBool("isRunning", shouldRun);
        }



       
    }
    
    bool CheckGrabLedgeFromAbove()
    {
        if (player.Collision.ShouldAutoGrabLedge())
        {
            Debug.Log("<color=yellow>[AUTO LEDGE MOVE] Entrando automáticamente en LedgeState!</color>");
            
            Vector2 cornerPos = player.Collision.DetermineCornerPositionFromAbove();
            
            if (cornerPos != Vector2.zero)
            {
                Debug.Log($"<color=green>[AUTO LEDGE MOVE] Corner: {cornerPos}, Player: {player.transform.position}, FacingRight: {player.FacingRight}</color>");
                
                float playerX = player.transform.position.x;
                float cornerX = cornerPos.x;
                
                bool shouldFaceRight = playerX > cornerX;
                
                if (shouldFaceRight && player.FacingRight != 1)
                {
                    Debug.Log("<color=magenta>[AUTO LEDGE MOVE] Volteando a la derecha</color>");
                    player.Flip();
                }
                else if (!shouldFaceRight && player.FacingRight != -1)
                {
                    Debug.Log("<color=magenta>[AUTO LEDGE MOVE] Volteando a la izquierda</color>");
                    player.Flip();
                }
                
                player.SetVelocityZero();
                stateMachine.ChangeState(player.WallLedgeState);
                return true;
            }
            else
            {
                Debug.Log("<color=red>[AUTO LEDGE MOVE] cornerPos es Zero, no se puede agarrar</color>");
            }
        }
        
        return false;
    }

    public override void PhysicsUpdate()
    {
        base.PhysicsUpdate();
    }

    public override void AnimationFinishTrigger()
    {
        base.AnimationFinishTrigger();
        if (isTurning)
        {
            CompleteTurn();
        }

    }
    private void StartTurn()
    {
        isTurning = true;
        player.anim.SetBool("turnIt", true);
        player.SetVelocityX(0f);
    }

    private void CompleteTurn()
    {
        player.Flip();
        isTurning = false;
        player.anim.SetBool("turnIt", false);
    }

    private void CancelTurn()
    {
        isTurning = false;
        player.anim.SetBool("turnIt", false);
    }
}
