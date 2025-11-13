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
        
        Debug.Log($"<color=cyan>━━━━━━━━ CROUCH IDLE ENTER ━━━━━━━━</color>");
        Debug.Log($"[CROUCH] Posición: {player.transform.position}");
        Debug.Log($"[CROUCH] Velocidad: {player.RB.linearVelocity}");
        Debug.Log($"[CROUCH] JustFinishedLedgeClimb: {player.JustFinishedLedgeClimb}");
        Debug.Log($"<color=cyan>━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━</color>");
    }

    public override void Exit()
    {
        base.Exit();
        player.SetColliderHeight(playerData.standColliderHeight);
    }

    public override void LogicUpdate()
    {
        base.LogicUpdate();
        
        if (player.JustFinishedLedgeClimb)
        {
            Debug.Log("<color=yellow>[CROUCH] Reseteando JustFinishedLedgeClimb flag AL INICIO</color>");
            player.JustFinishedLedgeClimb = false;
        }
        
        if (xInput != 0)
            stateMachine.ChangeState(player.CrouchMoveState);
        else if (yInput != -1 && !isTouchingCeiling)
            stateMachine.ChangeState(player.IdleState);
    }
}
