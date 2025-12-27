using UnityEngine;

public class PlayerFireState : PlayerAbilityState
{
    private PlayerWeaponController weaponController;
    private bool hasFired;
    private float fireTime;
    private const float MIN_FIRE_DURATION = 0.2f;
    
    public PlayerFireState(Player player, PlayerStateMachine stateMachine, PlayerData playerData, string animBoolName) : base(player, stateMachine, playerData, animBoolName)
    {
    }

    public override void Enter()
    {
        base.Enter();
        
        weaponController = player.GetComponent<PlayerWeaponController>();
        hasFired = false;
        fireTime = Time.time;
        
        player.anim.SetBool("aim", true);
        
        if (weaponController == null || weaponController.ActiveWeapon == null)
        {
            isAbilityDone = true;
            player.InputHandler.FireEnded();
            return;
        }
        
        PerformShot();
        player.InputHandler.FireEnded();
    }

    public override void Exit()
    {
        base.Exit();
    }

    public override void LogicUpdate()
    {
        base.LogicUpdate();
        
        if (hasFired && Time.time >= fireTime + MIN_FIRE_DURATION)
        {
            if (player.InputHandler.AimInput)
            {
                stateMachine.ChangeState(player.AimState);
            }
            else
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
            }
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
