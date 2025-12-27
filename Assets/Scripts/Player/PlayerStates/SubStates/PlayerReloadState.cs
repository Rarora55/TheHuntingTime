using UnityEngine;

public class PlayerReloadState : PlayerAbilityState
{
    private PlayerWeaponController weaponController;
    private bool hasReloaded;
    private float reloadStartTime;
    private float reloadDuration = 1.2f;
    
    public PlayerReloadState(Player player, PlayerStateMachine stateMachine, PlayerData playerData, string animBoolName) : base(player, stateMachine, playerData, animBoolName)
    {
    }

    public override void Enter()
    {
        base.Enter();
        
        weaponController = player.GetComponent<PlayerWeaponController>();
        hasReloaded = false;
        reloadStartTime = Time.time;
        
        player.anim.SetBool("aim", true);
        
        if (weaponController == null || weaponController.ActiveWeapon == null)
        {
            player.InputHandler.ReloadEnded();
            stateMachine.ChangeState(player.AimState);
            return;
        }
        
        if (!weaponController.CanReload())
        {
            player.InputHandler.ReloadEnded();
            stateMachine.ChangeState(player.AimState);
            return;
        }
        
        PerformReload();
        player.InputHandler.ReloadEnded();
    }

    public override void Exit()
    {
        base.Exit();
    }

    public override void LogicUpdate()
    {
        base.LogicUpdate();
        
        if (!player.InputHandler.AimInput)
        {
            player.anim.SetBool("aim", false);
            
            bool isGrounded = player.CheckIsGrounded();
            if (isGrounded && player.CurrentVelocity.y < 0.01f)
            {
                stateMachine.ChangeState(player.IdleState);
            }
            else
            {
                stateMachine.ChangeState(player.AirState);
            }
            return;
        }
        
        if (hasReloaded && Time.time >= reloadStartTime + reloadDuration)
        {
            stateMachine.ChangeState(player.AimState);
        }
    }

    public override void PhysicsUpdate()
    {
        base.PhysicsUpdate();
    }
    
    public override void DoChecks()
    {
        base.DoChecks();
    }
    
    private void PerformReload()
    {
        if (weaponController == null)
            return;
        
        weaponController.Reload();
        hasReloaded = true;
    }
}
