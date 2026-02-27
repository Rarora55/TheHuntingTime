using UnityEngine;
using TheHunt.Inventory;
using TheHunt.Environment;
using TheHunt.UI;
using TheHunt.Events;
using TheHunt.Interaction.Rope;
using System;
#if UNITY_EDITOR
using UnityEditor;
#endif

namespace TheHunt.Interaction
{
    public class RopeAnchorPassiveItem : PassiveItem
    {
        [Header("Events")]
        [SerializeField] private ScreenFadeEvent screenFadeEvent;

        [Header("Rope Settings")]
        [SerializeField] private float ropeLength = 5f;
        [SerializeField] private GameObject ropePrefab;
        [SerializeField] private Transform spawnPoint;
        [SerializeField] private float fadeDuration = 0.5f;
        
        [Header("Spawn Points")]
        [SerializeField] private GameObject topSpawnPoint;
        [SerializeField] private GameObject bottomSpawnPoint;
        [Tooltip("Configuration for rope spawn point dialogs and usage.")]
        [SerializeField] private RopeSpawnConfig ropeSpawnConfig;
        
        [Header("Visual Feedback")]
        [SerializeField] private SpriteRenderer anchorSprite;
        [SerializeField] private Color deployedColor = Color.gray;
        
        [Header("Dialog Settings")]
        [SerializeField] private string confirmationTitle = "Deploy Rope";
        [SerializeField] private string confirmationMessage = "Do you want to deploy the rope?";
        [SerializeField] private string noRopeMessage = "I need a rope";
        
        private GameObject deployedRope;
        private bool isDeployed = false;
        private Color originalColor;
        private DialogService dialogService;
        private GameObject pendingInteractor;
        private ItemData usedRopeItem;
        
        [Header("Rope Item Reference")]
        [SerializeField] private ItemData ropeItemData;
        
        private void Awake()
        {
            if (anchorSprite != null)
            {
                originalColor = anchorSprite.color;
            }
            
            if (spawnPoint == null)
            {
                spawnPoint = transform;
            }
            
            LoadRopePrefabIfNeeded();
            
            dialogService = FindFirstObjectByType<DialogService>();
            if (dialogService == null)
            {
                Debug.LogError("<color=red>[ROPE ANCHOR] DialogService not found in scene!</color>");
            }
            
            DisableSpawnPoints();
            
            if (isDeployed)
            {
                interactionPrompt = "Press E to retrieve rope";
            }
            else
            {
                interactionPrompt = "Press E to use anchor";
            }
        }
        
        private void LoadRopePrefabIfNeeded()
        {
            if (ropePrefab != null)
            {
                Debug.Log($"<color=green>[ROPE ANCHOR] ropePrefab already assigned: {ropePrefab.name}</color>");
                return;
            }
            
            #if UNITY_EDITOR
            string prefabPath = "Assets/Prefabs/ObjectsForTests/RopeClimbable.prefab";
            ropePrefab = UnityEditor.AssetDatabase.LoadAssetAtPath<GameObject>(prefabPath);
            if (ropePrefab != null)
            {
                Debug.Log($"<color=cyan>[ROPE ANCHOR] ✓ Auto-loaded RopeClimbable prefab from {prefabPath}</color>");
            }
            else
            {
                Debug.LogError($"<color=red>[ROPE ANCHOR] ✗ Failed to auto-load RopeClimbable prefab from {prefabPath}</color>");
            }
            #else
            Debug.LogError($"<color=red>[ROPE ANCHOR] ✗ ropePrefab is null and cannot auto-load in build!</color>");
            #endif
        }
        
        protected override bool CanExecuteAction(GameObject interactor)
        {
            return true;
        }
        
        protected override void ExecutePassiveAction(GameObject interactor)
        {
            Debug.Log($"<color=magenta>[ROPE ANCHOR] ===== ExecutePassiveAction() CALLED ===== isDeployed: {isDeployed}</color>");
            
            global::Player player = interactor.GetComponent<global::Player>();
            if (player == null)
            {
                Debug.LogWarning("<color=yellow>[ROPE ANCHOR] Interactor is not a player</color>");
                return;
            }
            
            pendingInteractor = interactor;
            
            if (isDeployed)
            {
                // SEGUNDA INTERACCIÓN: Cuerda ya colocada
                Debug.Log("<color=cyan>[ROPE ANCHOR] Rope already deployed - showing pickup/use options</color>");
                ShowRopeOptionsDialog();
            }
            else
            {
                // PRIMERA INTERACCIÓN: Colocar cuerda
                Debug.Log("<color=cyan>[ROPE ANCHOR] Rope NOT deployed - attempting to deploy</color>");
                ShowDeployConfirmation(player);
            }
            
            Debug.Log($"<color=magenta>[ROPE ANCHOR] ===== ExecutePassiveAction() COMPLETE =====</color>");
        }
        
