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
    private bool isInitialized;

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
        
        if (isInitialized)
        {
            Debug.Log("<color=red>[LEDGE] ⚠️ RE-ENTRADA detectada - ignorando</color>");
            return;
        }
        
        isHanging = false;
        isClimbing = false;
        isInitialized = true;

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
        
        CheckForSpace();
        
        Debug.Log($"<color=yellow>========== [LEDGE CLIMB] COMPLETO ==========</color>");
    }

    public override void Exit()
    {
        base.Exit();
        player.anim.SetBool("ledge", false);
        player.anim.SetBool("isTouchingCeiling", false);
        isHanging = false;
        isInitialized = false;
        
        if (isClimbing)
        {
            Debug.Log($"<color=green>[LEDGE] EXIT - Moviendo a stopPos: {stopPos}</color>");
            player.transform.position = stopPos;
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
            if (isTouchingCeiling)
            {
                Debug.Log("<color=red>[LEDGE] → CrouchIdleState (hay techo)</color>");
                stateMachine.ChangeState(player.CrouchIdleState);
            }
            else
            {
                Debug.Log("<color=green>[LEDGE] → IdleState (sin techo)</color>");
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
        isTouchingCeiling = Physics2D.Raycast(cornerPos + (Vector2.up * 0.015f) + (Vector2.right * player.FacingRight * 0.015f), Vector2.up, playerData.standColliderHeight, playerData.WhatIsGround);
        player.anim.SetBool("isTouchingCeiling", isTouchingCeiling);
        
        Debug.Log($"<color={(isTouchingCeiling ? "red" : "green")}>[LEDGE] CheckForSpace - isTouchingCeiling: {isTouchingCeiling}</color>");
    }
}
