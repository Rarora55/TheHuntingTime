using UnityEngine;
using TheHunt.Events;

namespace TheHunt.Respawn
{
    [RequireComponent(typeof(Collider2D))]
    public class RespawnPoint : MonoBehaviour
    {
        [Header("Events")]
        [SerializeField] private RespawnActivatedEvent onRespawnActivated;

        [Header("Respawn Settings")]
        [SerializeField] private string respawnID = "Respawn_01";
        [SerializeField] private bool autoActivateOnEnter = true;
        
        [Header("Visual Feedback")]
        [SerializeField] private Color gizmoColor = Color.green;
        [SerializeField] private float gizmoRadius = 0.5f;
        [SerializeField] private bool showLabel = true;
        
        [Header("Advanced Settings")]
        [SerializeField] private bool oneTimeUse = false;
        
        private bool hasBeenUsed = false;
        private Collider2D triggerCollider;
        private global::Player playerInZone;

        private void Awake()
        {
            triggerCollider = GetComponent<Collider2D>();
            triggerCollider.isTrigger = true;
            
            if (string.IsNullOrEmpty(respawnID))
            {
                respawnID = $"Respawn_{gameObject.GetInstanceID()}";
                Debug.LogWarning($"[RESPAWN POINT] No ID assigned. Auto-generated: {respawnID}");
            }
            
            if (onRespawnActivated == null)
            {
                Debug.LogError($"[RESPAWN POINT] RespawnActivatedEvent is not assigned on '{gameObject.name}'!");
            }
        }

        private void OnTriggerEnter2D(Collider2D collision)
        {
            if (collision.CompareTag("Player"))
            {
                global::Player player = collision.GetComponent<global::Player>();
                if (player != null)
                {
                    playerInZone = player;
                    
                    if (autoActivateOnEnter && !hasBeenUsed)
                    {
                        ActivateRespawn(player);
                    }
                }
            }
        }

        private void OnTriggerExit2D(Collider2D collision)
        {
            if (collision.CompareTag("Player"))
            {
                playerInZone = null;
            }
        }

        private void ActivateRespawn(global::Player player)
        {
            if (oneTimeUse && hasBeenUsed)
            {
                Debug.LogWarning($"[RESPAWN POINT] {respawnID} has already been used (one-time only).");
                return;
            }

            Vector3 respawnPosition = transform.position;
            player.transform.position = respawnPosition;
            
            player.SetVelocityX(0);
            player.SetVelocityY(0);
            
            if (onRespawnActivated != null)
            {
                onRespawnActivated.Raise(respawnPosition, respawnID);
            }
            
            hasBeenUsed = true;
            
            Debug.Log($"<color=green>[RESPAWN POINT] ✓ Activated {respawnID} at {respawnPosition}</color>");
        }

        public void ManualActivate()
        {
            if (playerInZone != null)
            {
                ActivateRespawn(playerInZone);
            }
            else
            {
                Debug.LogWarning("[RESPAWN POINT] No player in zone for manual activation.");
            }
        }

        public void ResetUsage()
        {
            hasBeenUsed = false;
        }

        private void OnDrawGizmos()
        {
            Gizmos.color = gizmoColor;
            Gizmos.DrawWireSphere(transform.position, gizmoRadius);
            
            Gizmos.color = new Color(gizmoColor.r, gizmoColor.g, gizmoColor.b, 0.2f);
            Gizmos.DrawSphere(transform.position, gizmoRadius);
            
            if (showLabel)
            {
                #if UNITY_EDITOR
                UnityEditor.Handles.Label(
                    transform.position + Vector3.up * (gizmoRadius + 0.5f),
                    respawnID,
                    new GUIStyle()
                    {
                        normal = new GUIStyleState() { textColor = gizmoColor },
                        fontSize = 12,
                        fontStyle = FontStyle.Bold
                    }
                );
                #endif
            }
        }
    }
}