        private void ShowDeployConfirmation(global::Player player)
        {
            Debug.Log("<color=magenta>[ROPE ANCHOR] === ShowDeployConfirmation() called ===</color>");
            
            bool hasRope = HasRopeInInventory(player);
            
            if (hasRope)
            {
                usedRopeItem = ropeItemData;
            }
            
            // ✅ CRÍTICO: Cerrar cualquier diálogo abierto antes de desplegar
            if (dialogService != null && dialogService.IsDialogOpen)
            {
                Debug.Log("<color=yellow>[ROPE ANCHOR] Closing any open dialog before deployment</color>");
                dialogService.HideDialog();
            }
            
            if (dialogService == null)
            {
                Debug.LogError("<color=red>[ROPE ANCHOR] DialogService is null! Cannot show confirmation.</color>");
                if (hasRope)
                {
                    ConsumeRopeFromInventory();
                    DeployRope();
                }
                return;
            }
            
            if (hasRope)
            {
                // ✅ PRIMERA VEZ: Colocar cuerda directamente SIN PREGUNTAR
                Debug.Log("<color=green>[ROPE ANCHOR] Player has rope, deploying directly (no confirmation on first deployment)</color>");
                
                if (screenFadeEvent != null)
                {
                    screenFadeEvent.RaiseFadeToBlack(fadeDuration, () =>
                    {
                        Debug.Log("<color=cyan>[ROPE ANCHOR] Inside fade callback - deploying rope now</color>");
                        ConsumeRopeFromInventory();
                        DeployRope();
                        
                        screenFadeEvent.RaiseFadeFromBlack(fadeDuration, () =>
                        {
                            Debug.Log("<color=cyan>[ROPE ANCHOR] Fade complete - clearing pending</color>");
                            ClearPending();
                        });
                    });
                }
                else
                {
                    Debug.LogWarning("<color=yellow>[ROPE ANCHOR] ScreenFadeEvent not assigned, deploying without fade</color>");
                    ConsumeRopeFromInventory();
                    DeployRope();
                    ClearPending();
                }
            }
            else
            {
                dialogService.ShowInfo(
                    confirmationTitle,
                    noRopeMessage,
                    OnCancelled
                );
                
                Debug.Log("<color=yellow>[ROPE ANCHOR] Showing 'no rope' message</color>");
            }
            
            Debug.Log("<color=magenta>[ROPE ANCHOR] === ShowDeployConfirmation() complete ===</color>");
        }
        
        private void ShowRopeOptionsDialog()
        {
            Debug.Log("<color=cyan>[ROPE ANCHOR] === ShowRopeOptionsDialog() called ===</color>");
            
            if (dialogService == null)
            {
                Debug.LogError("<color=red>[ROPE ANCHOR] DialogService is null!</color>");
                ClearPending();
                return;
            }
            
            // Pregunta: ¿Coger cuerda o Usar cuerda?
            dialogService.ShowConfirmation(
                "Rope",
                "Pickup rope (YES) or Use rope (NO)?",
                OnConfirmedPickupRope,  // YES → Coger cuerda del anchor
                OnConfirmedUseRope      // NO → Usar spawn points (cierra diálogo)
            );
            
            Debug.Log("<color=cyan>[ROPE ANCHOR] Showing rope options: Pickup (YES) or Use (NO)</color>");
        }
        
        private void OnConfirmedPickupRope()
        {
            Debug.Log("<color=green>[ROPE ANCHOR] === OnConfirmedPickupRope() - Picking up rope from anchor ===</color>");
            
            if (screenFadeEvent != null)
            {
                screenFadeEvent.RaiseFadeToBlack(fadeDuration, () =>
                {
                    RetractRopeInternal();
                    ReturnRopeToInventory();
                    
                    screenFadeEvent.RaiseFadeFromBlack(fadeDuration, () =>
                    {
                        ClearPending();
                    });
                });
            }
            else
            {
                RetractRopeInternal();
                ReturnRopeToInventory();
                ClearPending();
            }
        }
        
