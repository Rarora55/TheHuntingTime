using UnityEngine;
using System.Collections.Generic;
using System.Collections;

public class PlayerLedgeClimbState : PlayerState
{
    private Vector2 detectedPos;
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
 

        
        player.SetVelocityZero();
        player.transform.position = detectedPos;
        cornerPos = player.DeterminetCornerPos();
        startPos.Set(cornerPos.x - (player.FacingRight * playerData.startOffSet.x), cornerPos.y - playerData.startOffSet.y);
        stopPos.Set(cornerPos.x + (player.FacingRight * playerData.stopOffSet.x), cornerPos.y + playerData.stopOffSet.y);
        player.transform.position = startPos;

        isHanging = true;
    }

    public override void Exit()
    {
        //base.Exit();
        isHanging = false;
        isClimbing = false;
        
    }

    public override void LogicUpdate()
    {
        

        if (!isClimbing)
        {
            player.SetVelocityZero();
            player.transform.position = startPos;
        }
        xInput = player.InputHandler.NormInputX;
        yInput = player.InputHandler.NormInputY;


        if (isAnimationFinish)
        {
            if (isTouchingCeiling)
            {
                stateMachine.ChangeState(player.CrouchIdleState);
            }
            else
            {
                stateMachine.ChangeState(player.IdleState);

            }
        }
        else
        {
            if (xInput == player.FacingRight && isHanging && !isClimbing)
            {
                CheckForSpace();
                isClimbing = true;
                player.anim.SetBool("climbLedge", true);
            }
            else if (yInput == -1 && !isHanging && !isClimbing)
            {
                stateMachine.ChangeState(player.AirState);
            }
        }
        base.LogicUpdate();
    }

    public void SetDetectedPosition (Vector2 pos)
    {
        detectedPos = pos;
    }

    private void CheckForSpace()
    {
        isTouchingCeiling = Physics2D.Raycast(cornerPos + (Vector2.up * 0.015f) + (Vector2.right * player.FacingRight * 0.015f), Vector2.up, playerData.standColliderHeight, playerData.WhatIsGround);

    }
}
