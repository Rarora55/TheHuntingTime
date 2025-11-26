using UnityEngine;
using System.Collections.Generic;
using System.Collections;

public class PlayerLedgeClimbState : PlayerState
{
    private Vector2 cornerPos;
    private Vector2 startPos;
    private Vector2 stopPos;

    private bool isHanging;
    private bool isClimbing;
    private bool isTouchingCeiling;

    private int xInput;
    private int yInput;
    public PlayerLedgeClimbState(Player player, PlayerStateMachine stateMachine, PlayerData playerData, string animBoolName) : base(player, stateMachine, playerData, animBoolName)
    {
    }

    public override void AnimationFinishTrigger()
    {
        base.AnimationFinishTrigger();
        player.anim.SetBool("climbLedge", false);

    }

    public override void AnimationTrigger()
    {
        base.AnimationTrigger();
        isHanging = true;
    }

    public override void Enter()
    {
        base.Enter();
        
        isHanging = false;
        isClimbing = false;
        isTouchingCeiling = false;

        player.SetVelocityZero();
        
        Debug.Log($"<color=yellow>========== [LEDGE CLIMB] ENTER ==========</color>");
        Debug.Log($"[LEDGE] Player actual pos ANTES de calcular: {player.transform.position}");
        Debug.Log($"[LEDGE] FacingRight: {player.FacingRight}");
        
        cornerPos = player.DeterminetCornerPos();
        
        Debug.Log($"[LEDGE] CornerPos obtenido: {cornerPos}");
        
        startPos.Set(
            cornerPos.x - (player.FacingRight * playerData.startOffSet.x), 
            cornerPos.y - playerData.startOffSet.y
        );
        
        stopPos.Set(
            cornerPos.x + (player.FacingRight * playerData.stopOffSet.x), 
            cornerPos.y + playerData.stopOffSet.y
        );
        
        Debug.Log($"[LEDGE] StartPos: {startPos} (offset: {playerData.startOffSet})");
        Debug.Log($"[LEDGE] StopPos: {stopPos} (offset: {playerData.stopOffSet})");
        
        player.transform.position = startPos;
        
        Debug.DrawLine(cornerPos, cornerPos + Vector2.up * 0.6f, Color.yellow, 4f);
        Debug.DrawLine(startPos, startPos + Vector2.right * player.FacingRight * 0.3f, Color.cyan, 4f);
        Debug.DrawLine(stopPos, stopPos + Vector2.left * player.FacingRight * 0.3f, Color.magenta, 4f);

        isHanging = true;
        
        Debug.Log($"<color=yellow>========== [LEDGE CLIMB] COMPLETO ==========</color>");
    }

    public override void Exit()
    {
        base.Exit();
        player.anim.SetBool("ledge", false);
        player.anim.SetBool("isTouchingCeiling", false);
        isHanging = false;
        
        if (isClimbing)
        {
            Debug.Log($"<color=green>━━━━━━━━ LEDGE EXIT (CLIMBING) ━━━━━━━━</color>");
            Debug.Log($"[LEDGE] Moviendo a stopPos: {stopPos}");
            
            if (isTouchingCeiling)
            {
                Debug.Log($"[LEDGE EXIT] HAY TECHO - Reduciendo collider ANTES de mover");
                player.SetColliderHeight(playerData.crouchColliderHeight);
            }
            
            player.transform.position = stopPos;
            player.RB.linearVelocity = Vector2.zero;
            player.RB.angularVelocity = 0f;
            
            Debug.Log($"[LEDGE] Nueva posición: {player.transform.position}");
            Debug.Log($"[LEDGE] isTouchingCeiling (guardado): {isTouchingCeiling}");
            Debug.Log($"<color=green>━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━</color>");
            isClimbing = false;
        }
        else
        {
            Debug.Log("<color=yellow>[LEDGE] EXIT - Sin climb, cancelado</color>");
        }
    }

    public override void LogicUpdate()
    {
        base.LogicUpdate();

        Debug.Log($"[LEDGE] LogicUpdate - isAnimationFinish:{isAnimationFinish} | isTouchingCeiling:{isTouchingCeiling} | isClimbing:{isClimbing}");

        if (isAnimationFinish)
        {
            player.JustFinishedLedgeClimb = true;
            
            if (isTouchingCeiling)
            {
                Debug.Log("<color=red>[LEDGE] → CrouchIdleState (hay techo, marcado JustFinishedLedgeClimb)</color>");
                stateMachine.ChangeState(player.CrouchIdleState);
            }
            else
            {
                Debug.Log("<color=green>[LEDGE] → IdleState (sin techo, marcado JustFinishedLedgeClimb)</color>");
                stateMachine.ChangeState(player.IdleState);
            }
        }
        else
        {
            xInput = player.InputHandler.NormInputX;
            yInput = player.InputHandler.NormInputY;

           
            player.SetVelocityZero();
            player.transform.position = startPos;
           
            
                if (xInput == player.FacingRight && isHanging && !isClimbing)
                {
                    CheckForSpace();
                    isClimbing = true;
                    player.anim.SetBool("climbLedge", true);
                }
                else if (yInput == -1 && isHanging && !isClimbing)
                {
                    stateMachine.ChangeState(player.AirState);
                }
        }


    }

    private void CheckForSpace()
    {
        Vector2 checkPosition = stopPos + (Vector2.up * 0.015f);
        
        RaycastHit2D hit = Physics2D.Raycast(checkPosition, Vector2.up,playerData.standColliderHeight, playerData.WhatIsGround);
        
        isTouchingCeiling = hit.collider != null;
        player.anim.SetBool("isTouchingCeiling", isTouchingCeiling);
        
        Debug.Log($"<color=yellow>━━━━━━━━ CHECK FOR SPACE ━━━━━━━━</color>");
        Debug.Log($"[LEDGE] CheckPosition: {checkPosition} (stopPos:{stopPos} + 0.015 arriba)");
        Debug.Log($"[LEDGE] Raycast Dir: UP, Distance: {playerData.standColliderHeight}");
        Debug.Log($"[LEDGE] LayerMask: {playerData.WhatIsGround.value}");
        Debug.Log($"<color={(isTouchingCeiling ? "red" : "green")}>[LEDGE] Hit: {(hit.collider != null ? hit.collider.name : "NINGUNO")} → isTouchingCeiling: {isTouchingCeiling}</color>");
        if (hit.collider != null)
        {
            Debug.Log($"[LEDGE] Hit Point: {hit.point}, Distance: {hit.distance:F3}");
        }
        Debug.Log($"<color=yellow>━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━</color>");
        
        Debug.DrawRay(checkPosition, Vector2.up * playerData.standColliderHeight, isTouchingCeiling ? Color.red : Color.green, 5f);
        if (hit.collider != null)
        {
            Debug.DrawLine(checkPosition, hit.point, Color.magenta, 5f);
        }
    }
}