        private void OnConfirmedUseRope()
        {
            Debug.Log("<color=green>[ROPE ANCHOR] === OnConfirmedUseRope() - Using respawn system ===</color>");
            
            // En lugar de teletransportar directamente, simplemente cancelar el diálogo
            // El jugador debe usar los ClimbSpawnPoint normales (topSpawnPoint o bottomSpawnPoint)
            Debug.Log("<color=cyan>[ROPE ANCHOR] Dialog closed - Player can now interact with spawn points</color>");
            ClearPending();
        }
        
        private void OnCancelled()
        {
            Debug.Log("<color=yellow>[ROPE ANCHOR] Action cancelled - Clearing pending and re-enabling player control</color>");
            ClearPending();
            
            // ✅ NUEVO: Asegurar que el input del jugador se libere
            if (pendingInteractor != null)
            {
                var simpleConfirmable = pendingInteractor.GetComponent<TheHunt.Interaction.SimpleConfirmableInteraction>();
                if (simpleConfirmable != null)
                {
                    // Forzar reset del estado de confirmación
                    var field = simpleConfirmable.GetType().GetField("isWaitingForConfirmation", 
                        System.Reflection.BindingFlags.NonPublic | System.Reflection.BindingFlags.Instance);
                    if (field != null)
                    {
                        field.SetValue(simpleConfirmable, false);
                        Debug.Log("<color=green>[ROPE ANCHOR] ✓ Forced reset of confirmation state</color>");
                    }
                }
            }
        }
        
        private void ClearPending()
        {
            Debug.Log("<color=cyan>[ROPE ANCHOR] === ClearPending() called ===</color>");
            
            // Liberar el InputContext si está bloqueado
            if (pendingInteractor != null)
            {
                var inputContextManager = pendingInteractor.GetComponent<TheHunt.Input.InputContextManager>();
                if (inputContextManager != null)
                {
                    Debug.Log($"<color=cyan>[ROPE ANCHOR] Current InputContext: {inputContextManager.CurrentContext}</color>");
                    
                    // Forzar vuelta a Gameplay si está en Dialog
                    if (inputContextManager.CurrentContext == TheHunt.Input.InputContext.Dialog)
                    {
                        inputContextManager.PopContext();
                        Debug.Log("<color=green>[ROPE ANCHOR] ✓ Popped Dialog context back to Gameplay</color>");
                    }
                }
            }
            
            pendingInteractor = null;
            usedRopeItem = null;
            
            Debug.Log("<color=cyan>[ROPE ANCHOR] === ClearPending() complete ===</color>");
        }
        
        private void DeployRope()
        {
            if (ropePrefab == null)
            {
                Debug.LogError("<color=red>[ROPE ANCHOR] No rope prefab assigned!</color>");
                return;
            }
            
            Vector3 spawnPosition = spawnPoint.position;
            Vector3 ropeEndPosition = spawnPosition + Vector3.down * ropeLength;
            
            deployedRope = Instantiate(ropePrefab, spawnPosition, Quaternion.identity);
            deployedRope.name = "DeployedRope";
            
            ScaleRopeToFit(deployedRope, spawnPosition, ropeEndPosition);
            
            isDeployed = true;
            
            if (anchorSprite != null)
            {
                anchorSprite.color = deployedColor;
            }
            
            EnableSpawnPoints();
            
            interactionPrompt = "Press E to retrieve rope";
            
            Debug.Log($"<color=green>[ROPE ANCHOR] ✓ Rope deployed successfully! Length: {ropeLength}</color>");
            Debug.Log($"<color=cyan>[ROPE ANCHOR] ✓ Prompt changed to 'retrieve rope'</color>");
        }
        
