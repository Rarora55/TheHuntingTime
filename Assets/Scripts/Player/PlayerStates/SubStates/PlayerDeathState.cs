using UnityEngine;
using TheHunt.Events;

public class PlayerDeathState : PlayerState
{
    private DeathData deathData;
    private bool isDeathByFall;
    private float deathTimer;
    private bool deathScreenShown = false;

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
        DoChecks();
        startTime = Time.time;
        isAnimationFinish = false;
        
        Debug.Log($"<color=magenta>[DEATH STATE] Enter() - Iniciando muerte</color>");
        
        player.SetVelocityZero();
        
        DisableAllAnimatorParametersExceptDeath();
        
        player.anim.SetBool("death", true);
        Debug.Log("<color=cyan>[DEATH STATE] Parámetro 'death' establecido a TRUE</color>");
        
        if (deathData != null)
        {
            DeathType type = isDeathByFall ? DeathType.Fall : DeathType.Normal;
            deathTimer = deathData.GetDeathDuration(type);
            Debug.Log($"<color=yellow>[DEATH STATE] Duración configurada: {deathTimer}s para tipo: {type}</color>");
        }
        else
        {
            deathTimer = 1.5f;
            Debug.LogWarning("<color=orange>[DEATH STATE] DeathData no asignado, usando duración predeterminada de 1.5s</color>");
        }
        
        deathScreenShown = false;
        
        AnimatorStateInfo stateInfo = player.anim.GetCurrentAnimatorStateInfo(0);
        Debug.Log($"<color=red>[DEATH STATE] Muerte iniciada - Tipo: {(isDeathByFall ? "Caída" : "Normal")}, Duración: {deathTimer}s</color>");
        Debug.Log($"<color=yellow>[DEATH STATE] Estado del Animator: {stateInfo.shortNameHash}, IsName 'deathAnim': {stateInfo.IsName("deathAnim")}</color>");
    }
    
    void DisableAllAnimatorParametersExceptDeath()
    {
        foreach (AnimatorControllerParameter param in player.anim.parameters)
        {
            if (param.type == AnimatorControllerParameterType.Bool && param.name != "death")
            {
                player.anim.SetBool(param.name, false);
            }
        }
        
        Debug.Log("<color=cyan>[DEATH STATE] Todos los parámetros del Animator desactivados excepto 'death'</color>");
    }
    
    public override void Exit()
    {
        Debug.Log($"<color=yellow>[DEATH STATE] Exit() - Saliendo del estado de muerte</color>");
        
        player.anim.SetBool("death", false);
        
        isDeathByFall = false;
        deathTimer = 0f;
        deathScreenShown = false;
    }

    public override void LogicUpdate()
    {
        base.LogicUpdate();
        
        if (deathTimer > 0f && !deathScreenShown)
        {
            deathTimer -= Time.deltaTime;
            
            if (deathTimer <= 0f)
            {
                AnimatorStateInfo currentState = player.anim.GetCurrentAnimatorStateInfo(0);
                Debug.Log("<color=green>[DEATH STATE] ¡Timer expirado! Mostrando pantalla de muerte...</color>");
                Debug.Log($"<color=cyan>[DEATH STATE] Estado actual del Animator: IsName('deathAnim'): {currentState.IsName("deathAnim")}, NormalizedTime: {currentState.normalizedTime:F4}</color>");
                
                PlayerDeathHandler deathHandler = player.GetComponent<PlayerDeathHandler>();
                if (deathHandler != null)
                {
                    deathHandler.OnDeathAnimationComplete();
                    deathScreenShown = true;
                }
                else
                {
                    Debug.LogError("<color=red>[DEATH STATE] ¡PlayerDeathHandler no encontrado!</color>");
                }
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
