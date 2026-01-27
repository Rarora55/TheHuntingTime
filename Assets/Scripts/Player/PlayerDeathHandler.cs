using UnityEngine;
using TheHunt.Events;

public class PlayerDeathHandler : MonoBehaviour
{
    [Header("SO References")]
    [SerializeField] private DeathData deathData;
    [SerializeField] private PlayerDeathEvent onPlayerDeathEvent;
    [SerializeField] private ShowDeathScreenEvent showDeathScreenEvent;
    
    [Header("Component References")]
    [SerializeField] private Player player;
    [SerializeField] private HealthController healthController;
    
    private DeathType currentDeathType;

    void Start()
    {
        if (player == null)
            player = GetComponent<Player>();
            
        if (healthController == null)
            healthController = GetComponent<HealthController>();
            
        if (healthController != null)
        {
            healthController.OnDeath += HandleDeath;
        }
        
        if (deathData != null && player != null)
        {
            deathData.SetLastSafePosition(player.transform.position);
        }
    }

    void OnDestroy()
    {
        if (healthController != null)
        {
            healthController.OnDeath -= HandleDeath;
        }
    }

    void Update()
    {
        if (player != null && deathData != null && !deathData.IsDead)
        {
            if (player.StateMachine.CurrentState is PlayerIdleState or PlayerMoveState)
            {
                deathData.SetLastSafePosition(player.transform.position);
            }
        }
    }

    void HandleDeath()
    {
        if (deathData != null && deathData.IsDead)
        {
            Debug.LogWarning("<color=orange>[DEATH HANDLER] El jugador ya está muerto, ignorando evento de muerte</color>");
            return;
        }
        
        Debug.Log("<color=red>[DEATH HANDLER] ¡El jugador está muriendo...!</color>");
        
        Vector3 deathPosition = player != null ? player.transform.position : Vector3.zero;
        
        if (deathData != null)
        {
            deathData.SetDeathState(currentDeathType, deathPosition);
            Debug.Log($"<color=red>[DEATH HANDLER] DeathData.IsDead establecido a TRUE - Tipo: {currentDeathType}</color>");
        }
        
        if (player != null && player.InputHandler != null)
        {
            player.InputHandler.enabled = false;
            Debug.Log("<color=red>[DEATH HANDLER] Input desactivado</color>");
        }
        
        if (onPlayerDeathEvent != null)
        {
            onPlayerDeathEvent.Raise(currentDeathType, deathPosition);
        }
        
        if (player != null && player.StateMachine != null && player.DeathState != null)
        {
            player.DeathState.SetDeathByFall(currentDeathType == DeathType.Fall);
            player.StateMachine.ChangeState(player.DeathState);
            Debug.Log("<color=red>[DEATH HANDLER] Cambio a DeathState completado</color>");
        }
    }

    public void OnDeathAnimationComplete()
    {
        Debug.Log("<color=magenta>[DEATH HANDLER] Animación de muerte completada, mostrando pantalla de muerte</color>");
        
        if (showDeathScreenEvent != null && deathData != null)
        {
            showDeathScreenEvent.Raise(deathData.CurrentDeathType);
            Debug.Log($"<color=green>[DEATH HANDLER] Evento ShowDeathScreen disparado - Tipo: {deathData.CurrentDeathType}</color>");
        }
        else
        {
            if (showDeathScreenEvent == null)
                Debug.LogError("<color=red>[DEATH HANDLER] ShowDeathScreenEvent no está asignado!</color>");
            if (deathData == null)
                Debug.LogError("<color=red>[DEATH HANDLER] DeathData no está asignado!</color>");
        }
    }

    public void CheckForFallDeath(float fallHeight)
    {
        if (deathData == null || healthController == null)
            return;
            
        if (fallHeight >= deathData.FallDeathThreshold)
        {
            currentDeathType = DeathType.Fall;
            healthController.TakeDamage(healthController.CurrentHealth);
        }
    }

    public void SetDeathType(DeathType type)
    {
        currentDeathType = type;
    }

    public DeathData GetDeathData()
    {
        return deathData;
    }
}
