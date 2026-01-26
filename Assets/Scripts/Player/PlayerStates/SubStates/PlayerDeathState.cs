using UnityEngine;
using TheHunt.Events;

public class PlayerDeathState : PlayerState
{
    private DeathData deathData;
    private bool isDeathByFall;
    private float deathTimer;

    public PlayerDeathState(Player player, PlayerStateMachine stateMachine, PlayerData playerData, string animBoolName) 
        : base(player, stateMachine, playerData, animBoolName)
    {
    }

    public void SetDeathData(DeathData data)
    {
        deathData = data;
    }

    public override void Enter()
    {
        base.Enter();
        
        Debug.Log($"<color=magenta>[DEATH STATE] Enter() called. Animator 'death' parameter is now: {player.anim.GetBool("death")}</color>");
        
        player.SetVelocityZero();
        
        if (deathData != null)
        {
            DeathType type = isDeathByFall ? DeathType.Fall : DeathType.Normal;
            deathTimer = deathData.GetDeathDuration(type);
        }
        else
        {
            deathTimer = isDeathByFall ? 1f : 2f;
        }
        
        Debug.Log($"<color=red>[DEATH STATE] Player has died. Fall death: {isDeathByFall}, Duration: {deathTimer}s</color>");
    }

    public override void Exit()
    {
        base.Exit();
        
        Debug.Log($"<color=yellow>[DEATH STATE] Exit() called. Resetting 'death' parameter to FALSE</color>");
        
        isDeathByFall = false;
        deathTimer = 0f;
    }

    public override void LogicUpdate()
    {
        base.LogicUpdate();
        
        if (deathTimer > 0f && deathTimer != float.MaxValue)
        {
            deathTimer -= Time.unscaledDeltaTime;
            
            if (deathTimer <= 0f)
            {
                PlayerDeathHandler deathHandler = player.GetComponent<PlayerDeathHandler>();
                if (deathHandler != null)
                {
                    deathHandler.OnDeathAnimationComplete();
                }
                
                deathTimer = float.MaxValue;
            }
        }
    }

    public override void PhysicsUpdate()
    {
        base.PhysicsUpdate();
        player.SetVelocityZero();
    }

    public override void DoChecks()
    {
        // DO NOTHING - Block all input checks during death
    }

    public void SetDeathByFall(bool isFallDeath)
    {
        isDeathByFall = isFallDeath;
    }
}
