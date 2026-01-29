using UnityEngine;
using System.Collections.Generic;
using System.Collections;

public class PlayerLandState : PlayerGroundState
{
    public PlayerLandState(Player player, PlayerStateMachine stateMachine, PlayerData playerData, string animBoolName) : base(player, stateMachine, playerData, animBoolName)
    {
    }

    public override void Enter()
    {
        base.Enter();
        player.anim.SetBool("isRunning", false);
        
        PlayerHealthIntegration healthIntegration = player.GetComponent<PlayerHealthIntegration>();
        if (healthIntegration != null)
        {
            healthIntegration.OnPlayerLanded();
        }
    }

    public override void LogicUpdate()
    {
        base.LogicUpdate();
        if(xInput != 0)
        {
            stateMachine.ChangeState(player.MoveState);
        }else if (isAnimationFinish)
        {
            stateMachine.ChangeState(player.IdleState);
        }
    }
}
