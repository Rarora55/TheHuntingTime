using UnityEngine;

public class PlayerCrouchIdleState : PlayerGroundState
{
    public PlayerCrouchIdleState(Player player, PlayerStateMachine stateMachine, PlayerData playerData, string animBoolName) : base(player, stateMachine, playerData, animBoolName)
    {
    }

    public override void Enter()
    {
        base.Enter();
        player.SetVelocityZero();
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
            stateMachine.ChangeState(player.CrouchMoveState);
        }
        else if (yInput != -1)
        {
            if (!isTouchingCeiling)
            {
                stateMachine.ChangeState(player.IdleState);
            }
            else
            {
                Debug.Log("<color=yellow>[CROUCH IDLE] No se puede levantar: hay techo encima</color>");
            }
        }
    }
    
    bool CheckGrabLedgeFromAbove()
    {
        if (player.Collision.ShouldAutoGrabLedge())
        {
            Debug.Log("<color=yellow>[AUTO LEDGE CROUCH IDLE] Entrando automáticamente en LedgeState!</color>");
            
            Vector2 cornerPos = player.Collision.DetermineCornerPositionFromAbove();
            
            if (cornerPos != Vector2.zero)
            {
                Debug.Log($"<color=green>[AUTO LEDGE CROUCH IDLE] Corner: {cornerPos}, Player: {player.transform.position}, FacingRight: {player.FacingRight}</color>");
                
                float playerX = player.transform.position.x;
                float cornerX = cornerPos.x;
                
                bool shouldFaceRight = playerX > cornerX;
                
                if (shouldFaceRight && player.FacingRight != 1)
                {
                    Debug.Log("<color=magenta>[AUTO LEDGE CROUCH IDLE] Volteando a la derecha</color>");
                    player.Flip();
                }
                else if (!shouldFaceRight && player.FacingRight != -1)
                {
                    Debug.Log("<color=magenta>[AUTO LEDGE CROUCH IDLE] Volteando a la izquierda</color>");
                    player.Flip();
                }
                
                player.SetColliderHeight(playerData.standColliderHeight);
                player.SetVelocityZero();
                stateMachine.ChangeState(player.WallLedgeState);
                return true;
            }
            else
            {
                Debug.Log("<color=red>[AUTO LEDGE CROUCH IDLE] cornerPos es Zero, no se puede agarrar</color>");
            }
        }
        
        return false;
    }
}
