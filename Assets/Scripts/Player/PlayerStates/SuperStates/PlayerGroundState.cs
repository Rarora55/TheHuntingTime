using UnityEngine;
using System.Collections;
using System.Collections.Generic;

public class PlayerGroundState : PlayerState
{
    protected int xInput;
    protected int yInput;

    protected bool isTouchingCeiling;
    protected bool isGrounded;

    private bool JumpInput;
    private bool isTouchingWall;
    private bool isTouchingLedge;
    protected bool grabInput;
    private bool AimInput;
    
    private PlayerWeaponController weaponController;

    public PlayerGroundState(Player player, PlayerStateMachine stateMachine, PlayerData playerData, string animBoolName) : base(player, stateMachine, playerData, animBoolName)
    {

    }

    public override void DoChecks()
    {
        base.DoChecks();

        bool wasGrounded = isGrounded;
        isGrounded = player.CheckIsGrounded();
        isTouchingWall = player.CheckIfTouchingWall();
        isTouchingLedge = player.CheckTouchingLedge();
        isTouchingCeiling = player.CheckForCeiling();
    }
    public override void Enter()
    {
        base.Enter();
        
        player.RB.gravityScale = 1f;
        
        if (weaponController == null)
        {
            weaponController = player.WeaponController;
        }
    }

    public override void Exit()
    {
        base.Exit();
    }

    public override void LogicUpdate()
    {
        base.LogicUpdate();

        xInput = player.InputHandler.NormInputX;
        yInput = player.InputHandler.NormInputY;
        JumpInput = player.InputHandler.JumpInput;
        grabInput = player.InputHandler.GrabInput;
        AimInput = player.InputHandler.AimInput;
        bool runInput = player.InputHandler.RunInput;

        // DESHABILITADO - Sistema de escalada con input grab anulado
        // if (player.IsOnLadder() && grabInput && (yInput == 1 || yInput == -1))
        // {
        //     stateMachine.ChangeState(player.LadderClimbState);
        // }
        
        if (AimInput && CanAim())
        {
            stateMachine.ChangeState(player.AimState);
        }
        else if (JumpInput && isGrounded)
        {
            player.InputHandler.JumpEnded();
            stateMachine.ChangeState(player.JumpState);
        }
        else if (isTouchingWall && grabInput && !isTouchingCeiling && !runInput)  // ✅ NO agarrar ledge si está corriendo
        {
            bool hasClimbable = player.CanClimbHere();
            bool isValidLedge = player.Collision.IsValidLedge(0.2f);
            
            if (isValidLedge)
            {
                if (hasClimbable)
                {
                    Debug.Log("<color=green>[GROUND] Climbable + Ledge válido - Cambiando a WallGrapState</color>");
                    stateMachine.ChangeState(player.WallGrapState);
                }
                else
                {
                    Debug.Log("<color=cyan>[GROUND] Ledge válido (Ground layer) - Cambiando a WallLedgeState</color>");
                    stateMachine.ChangeState(player.WallLedgeState);
                }
            }
            else
            {
                Debug.Log($"<color=yellow>[GROUND] Grab bloqueado - No hay ledge válido</color>");
            }
        }
        else if (!isGrounded)
        {
            stateMachine.ChangeState(player.AirState);
        }
    }

    public override void PhysicsUpdate()
    {
        base.PhysicsUpdate();
    }
    
    private bool CanAim()
    {
        if (weaponController == null)
            return false;
            
        return weaponController.ActiveWeapon != null;
    }
}
