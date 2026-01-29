using UnityEngine;
using TheHunt.Events;

namespace TheHunt.Respawn
{
    public class RespawnManager : MonoBehaviour
    {
        [Header("Events")]
        [SerializeField] private RespawnActivatedEvent onRespawnActivated;
        [SerializeField] private RespawnRequestEvent onRespawnRequest;

        [Header("Runtime Data")]
        [SerializeField] private RespawnRuntimeData runtimeData;

        [Header("Settings")]
        [SerializeField] private bool logRespawnChanges = true;
        [SerializeField] private float defaultGravityScale = 3f;

        private void OnEnable()
        {
            if (onRespawnActivated != null)
            {
                onRespawnActivated.AddListener(OnRespawnActivated);
            }
            else
            {
                Debug.LogError("[RESPAWN MANAGER] RespawnActivatedEvent is not assigned!");
            }

            if (onRespawnRequest != null)
            {
                onRespawnRequest.AddListener(OnRespawnRequest);
            }
            else
            {
                Debug.LogError("[RESPAWN MANAGER] RespawnRequestEvent is not assigned!");
            }
        }

        private void OnDisable()
        {
            if (onRespawnActivated != null)
            {
                onRespawnActivated.RemoveListener(OnRespawnActivated);
            }

            if (onRespawnRequest != null)
            {
                onRespawnRequest.RemoveListener(OnRespawnRequest);
            }
        }

        private void OnRespawnActivated(Vector3 position, string respawnID)
        {
            if (runtimeData == null)
            {
                Debug.LogError("[RESPAWN MANAGER] RespawnRuntimeData is not assigned!");
                return;
            }

            runtimeData.SetRespawn(position, respawnID);

            if (logRespawnChanges)
            {
                Debug.Log($"<color=cyan>[RESPAWN MANAGER] Checkpoint activated: {respawnID} at {position}</color>");
            }
        }

        private void OnRespawnRequest(global::Player player)
        {
            if (player == null)
            {
                Debug.LogError("[RESPAWN MANAGER] Player reference is null!");
                return;
            }

            if (runtimeData == null)
            {
                Debug.LogError("[RESPAWN MANAGER] RespawnRuntimeData is not assigned!");
                return;
            }

            if (!runtimeData.HasRespawnPoint)
            {
                Debug.LogWarning("[RESPAWN MANAGER] No respawn point set. Cannot respawn player.");
                return;
            }

            // Player stays at death position - NO TELEPORT
            // player.transform.position = runtimeData.CurrentRespawnPosition;
            
            // Reset physics
            if (player.RB != null)
            {
                player.RB.gravityScale = defaultGravityScale;
            }
            
            player.SetVelocityX(0);
            player.SetVelocityY(0);

            if (logRespawnChanges)
            {
                Debug.Log($"<color=green>[RESPAWN MANAGER] ✓ Player respawned in place (no teleport)</color>");
            }
        }

        public void ResetRespawnData()
        {
            if (runtimeData != null)
            {
                runtimeData.Reset();
            }
        }
    }
}
