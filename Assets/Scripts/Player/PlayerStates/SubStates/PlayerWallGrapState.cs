using UnityEngine;

public class PlayerWallGrapState : PlayerTouchingWallState
{
    private Vector2 holdPosition;
    
    public PlayerWallGrapState(Player player, PlayerStateMachine stateMachine, PlayerData playerData, string animBoolName) : base(player, stateMachine, playerData, animBoolName)
    {
    }

    public override void DoChecks()
    {
        base.DoChecks();
    }

    public override void Enter()
    {
        base.Enter();
        holdPosition = player.transform.position;
        HoldPosition();
        Debug.Log("<color=orange>[WALLGRAB] Enter - Agarrado a pared</color>");
    }

    public override void LogicUpdate()
    {
        xInput = player.InputHandler.NormInputX;
        yInput = player.InputHandler.NormInputY;
        grabInput = player.InputHandler.GrabInput;
        
        HoldPosition();
        
        Debug.Log($"[WALLGRAB] Ground:{isGrounded} | Wall:{isTouchingWall} | Ledge:{isTouchingLedge} | xIn:{xInput} | yIn:{yInput} | Grab:{grabInput} | FacingRight:{player.FacingRight}");

        if (!isTouchingWall)
        {
            Debug.Log("[WALLGRAB] -> AirState (no toca pared)");
            stateMachine.ChangeState(player.AirState);
        }
        else if (yInput > 0)
        {
            Debug.Log("[WALLGRAB] -> WallClimbState (subiendo)");
            stateMachine.ChangeState(player.WallClimbState);
        }
        else if (yInput < 0 && grabInput)
        {
            Debug.Log("[WALLGRAB] -> WallSlicedState (bajando con Grab)");
            stateMachine.ChangeState(player.WallSlicedState);
        }
        else if (!grabInput)
        {
            if (xInput != 0 && xInput == player.FacingRight)
            {
                Debug.Log("[WALLGRAB] -> WallSlicedState (soltó Grab pero presiona hacia pared)");
                stateMachine.ChangeState(player.WallSlicedState);
            }
            else
            {
                Debug.Log("[WALLGRAB] -> AirState (soltó Grab sin presionar hacia pared)");
                stateMachine.ChangeState(player.AirState);
            }
        }
    }

    private void HoldPosition()
    {
        player.transform.position = holdPosition;
        player.SetVelocityX(0);
        player.SetVelocityY(0);
    }
}