        private void ScaleRopeToFit(GameObject rope, Vector3 startPos, Vector3 endPos)
        {
            float distance = Vector3.Distance(startPos, endPos);
            
            SpriteRenderer ropeSprite = rope.GetComponent<SpriteRenderer>();
            if (ropeSprite != null && ropeSprite.sprite != null)
            {
                float spriteHeight = ropeSprite.sprite.bounds.size.y;
                float scaleY = distance / spriteHeight;
                rope.transform.localScale = new Vector3(1f, scaleY, 1f);
                
                Debug.Log($"<color=cyan>[ROPE ANCHOR] Rope scaled: distance={distance}, spriteHeight={spriteHeight}, scaleY={scaleY}</color>");
            }
            
            BoxCollider2D ropeCollider = rope.GetComponent<BoxCollider2D>();
            if (ropeCollider != null)
            {
                ropeCollider.size = new Vector2(ropeCollider.size.x, distance);
                ropeCollider.offset = new Vector2(0, -distance / 2f);
                
                Debug.Log($"<color=cyan>[ROPE ANCHOR] Rope collider updated: size={ropeCollider.size}, offset={ropeCollider.offset}</color>");
            }
            
            rope.transform.position = startPos;
        }
        
        private void ConsumeRopeFromInventory()
        {
            if (pendingInteractor == null || usedRopeItem == null)
            {
                Debug.LogWarning("<color=yellow>[ROPE ANCHOR] Cannot consume rope: no pending interactor or rope item</color>");
                return;
            }
            
            global::Player player = pendingInteractor.GetComponent<global::Player>();
            if (player == null)
            {
                Debug.LogWarning("<color=yellow>[ROPE ANCHOR] Cannot consume rope: interactor is not a player</color>");
                return;
            }
            
            InventorySystem inventory = player.GetComponent<InventorySystem>();
            if (inventory == null)
            {
                Debug.LogError("<color=red>[ROPE ANCHOR] InventorySystem not found on player!</color>");
                return;
            }
            
            Debug.Log($"<color=cyan>[ROPE ANCHOR DEBUG] About to remove item:</color>\n" +
                     $"  - ItemData reference: {usedRopeItem}\n" +
                     $"  - ItemName: {usedRopeItem.ItemName}\n" +
                     $"  - ItemID: {usedRopeItem.ItemID}\n" +
                     $"  - Current count in inventory: {inventory.GetItemCount(usedRopeItem)}");
            
            bool removed = inventory.RemoveItem(usedRopeItem, 1);
            
            if (removed)
            {
                Debug.Log($"<color=green>[ROPE ANCHOR] ✓ Successfully consumed {usedRopeItem.ItemName} from inventory</color>");
                Debug.Log($"<color=cyan>[ROPE ANCHOR] Remaining count: {inventory.GetItemCount(usedRopeItem)}</color>");
            }
            else
            {
                Debug.LogWarning($"<color=red>[ROPE ANCHOR] ✗ FAILED to remove {usedRopeItem.ItemName} from inventory!</color>");
                Debug.LogWarning($"<color=yellow>[ROPE ANCHOR DEBUG] Current inventory state:</color>");
                
                // Debug all inventory items
                var inventoryData = inventory.InventoryData;
                for (int i = 0; i < 6; i++)
                {
                    var item = inventoryData.GetItem(i);
                    if (item != null && item.itemData != null)
                    {
                        Debug.LogWarning($"  Slot {i}: {item.itemData.ItemName} (ID: {item.itemData.ItemID}) x{item.quantity} " +
                                       $"[Reference match: {item.itemData == usedRopeItem}]");
                    }
                }
            }
        }
        
        private bool HasRopeInInventory(global::Player player)
        {
            if (ropeItemData == null)
            {
                Debug.LogError("<color=red>[ROPE ANCHOR] Rope ItemData not assigned in inspector!</color>");
                return false;
            }
            
            InventorySystem inventory = player.GetComponent<InventorySystem>();
            if (inventory == null)
            {
                Debug.LogWarning("<color=yellow>[ROPE ANCHOR] InventorySystem not found on player</color>");
                return false;
            }
            
            int ropeCount = inventory.GetItemCount(ropeItemData);
            bool hasRope = ropeCount > 0;
            
            if (hasRope)
            {
                Debug.Log($"<color=cyan>[ROPE ANCHOR] Found {ropeCount} rope(s) in inventory</color>");
            }
            else
            {
                Debug.Log("<color=yellow>[ROPE ANCHOR] No rope found in inventory</color>");
            }
            
            return hasRope;
        }
        
