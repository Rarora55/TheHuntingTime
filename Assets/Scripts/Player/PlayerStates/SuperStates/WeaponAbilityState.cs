using UnityEngine;

public abstract class WeaponAbilityState : PlayerAbilityState
{
    protected PlayerWeaponController weaponController;
    
    protected WeaponAbilityState(Player player, PlayerStateMachine stateMachine, PlayerData playerData, string animBoolName) 
        : base(player, stateMachine, playerData, animBoolName)
    {
    }
    
    public override void Enter()
    {
        base.Enter();
        
        weaponController = player.WeaponController;
        
        if (!HasValidWeapon())
        {
            OnInvalidWeapon();
            return;
        }
        
        player.anim.SetBool("aim", true);
        OnWeaponStateEnter();
    }
    
    public override void Exit()
    {
        base.Exit();
        player.anim.SetBool("aim", false);
    }
    
    protected bool HasValidWeapon()
    {
        return weaponController != null && weaponController.ActiveWeapon != null;
    }
    
    protected abstract void OnInvalidWeapon();
    
    protected virtual void OnWeaponStateEnter() { }
}
