using UnityEngine;

public class PlayerWallClimbState : PlayerTouchingWallState
{
    public PlayerWallClimbState(Player player, PlayerStateMachine stateMachine, PlayerData playerData, string animBoolName) : base(player, stateMachine, playerData, animBoolName)
    {
    }

    public override void DoChecks()
    {
        base.DoChecks();
    }

    public override void Enter()
    {
        base.Enter();
        Debug.Log("<color=lime>[WALLCLIMB] Enter - Escalando pared</color>");
    }

    public override void LogicUpdate()
    {
        xInput = player.InputHandler.NormInputX;
        yInput = player.InputHandler.NormInputY;
        grabInput = player.InputHandler.GrabInput;

        Debug.Log($"[WALLCLIMB] Ground:{isGrounded} | Wall:{isTouchingWall} | Ledge:{isTouchingLedge} | yIn:{yInput} | Grab:{grabInput}");

        if (!isTouchingWall)
        {
            Debug.Log("[WALLCLIMB] -> AirState (no toca pared)");
            stateMachine.ChangeState(player.AirState);
        }
        else if (isTouchingWall && !isTouchingLedge && grabInput)
        {
            Debug.Log("[WALLCLIMB] -> WallLedgeState (llegó al borde)");
            stateMachine.ChangeState(player.WallLedgeState);
        }
        else if (yInput != 1)
        {
            Debug.Log("[WALLCLIMB] -> WallGrapState (dejó de subir)");
            stateMachine.ChangeState(player.WallGrapState);
        }
        else
        {
            player.SetVelocityY(playerData.WallClimbVelocity);
        }
    }
}
