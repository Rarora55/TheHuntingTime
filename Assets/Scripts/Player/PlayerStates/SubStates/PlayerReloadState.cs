using UnityEngine;

public class PlayerReloadState : WeaponAbilityState
{
    private bool hasReloaded;
    private float reloadStartTime;
    private float reloadDuration = 1.2f;
    
    public PlayerReloadState(Player player, PlayerStateMachine stateMachine, PlayerData playerData, string animBoolName) 
        : base(player, stateMachine, playerData, animBoolName)
    {
    }

    protected override void OnInvalidWeapon()
    {
        player.InputHandler.ReloadEnded();
        stateMachine.ChangeState(player.AimState);
    }

    protected override void OnWeaponStateEnter()
    {
        hasReloaded = false;
        reloadStartTime = Time.time;
        
        if (!weaponController.CanReload())
        {
            player.InputHandler.ReloadEnded();
            stateMachine.ChangeState(player.AimState);
            return;
        }
        
        PerformReload();
        player.InputHandler.ReloadEnded();
    }

    public override void LogicUpdate()
    {
        base.LogicUpdate();
        
        if (!hasReloaded)
            return;
        
        float timeSinceReload = Time.time - reloadStartTime;
        
        if (timeSinceReload >= reloadDuration)
        {
            if (player.InputHandler.AimInput)
            {
                stateMachine.ChangeState(player.AimState);
                return;
            }
            
            isAbilityDone = true;
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