        private void DisableSpawnPoints()
        {
            Debug.Log("<color=orange>[ROPE ANCHOR] === DisableSpawnPoints() - Deactivating spawn points ===</color>");
            
            if (topSpawnPoint != null)
            {
                topSpawnPoint.SetActive(false);
                Debug.Log($"<color=orange>[ROPE ANCHOR] ✓ TOP spawn point '{topSpawnPoint.name}' DEACTIVATED</color>");
            }
            
            if (bottomSpawnPoint != null)
            {
                bottomSpawnPoint.SetActive(false);
                Debug.Log($"<color=orange>[ROPE ANCHOR] ✓ BOTTOM spawn point '{bottomSpawnPoint.name}' DEACTIVATED</color>");
            }
            
            Debug.Log("<color=orange>[ROPE ANCHOR] === DisableSpawnPoints() completed ===</color>");
        }
        
        private void EnableSpawnPoints()
        {
            Debug.Log("<color=cyan>[ROPE ANCHOR] === EnableSpawnPoints() - Activating spawn points ===</color>");
            
            InitializeRopeSpawnPoint(topSpawnPoint, "TOP");
            InitializeRopeSpawnPoint(bottomSpawnPoint, "BOTTOM");
            
            Debug.Log("<color=green>[ROPE ANCHOR] === EnableSpawnPoints() completed ===</color>");
        }

        private void InitializeRopeSpawnPoint(GameObject spawnPointGO, string label)
        {
            if (spawnPointGO == null)
            {
                Debug.LogWarning($"<color=yellow>[ROPE ANCHOR] ✗ {label} spawn point is NULL!</color>");
                return;
            }

            spawnPointGO.SetActive(true);

            var ropeSpawnController = spawnPointGO.GetComponent<RopeClimbSpawnPoint>();
            if (ropeSpawnController != null && ropeSpawnConfig != null)
            {
                ropeSpawnController.Initialize(this, ropeSpawnConfig);
                Debug.Log($"<color=green>[ROPE ANCHOR] ✓ {label} '{spawnPointGO.name}' ACTIVATED + RopeClimbSpawnPoint initialized</color>");
            }
            else
            {
                Debug.Log($"<color=green>[ROPE ANCHOR] ✓ {label} '{spawnPointGO.name}' ACTIVATED (no RopeClimbSpawnPoint found)</color>");
            }
        }
        
        private void RetractRopeInternal()
        {
            if (!isDeployed)
            {
                Debug.LogWarning("<color=yellow>[ROPE ANCHOR] ✗ Cannot retract: No rope deployed</color>");
                return;
            }
            
            if (deployedRope != null)
            {
                Destroy(deployedRope);
                deployedRope = null;
            }
            
            isDeployed = false;
            
            if (anchorSprite != null)
            {
                anchorSprite.color = originalColor;
            }
            
            DisableSpawnPoints();
            
            interactionPrompt = "Press E to use anchor";
            
            Debug.Log("<color=orange>[ROPE ANCHOR] ✓ Rope retracted successfully</color>");
            Debug.Log("<color=cyan>[ROPE ANCHOR] ✓ Spawn points DISABLED</color>");
            Debug.Log("<color=cyan>[ROPE ANCHOR] ✓ Prompt changed to 'use anchor'</color>");
        }
        
        private void ReturnRopeToInventory()
        {
            if (pendingInteractor == null)
            {
                Debug.LogWarning("<color=yellow>[ROPE ANCHOR] Cannot return rope: no pending interactor</color>");
                return;
            }
            
            global::Player player = pendingInteractor.GetComponent<global::Player>();
            if (player == null)
            {
                Debug.LogWarning("<color=yellow>[ROPE ANCHOR] Cannot return rope: interactor is not a player</color>");
                return;
            }
            
            InventorySystem inventory = player.GetComponent<InventorySystem>();
            if (inventory == null)
            {
                Debug.LogError("<color=red>[ROPE ANCHOR] InventorySystem not found on player!</color>");
                return;
            }
            
            if (ropeItemData == null)
            {
                Debug.LogError("<color=red>[ROPE ANCHOR] Rope ItemData not assigned!</color>");
                return;
            }
            
            bool added = inventory.TryAddItem(ropeItemData);
            
            if (added)
            {
                Debug.Log($"<color=green>[ROPE ANCHOR] ✓ Successfully returned {ropeItemData.ItemName} to inventory</color>");
            }
            else
            {
                Debug.LogWarning($"<color=red>[ROPE ANCHOR] ✗ FAILED to return {ropeItemData.ItemName} to inventory (inventory full?)</color>");
            }
        }
        
        public void RetractRope()
        {
            RetractRopeInternal();
        }
    }
}
