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
    private bool hasDetectedLadderBelow;
    private bool enteredFromAbove;
    private LedgeType currentLedgeType;

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
        hasDetectedLadderBelow = false;
        
        enteredFromAbove = player.CheckIsGrounded();

        player.SetVelocityZero();
        
        currentLedgeType = player.Collision.DetectLedgeType();
        
        if (enteredFromAbove)
        {
            cornerPos = player.Collision.DetermineCornerPositionFromAbove();
            
            startPos.Set(
                cornerPos.x - (player.FacingRight * playerData.startOffSet.x), 
                cornerPos.y - playerData.startOffSet.y
            );
            
            stopPos.Set(
                cornerPos.x + (player.FacingRight * playerData.stopOffSet.x), 
                cornerPos.y + playerData.stopOffSet.y
            );
        }
        else
        {
            if (currentLedgeType == LedgeType.Corner)
            {
                cornerPos = player.DeterminetCornerPos();
            }
            else if (currentLedgeType == LedgeType.Edge)
            {
                cornerPos = player.Collision.DetermineEdgePosition();
            }
            
            startPos.Set(
                cornerPos.x - (player.FacingRight * playerData.startOffSet.x), 
                cornerPos.y - playerData.startOffSet.y
            );
            
            stopPos.Set(
                cornerPos.x + (player.FacingRight * playerData.stopOffSet.x), 
                cornerPos.y + playerData.stopOffSet.y
            );
        }
        
        player.transform.position = startPos;
        
        SetLedgeAnimation();
        isHanging = true;
    }

    public override void Exit()
    {
        base.Exit();
        player.anim.SetBool("ledge", false);
        player.anim.SetBool("edgeLedge", false);
        player.anim.SetBool("isTouchingCeiling", false);
        isHanging = false;
        
        if (isClimbing)
        {
            if (isTouchingCeiling)
            {
                player.SetColliderHeight(playerData.crouchColliderHeight);
            }
            
            player.transform.position = stopPos;
            player.RB.linearVelocity = Vector2.zero;
            player.RB.angularVelocity = 0f;
            
            isClimbing = false;
        }
    }

    public override void LogicUpdate()
    {
        base.LogicUpdate();

        if (isAnimationFinish)
        {
            player.JustFinishedLedgeClimb = true;
            
            if (CheckForLadderAfterClimb())
            {
                return;
            }
            
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
            xInput = player.InputHandler.NormInputX;
            yInput = player.InputHandler.NormInputY;
            bool grabInput = player.InputHandler.GrabInput;

           
            player.SetVelocityZero();
            player.transform.position = startPos;
           
            
            if (yInput == 1 && isHanging && !isClimbing)
            {
                CheckForSpace();
                isClimbing = true;
                player.anim.SetBool("climbLedge", true);
            }
            else if (yInput == -1 && isHanging && !isClimbing)
            {
                
                if (grabInput && CheckForLadderBelow())
                {
                    EnterLadderFromLedge();
                }
                else
                {
                    stateMachine.ChangeState(player.AirState);
                }
            }
        }


    }
    
    bool CheckForLadderAfterClimb()
    {
        return false;
    }
    
    bool CheckForLadderBelow()
    {
        Vector2 checkPosition = startPos;
        float checkDistance = 2f;
        
        Collider2D[] colliders = Physics2D.OverlapCircleAll(checkPosition, checkDistance);
        
        foreach (var collider in colliders)
        {
            if (collider.CompareTag("FrontLadder"))
            {
                TheHunt.Environment.Ladder ladder = collider.GetComponent<TheHunt.Environment.Ladder>();
                
                if (ladder != null)
                {
                    Vector3 ladderTop = ladder.GetTopPosition();
                    float verticalDistance = Mathf.Abs(startPos.y - ladderTop.y);
                    
                    if (verticalDistance <= 1.5f)
                    {
                        player.SetCurrentLadder(collider);
                        hasDetectedLadderBelow = true;
                        return true;
                    }
                }
            }
        }
        
        return false;
    }
    
    void EnterLadderFromLedge()
    {
        if (player.FacingRight == 1)
        {
            player.Flip();
        }
        
        player.SetVelocityZero();
        player.RB.gravityScale = 0f;
        
        stateMachine.ChangeState(player.LadderClimbState);
    }

    private void CheckForSpace()
    {
        Vector2 checkPosition = stopPos + (Vector2.up * 0.015f);
        
        RaycastHit2D hit = Physics2D.Raycast(checkPosition, Vector2.up,playerData.standColliderHeight, playerData.WhatIsGround);
        
        isTouchingCeiling = hit.collider != null;
        player.anim.SetBool("isTouchingCeiling", isTouchingCeiling);
    }
    
    private void SetLedgeAnimation()
    {
        if (currentLedgeType == LedgeType.Corner)
        {
            player.anim.SetBool("ledge", true);
            player.anim.SetBool("edgeLedge", false);
        }
        else if (currentLedgeType == LedgeType.Edge)
        {
            player.anim.SetBool("ledge", false);
            player.anim.SetBool("edgeLedge", true);
        }
    }
}

