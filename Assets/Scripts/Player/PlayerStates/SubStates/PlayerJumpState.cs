using UnityEngine;
using System.Collections;
using System.Collections.Generic;

public class PlayerJumpState : PlayerAbilityState
{
    private int xInput;
    private float jumpStartTime;
    public PlayerJumpState(Player player, PlayerStateMachine stateMachine, PlayerData playerData, string animBoolName) : base(player, stateMachine, playerData, animBoolName)
    {
    }

    public override void Enter()
    {
        base.Enter();
        player.SetVelocityY(playerData.JumpVelocity);
        jumpStartTime = Time.time;
        //isAbilityDone = true;
    }

    public override void LogicUpdate()
    {
        base.LogicUpdate();
        int xInput = player.InputHandler.NormInputX;
        player.CheckFlip(xInput);
        player.SetVelocityX(playerData.movementVelocity * xInput);
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
