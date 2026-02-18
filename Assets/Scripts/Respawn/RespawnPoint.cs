using UnityEngine;
using UnityEngine.InputSystem;
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
        [SerializeField] private bool requiresInput = true;
        
        [Header("Visual Feedback")]
        [SerializeField] private Color gizmoColor = Color.green;
        [SerializeField] private float gizmoRadius = 0.5f;
        [SerializeField] private bool showLabel = true;
        [SerializeField] private string interactionPrompt = "[E] Guardar checkpoint";
        
        [Header("Advanced Settings")]
        [SerializeField] private bool oneTimeUse = false;
        
        private bool hasBeenUsed = false;
        private Collider2D triggerCollider;
        private global::Player playerInZone;
        private PlayerInput playerInput;
        private InputAction interactAction;

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

        private void Update()
        {
            if (requiresInput && playerInZone != null && !hasBeenUsed)
            {
                if (interactAction == null)
                {
                    SetupInputAction();
                }
                
                if (interactAction != null)
                {
                    if (interactAction.WasPressedThisFrame())
                    {
                        Debug.Log($"<color=green>[RESPAWN POINT] ✓ Interact button pressed! Activating respawn...</color>");
                        ActivateRespawn(playerInZone);
                    }
                }
                else
                {
                    Debug.LogWarning($"[RESPAWN POINT] interactAction is NULL! Cannot detect input.");
                }
            }
        }

        private void SetupInputAction()
        {
            if (playerInZone != null)
            {
                playerInput = playerInZone.GetComponent<PlayerInput>();
                if (playerInput != null)
                {
                    Debug.Log($"<color=cyan>[RESPAWN POINT] PlayerInput found on {playerInZone.name}</color>");
                    interactAction = playerInput.actions.FindAction("Interact");
                    if (interactAction == null)
                    {
                        Debug.LogError($"<color=red>[RESPAWN POINT] 'Interact' action NOT FOUND in Player Input Actions!</color>");
                        Debug.LogError($"<color=red>Available actions: {string.Join(", ", playerInput.actions)}</color>");
                    }
                    else
                    {
                        Debug.Log($"<color=green>[RESPAWN POINT] ✓ Interact action found and ready!</color>");
                    }
                }
                else
                {
                    Debug.LogError($"<color=red>[RESPAWN POINT] PlayerInput component NOT FOUND on {playerInZone.name}!</color>");
                }
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
                    
                    if (!requiresInput && !hasBeenUsed)
                    {
                        Debug.Log($"<color=cyan>[RESPAWN POINT] Auto-activating (requiresInput=false)</color>");
                        ActivateRespawn(player);
                    }
                    else if (requiresInput && !hasBeenUsed)
                    {
                        Debug.Log($"<color=yellow>[RESPAWN POINT] Player in zone. {interactionPrompt} | requiresInput={requiresInput}</color>");
                    }
                    else if (hasBeenUsed)
                    {
                        Debug.Log($"<color=gray>[RESPAWN POINT] Already used (oneTimeUse)</color>");
                    }
                }
            }
        }

        private void OnTriggerExit2D(Collider2D collision)
        {
            if (collision.CompareTag("Player"))
            {
                playerInZone = null;
                interactAction = null;
                playerInput = null;
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
            
            if (onRespawnActivated != null)
            {
                onRespawnActivated.Raise(respawnPosition, respawnID);
            }
            
            hasBeenUsed = true;
            
            Debug.Log($"<color=green>[RESPAWN POINT] ✓ Checkpoint saved: {respawnID} at {respawnPosition}</color>");
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