using UnityEngine;

public class PlayerFireState : WeaponAbilityState
{
    private bool hasFired;
    private float fireTime;
    private float animationDuration;
    
    public PlayerFireState(Player player, PlayerStateMachine stateMachine, PlayerData playerData, string animBoolName) 
        : base(player, stateMachine, playerData, animBoolName)
    {
    }

    protected override void OnInvalidWeapon()
    {
        isAbilityDone = true;
        player.InputHandler.FireEnded();
    }

    protected override void OnWeaponStateEnter()
    {
        hasFired = false;
        fireTime = Time.time;
        
        if (!weaponController.CanShoot())
        {
            isAbilityDone = true;
            player.InputHandler.FireEnded();
            return;
        }
        
        AnimatorStateInfo stateInfo = player.anim.GetCurrentAnimatorStateInfo(0);
        animationDuration = stateInfo.length;
        
        if (animationDuration <= 0)
            animationDuration = 0.3f;
        
        PerformShot();
        player.InputHandler.FireEnded();
    }

    public override void LogicUpdate()
    {
        base.LogicUpdate();
        
        if (!hasFired)
        {
            if (Time.time - fireTime > 0.5f)
            {
                isAbilityDone = true;
            }
            return;
        }
        
        float timeSinceFire = Time.time - fireTime;
        
        if (timeSinceFire >= animationDuration * 0.7f)
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
    
    private void PerformShot()
    {
        if (weaponController == null)
            return;
        
        weaponController.Shoot();
        hasFired = true;
    }
}
