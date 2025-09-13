using UnityEngine;
using System.Collections;
using System.Collections.Generic;

public class PlayerJumpState : PlayerAbilityState
{
    public PlayerJumpState(Player player, PlayerStateMachine stateMachine, PlayerData playerData, string animBoolName) : base(player, stateMachine, playerData, animBoolName)
    {
    }

    public override void Enter()
    {
        base.Enter();
        player.SetVelocityY(playerData.JumpVelocity);
        isAbilityDone = true;
    }
}
