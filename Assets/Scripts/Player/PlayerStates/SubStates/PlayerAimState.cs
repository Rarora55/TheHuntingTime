using UnityEngine;

public class PlayerAimState : WeaponAbilityState
{
    private bool isGrounded;
    
    public PlayerAimState(Player player, PlayerStateMachine stateMachine, PlayerData playerData, string animBoolName) 
        : base(player, stateMachine, playerData, animBoolName)
    {
    }

    protected override void OnInvalidWeapon()
    {
        isAbilityDone = true;
    }

    public override void LogicUpdate()
    {
        base.LogicUpdate();
        
        if (!HasValidWeapon())
        {
            isAbilityDone = true;
            return;
        }
        
        if (player.InputHandler.FireInput)
        {
            if (weaponController.CanShoot())
            {
                player.anim.SetBool(animBoolName, false);
                player.InputHandler.FireEnded();
                stateMachine.ChangeState(player.FireState);
                return;
            }
            else
            {
                player.InputHandler.FireEnded();
                weaponController.Shoot();
            }
        }
        
        if (player.InputHandler.ReloadInput)
        {
            if (weaponController.CanReload())
            {
                player.anim.SetBool(animBoolName, false);
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
