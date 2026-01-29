using UnityEngine;
using UnityEngine.SceneManagement;
using TheHunt.Events;

public class PlayerRespawnHandler : MonoBehaviour
{
    [Header("SO References")]
    [SerializeField] private DeathData deathData;
    [SerializeField] private PlayerRespawnEvent onPlayerRespawnEvent;
    
    [Header("Component References")]
    [SerializeField] private Player player;
    [SerializeField] private HealthController healthController;
    [SerializeField] private Transform spawnDeathTransform;
    
    [Header("Physics Settings")]
    [SerializeField] private float defaultGravityScale = 3f;
    
    void Start()
    {
        if (player == null)
            player = GetComponent<Player>();
            
        if (healthController == null)
            healthController = GetComponent<HealthController>();
            
        if (onPlayerRespawnEvent != null)
        {
            onPlayerRespawnEvent.AddListener(HandleRespawn);
        }
    }

    void OnDestroy()
    {
        if (onPlayerRespawnEvent != null)
        {
            onPlayerRespawnEvent.RemoveListener(HandleRespawn);
        }
    }

    void HandleRespawn()
    {
        if (deathData == null || player == null)
        {
            Debug.LogError("<color=red>[RESPAWN HANDLER] Missing references!</color>");
            return;
        }
        
        Debug.Log($"<color=cyan>[RESPAWN HANDLER] Starting respawn. IsDead before: {deathData.IsDead}</color>");
        
        // 1. Update SpawnDeath position to current death position
        if (spawnDeathTransform != null)
        {
            spawnDeathTransform.position = player.transform.position;
            Debug.Log($"<color=cyan>[RESPAWN HANDLER] SpawnDeath updated to death position: {spawnDeathTransform.position}</color>");
        }
        
        // 2. Change to IdleState to stop any death logic
        if (player.StateMachine != null && player.IdleState != null)
        {
            player.StateMachine.ChangeState(player.IdleState);
            Debug.Log($"<color=cyan>[RESPAWN HANDLER] Changed to IdleState. Current: {player.StateMachine.CurrentState?.GetType().Name}</color>");
        }
        
        // 3. Reset animator AFTER changing state
        if (player.anim != null)
        {
            player.anim.SetBool("death", false);
            Debug.Log("<color=cyan>[RESPAWN HANDLER] Animator 'death' parameter reset to false</color>");
        }
        
        // 4. Clear death data
        deathData.ClearDeathState();
        Debug.Log($"<color=cyan>[RESPAWN HANDLER] DeathData cleared. IsDead after: {deathData.IsDead}</color>");
        
        // 5. Reset health
        if (healthController != null)
        {
            healthController.ResetHealth();
            Debug.Log($"<color=cyan>[RESPAWN HANDLER] Health reset to: {healthController.CurrentHealth}</color>");
        }
        
        // 6. Player stays at death position (no teleport needed - SpawnDeath already marks the spot)
        Debug.Log($"<color=cyan>[RESPAWN HANDLER] Player remains at death position: {player.transform.position}</color>");
        
        // 7. Reset physics properties
        if (player.RB != null)
        {
            player.RB.gravityScale = defaultGravityScale;
            player.SetVelocityZero();
            Debug.Log($"<color=cyan>[RESPAWN HANDLER] Physics reset - GravityScale: {defaultGravityScale}, Velocity: Zero</color>");
        }
        
        // 8. Re-enable input LAST
        if (player.InputHandler != null)
        {
            player.InputHandler.enabled = true;
            Debug.Log("<color=cyan>[RESPAWN HANDLER] Input enabled</color>");
        }
        
        Debug.Log("<color=green>[RESPAWN HANDLER] ✅ Player respawned successfully!</color>");
    }

    public void RestartLevel()
    {
        Time.timeScale = 1f;
        SceneManager.LoadScene(SceneManager.GetActiveScene().buildIndex);
    }

    public Vector3 GetDeathPosition()
    {
        return spawnDeathTransform != null ? spawnDeathTransform.position : Vector3.zero;
    }

    public Transform GetSpawnDeathTransform()
    {
        return spawnDeathTransform;
    }
}
