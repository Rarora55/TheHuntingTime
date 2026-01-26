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
        
        // 1. First change to IdleState to stop any death logic
        if (player.StateMachine != null && player.IdleState != null)
        {
            player.StateMachine.ChangeState(player.IdleState);
            Debug.Log($"<color=cyan>[RESPAWN HANDLER] Changed to IdleState. Current: {player.StateMachine.CurrentState?.GetType().Name}</color>");
        }
        
        // 2. Reset animator AFTER changing state
        if (player.anim != null)
        {
            player.anim.SetBool("death", false);
            Debug.Log("<color=cyan>[RESPAWN HANDLER] Animator 'death' parameter reset to false</color>");
        }
        
        // 3. Clear death data
        deathData.ClearDeathState();
        Debug.Log($"<color=cyan>[RESPAWN HANDLER] DeathData cleared. IsDead after: {deathData.IsDead}</color>");
        
        // 4. Reset health
        if (healthController != null)
        {
            healthController.ResetHealth();
            Debug.Log($"<color=cyan>[RESPAWN HANDLER] Health reset to: {healthController.CurrentHealth}</color>");
        }
        
        // 5. Teleport to safe position
        player.transform.position = deathData.LastSafePosition;
        Debug.Log($"<color=cyan>[RESPAWN HANDLER] Teleported to: {deathData.LastSafePosition}</color>");
        
        // 6. Re-enable input LAST
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
}
