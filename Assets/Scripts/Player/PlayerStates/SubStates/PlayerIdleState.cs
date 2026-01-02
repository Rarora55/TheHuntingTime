using UnityEngine;
using System.Collections;
using System.Collections.Generic;

public class PlayerIdleState : PlayerGroundState
{
   

    public PlayerIdleState(Player player, PlayerStateMachine stateMachine, PlayerData playerData, string animBoolName) : base(player, stateMachine, playerData, animBoolName)
    {
    }

    public override void DoChecks()
    {
        base.DoChecks();
    }
    
    public override void Enter()
    {
        base.Enter();
        player.SetVelocityX(0);
        player.anim.SetBool("isRunning", false);
    }

    public override void Exit()
    {
        base.Exit();
    }

    public override void LogicUpdate()
    {
        xInput = player.InputHandler.NormInputX;
        yInput = player.InputHandler.NormInputY;
        grabInput = player.InputHandler.GrabInput;
        
        if (player.JustFinishedLedgeClimb)
        {
            player.JustFinishedLedgeClimb = false;
        }
        
        if (CheckGrabLedgeFromAbove())
        {
            return;
        }
        
        base.LogicUpdate();

        if (xInput != 0)
        {
            stateMachine.ChangeState(player.MoveState);
        }
        else if (yInput == -1 && !player.CheckIfTouchingWall())
        {
            stateMachine.ChangeState(player.CrouchIdleState);
        }
    }

    public override void PhysicsUpdate()
    {
        base.PhysicsUpdate();
    }
    
    bool CheckGrabLedgeFromAbove()
    {
        if (player.Collision.ShouldAutoGrabLedge())
        {
            Debug.Log("<color=yellow>[AUTO LEDGE IDLE] Entrando automáticamente en LedgeState!</color>");
            
            Vector2 cornerPos = player.Collision.DetermineCornerPositionFromAbove();
            
            if (cornerPos != Vector2.zero)
            {
                Debug.Log($"<color=green>[AUTO LEDGE IDLE] Corner: {cornerPos}, Player: {player.transform.position}, FacingRight: {player.FacingRight}</color>");
                
                float playerX = player.transform.position.x;
                float cornerX = cornerPos.x;
                
                bool shouldFaceRight = playerX > cornerX;
                
                if (shouldFaceRight && player.FacingRight != 1)
                {
                    Debug.Log("<color=magenta>[AUTO LEDGE IDLE] Volteando a la derecha</color>");
                    player.Flip();
                }
                else if (!shouldFaceRight && player.FacingRight != -1)
                {
                    Debug.Log("<color=magenta>[AUTO LEDGE IDLE] Volteando a la izquierda</color>");
                    player.Flip();
                }
                
                player.SetVelocityZero();
                stateMachine.ChangeState(player.WallLedgeState);
                return true;
            }
            else
            {
                Debug.Log("<color=red>[AUTO LEDGE IDLE] cornerPos es Zero, no se puede agarrar</color>");
            }
        }
        
        return false;
    }

   
}
