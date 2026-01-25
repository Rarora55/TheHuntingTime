using UnityEngine;
using System.Collections;
using TheHunt.Interaction;

namespace TheHunt.Environment
{
    [RequireComponent(typeof(Collider2D))]
    public class ClimbSpawnPoint : InteractableObject
    {
        [Header("Spawn Point Settings")]
        [SerializeField] private string spawnPointID;
        [SerializeField] private string targetSpawnPointID;
        [SerializeField] private float fadeDuration = 0.5f;
        [SerializeField] private float cooldownAfterTeleport = 0.5f;
        
        [Header("Visual Feedback")]
        [SerializeField] private SpriteRenderer visual;
        [SerializeField] private Color availableColor = Color.cyan;
        [SerializeField] private Color inUseColor = Color.yellow;
        
        private Collider2D triggerCollider;
        private global::Player playerInRange;
        private bool isTeleporting = false;
        private static bool anyPointTeleporting = false;
        
        public string SpawnPointID => spawnPointID;
        
        private void Awake()
        {
            triggerCollider = GetComponent<Collider2D>();
            if (triggerCollider != null)
            {
                triggerCollider.isTrigger = true;
            }
            
            if (string.IsNullOrEmpty(spawnPointID))
            {
                spawnPointID = $"Spawn_{gameObject.GetInstanceID()}";
                Debug.LogWarning($"<color=yellow>[CLIMB SPAWN] {gameObject.name} has no ID, auto-generated: {spawnPointID}</color>");
            }
            
            interactionPrompt = "Press E to climb";
            UpdateVisual(false);
            
            Debug.Log($"<color=cyan>[CLIMB SPAWN] {gameObject.name} initialized - ID: {spawnPointID}, Target: {targetSpawnPointID}</color>");
        }
        
        private void OnTriggerEnter2D(Collider2D other)
        {
            Debug.Log($"<color=magenta>[CLIMB SPAWN] OnTriggerEnter2D - {gameObject.name} collided with {other.gameObject.name}, tag: {other.tag}</color>");
            
            if (other.CompareTag("Player"))
            {
                global::Player player = other.GetComponent<global::Player>();
                if (player != null)
                {
                    playerInRange = player;
                    UpdateVisual(true);
                    Debug.Log($"<color=green>[CLIMB SPAWN] Player entered range of {gameObject.name}</color>");
                }
            }
        }
        
        private void OnTriggerExit2D(Collider2D other)
        {
            if (other.CompareTag("Player"))
            {
                playerInRange = null;
                UpdateVisual(false);
            }
        }
        
        protected override void OnInteract(GameObject interactor)
        {
            if (isTeleporting || anyPointTeleporting)
            {
                Debug.Log($"<color=yellow>[CLIMB SPAWN] Teleport in progress, ignoring interaction</color>");
                return;
            }
            
            Debug.Log($"<color=magenta>[CLIMB SPAWN] OnInteract called on {gameObject.name} by {interactor.name}</color>");
            
            global::Player player = interactor.GetComponent<global::Player>();
            if (player == null)
            {
                Debug.LogWarning($"<color=yellow>[CLIMB SPAWN] Interactor is not a player</color>");
                return;
            }
            
            if (string.IsNullOrEmpty(targetSpawnPointID))
            {
                Debug.LogError($"<color=red>[CLIMB SPAWN] {gameObject.name} has no target spawn point ID!</color>");
                return;
            }
            
            ClimbSpawnPoint targetSpawn = FindSpawnPointByID(targetSpawnPointID);
            if (targetSpawn == null)
            {
                Debug.LogError($"<color=red>[CLIMB SPAWN] Target spawn point '{targetSpawnPointID}' not found!</color>");
                return;
            }
            
            StartCoroutine(TriggerClimbCoroutine(player, targetSpawn));
        }
        
        private IEnumerator TriggerClimbCoroutine(global::Player player, ClimbSpawnPoint targetSpawn)
        {
            isTeleporting = true;
            anyPointTeleporting = true;
            
            isInteractable = false;
            targetSpawn.isInteractable = false;
            
            Debug.Log($"<color=green>[CLIMB SPAWN] Starting teleport from {spawnPointID} to {targetSpawnPointID}</color>");
            
            player.SetVelocityZero();
            float originalGravity = player.RB.gravityScale;
            player.RB.gravityScale = 0f;
            
            ScreenFadeManager.Instance.FadeToBlackAndTeleport(
                targetSpawn.transform.position,
                player.gameObject,
                fadeDuration
            );
            
            float totalDuration = (fadeDuration * 2) + 0.1f + cooldownAfterTeleport;
            yield return new WaitForSeconds(totalDuration);
            
            player.RB.gravityScale = originalGravity;
            
            isInteractable = true;
            targetSpawn.isInteractable = true;
            
            isTeleporting = false;
            anyPointTeleporting = false;
            
            Debug.Log($"<color=green>[CLIMB SPAWN] Teleport complete, cooldown finished</color>");
        }
        
        private ClimbSpawnPoint FindSpawnPointByID(string id)
        {
            ClimbSpawnPoint[] allSpawnPoints = FindObjectsByType<ClimbSpawnPoint>(FindObjectsSortMode.None);
            
            foreach (ClimbSpawnPoint spawn in allSpawnPoints)
            {
                if (spawn.SpawnPointID == id)
                {
                    return spawn;
                }
            }
            
            return null;
        }
        
        private void UpdateVisual(bool isPlayerNear)
        {
            if (visual != null)
            {
                visual.color = isPlayerNear ? inUseColor : availableColor;
            }
        }
        
        private void OnDrawGizmos()
        {
            Gizmos.color = Color.cyan;
            Gizmos.DrawWireSphere(transform.position, 0.5f);
            
            if (!string.IsNullOrEmpty(spawnPointID))
            {
#if UNITY_EDITOR
                UnityEditor.Handles.Label(transform.position + Vector3.up * 0.8f, $"ID: {spawnPointID}");
#endif
            }
        }
        
        private void OnDrawGizmosSelected()
        {
            if (string.IsNullOrEmpty(targetSpawnPointID))
                return;
            
            ClimbSpawnPoint targetSpawn = FindSpawnPointByID(targetSpawnPointID);
            if (targetSpawn != null)
            {
                Gizmos.color = Color.green;
                Gizmos.DrawLine(transform.position, targetSpawn.transform.position);
                
                Vector3 direction = (targetSpawn.transform.position - transform.position).normalized;
                Vector3 arrowPoint = targetSpawn.transform.position - direction * 0.5f;
                Gizmos.DrawSphere(arrowPoint, 0.2f);
                
#if UNITY_EDITOR
                UnityEditor.Handles.Label(
                    (transform.position + targetSpawn.transform.position) / 2f,
                    $"→ {targetSpawnPointID}"
                );
#endif
            }
            else
            {
                Gizmos.color = Color.red;
                Gizmos.DrawWireSphere(transform.position, 1f);
                
#if UNITY_EDITOR
                UnityEditor.Handles.Label(
                    transform.position + Vector3.down * 1.2f,
                    $"TARGET NOT FOUND: {targetSpawnPointID}"
                );
#endif
            }
        }
    }
}
