using UnityEngine;

public class PlayerLandedState : PlayerGroundedState
{
    public PlayerLandedState(Player player, PlayerStateMachine stateMachine, PlayerData playerData, string animBoolName) : base(player, stateMachine, playerData, animBoolName)
    {
    }
}
