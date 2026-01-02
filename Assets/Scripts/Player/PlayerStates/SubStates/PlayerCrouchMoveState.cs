using UnityEngine;
using System.Collections;
using System.Collections.Generic;

public class PlayerCrouchMoveState : PlayerGroundState
{
    public PlayerCrouchMoveState(Player player, PlayerStateMachine stateMachine, PlayerData playerData, string animBoolName) : base(player, stateMachine, playerData, animBoolName)
    {
    }

    public override void Enter()
    {
        base.Enter();
        player.SetColliderHeight(playerData.crouchColliderHeight);
    }

    public override void Exit()
    {
        base.Exit();
        player.SetColliderHeight(playerData.standColliderHeight);
    }

    public override void LogicUpdate()
    {
        xInput = player.InputHandler.NormInputX;
        yInput = player.InputHandler.NormInputY;
        grabInput = player.InputHandler.GrabInput;
        
        if (CheckGrabLedgeFromAbove())
        {
            return;
        }
        
        base.LogicUpdate();

        player.SetVelocityX(playerData.crouchMovementVelocity * player.FacingRight);
        player.CheckFlip(xInput);

        if (xInput == 0)
            stateMachine.ChangeState(player.CrouchIdleState);
        else if (yInput != -1 && !isTouchingCeiling)
            stateMachine.ChangeState(player.MoveState);
    }
    
    bool CheckGrabLedgeFromAbove()
    {
        if (player.Collision.ShouldAutoGrabLedge())
        {
            Debug.Log("<color=yellow>[AUTO LEDGE CROUCH MOVE] Entrando automáticamente en LedgeState!</color>");
            
            Vector2 cornerPos = player.Collision.DetermineCornerPositionFromAbove();
            
            if (cornerPos != Vector2.zero)
            {
                Debug.Log($"<color=green>[AUTO LEDGE CROUCH MOVE] Corner: {cornerPos}, Player: {player.transform.position}, FacingRight: {player.FacingRight}</color>");
                
                float playerX = player.transform.position.x;
                float cornerX = cornerPos.x;
                
                bool shouldFaceRight = playerX > cornerX;
                
                if (shouldFaceRight && player.FacingRight != 1)
                {
                    Debug.Log("<color=magenta>[AUTO LEDGE CROUCH MOVE] Volteando a la derecha</color>");
                    player.Flip();
                }
                else if (!shouldFaceRight && player.FacingRight != -1)
                {
                    Debug.Log("<color=magenta>[AUTO LEDGE CROUCH MOVE] Volteando a la izquierda</color>");
                    player.Flip();
                }
                
                player.SetColliderHeight(playerData.standColliderHeight);
                player.SetVelocityZero();
                stateMachine.ChangeState(player.WallLedgeState);
                return true;
            }
            else
            {
                Debug.Log("<color=red>[AUTO LEDGE CROUCH MOVE] cornerPos es Zero, no se puede agarrar</color>");
            }
        }
        
        return false;
    }
}
