using UnityEngine;
using System.Collections;
using TheHunt.Events;

namespace TheHunt.Respawn
{
    [RequireComponent(typeof(global::Player))]
    public class PlayerRespawnController : MonoBehaviour
    {
        [Header("Events")]
        [SerializeField] private RespawnRequestEvent onRespawnRequest;

        [Header("Data")]
        [SerializeField] private RespawnData respawnData;
        [SerializeField] private RespawnRuntimeData runtimeData;
        
        [Header("Manual Settings (if no RespawnData)")]
        [SerializeField] private bool autoRespawnOnDeath = true;
        [SerializeField] private float respawnDelay = 2f;
        [SerializeField] private bool resetHealthOnRespawn = true;
        
        [Header("Debug")]
        [SerializeField] private bool showDebugLogs = true;
        
        private global::Player player;
        private HealthController healthController;
        private bool isRespawning = false;

        private void Awake()
        {
            player = GetComponent<global::Player>();
            healthController = GetComponent<HealthController>();
            
            if (onRespawnRequest == null)
            {
                Debug.LogError("[PLAYER RESPAWN] RespawnRequestEvent is not assigned!");
            }
        }

        private void OnEnable()
        {
            if (healthController != null)
            {
                healthController.OnDeath += HandlePlayerDeath;
            }
        }

        private void OnDisable()
        {
            if (healthController != null)
            {
                healthController.OnDeath -= HandlePlayerDeath;
            }
        }

        private void HandlePlayerDeath()
        {
            bool shouldAutoRespawn = respawnData != null ? respawnData.autoRespawnOnDeath : autoRespawnOnDeath;
            
            if (shouldAutoRespawn && !isRespawning)
            {
                float delay = respawnData != null ? respawnData.respawnDelay : respawnDelay;
                StartCoroutine(RespawnAfterDelay(delay));
            }
            else if (showDebugLogs)
            {
                Debug.Log("[PLAYER RESPAWN] Auto-respawn disabled. Player must respawn manually.");
            }
        }

        private IEnumerator RespawnAfterDelay(float delay)
        {
            isRespawning = true;
            
            if (showDebugLogs)
            {
                Debug.Log($"<color=yellow>[PLAYER RESPAWN] Respawning in {delay} seconds...</color>");
            }
            
            yield return new WaitForSeconds(delay);
            
            RespawnPlayer();
            
            isRespawning = false;
        }

        public void RespawnPlayer()
        {
            if (onRespawnRequest != null)
            {
                onRespawnRequest.Raise(player);
            }
            else
            {
                Debug.LogError("[PLAYER RESPAWN] RespawnRequestEvent is not assigned! Cannot respawn.");
                return;
            }
            
            bool shouldResetHealth = respawnData != null ? respawnData.resetHealthOnRespawn : resetHealthOnRespawn;
            
            if (shouldResetHealth && healthController != null)
            {
                float restorePercentage = respawnData != null ? respawnData.healthRestorePercentage : 1f;
                float healthToRestore = healthController.MaxHealth * restorePercentage;
                
                healthController.Heal(healthToRestore);
                
                if (showDebugLogs)
                {
                    Debug.Log($"<color=green>[PLAYER RESPAWN] ✓ Health restored to {restorePercentage * 100}%</color>");
                }
            }
            
            if (showDebugLogs)
            {
                Debug.Log("<color=green>[PLAYER RESPAWN] ✓ Player respawn requested!</color>");
            }
        }

        private void Update()
        {
            if (UnityEngine.Input.GetKeyDown(KeyCode.R))
            {
                Debug.Log("[PLAYER RESPAWN] Manual respawn triggered (R key)");
                RespawnPlayer();
            }
        }
    }
}
