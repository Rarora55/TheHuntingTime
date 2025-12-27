using UnityEngine;

public class PlayerAimState : PlayerAbilityState
{
    private PlayerWeaponController weaponController;
    private bool isGrounded;
    
    public PlayerAimState(Player player, PlayerStateMachine stateMachine, PlayerData playerData, string animBoolName) : base(player, stateMachine, playerData, animBoolName)
    {
    }

    public override void Enter()
    {
        base.Enter();
        
        weaponController = player.GetComponent<PlayerWeaponController>();
        
        if (weaponController == null || weaponController.ActiveWeapon == null)
        {
            isAbilityDone = true;
            return;
        }
    }

    public override void Exit()
    {
        base.Exit();
    }

    public override void LogicUpdate()
    {
        base.LogicUpdate();
        
        if (weaponController == null || weaponController.ActiveWeapon == null)
        {
            isAbilityDone = true;
            return;
        }
        
        if (player.InputHandler.FireInput)
        {
            stateMachine.ChangeState(player.FireState);
            return;
        }
        
        if (player.InputHandler.ReloadInput)
        {
            if (weaponController.CanReload())
            {
                stateMachine.ChangeState(player.ReloadState);
                return;
            }
        }
        
        if (!player.InputHandler.AimInput)
        {
            isAbilityDone = true;
            return;
        }
    }

    public override void PhysicsUpdate()
    {
        base.PhysicsUpdate();
    }
    
    public override void DoChecks()
    {
        base.DoChecks();
        isGrounded = player.CheckIsGrounded();
    }
}
