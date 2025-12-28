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
    private bool GrabInput;
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
    }
    public override void Enter()
    {
        base.Enter();
        
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
        GrabInput = player.InputHandler.GrabInput;
        AimInput = player.InputHandler.AimInput;

        if (AimInput && CanAim())
        {
            stateMachine.ChangeState(player.AimState);
        }
        else if (JumpInput && isGrounded)
        {
            player.InputHandler.JumpEnded();
            stateMachine.ChangeState(player.JumpState);
        }
        else if (!isGrounded)
        {
            stateMachine.ChangeState(player.AirState);
        }
        else if (isTouchingWall && GrabInput && !isTouchingCeiling)
        {
            stateMachine.ChangeState(player.WallGrapState);
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
