using UnityEngine;
using TheHunt.Inventory;
using TheHunt.Environment;
using TheHunt.UI;
using System;
#if UNITY_EDITOR
using UnityEditor;
#endif

namespace TheHunt.Interaction
{
    public class RopeAnchorPassiveItem : PassiveItem
    {
        [Header("Rope Settings")]
        [SerializeField] private float ropeLength = 5f;
        [SerializeField] private GameObject ropePrefab;
        [SerializeField] private Transform spawnPoint;
        [SerializeField] private float fadeDuration = 0.5f;
        
        [Header("Spawn Points")]
        [SerializeField] private ClimbSpawnPoint topSpawnPoint;
        [SerializeField] private ClimbSpawnPoint bottomSpawnPoint;
        
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
            
            interactionPrompt = "Press E to use anchor";
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
            return !isDeployed;
        }
        
        protected override void ExecutePassiveAction(GameObject interactor)
        {
            if (isDeployed)
            {
                Debug.LogWarning("<color=yellow>[ROPE ANCHOR] Rope already deployed</color>");
                return;
            }
            
            global::Player player = interactor.GetComponent<global::Player>();
            if (player == null)
            {
                Debug.LogWarning("<color=yellow>[ROPE ANCHOR] Interactor is not a player</color>");
                return;
            }
            
            pendingInteractor = interactor;
            
            bool hasRope = HasRopeInInventory(player);
            
            if (hasRope)
            {
                usedRopeItem = ropeItemData;
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
                dialogService.ShowConfirmation(
                    confirmationTitle,
                    confirmationMessage,
                    OnConfirmedWithRope,
                    OnCancelled
                );
                
                Debug.Log("<color=cyan>[ROPE ANCHOR] Showing confirmation dialog (player has rope)</color>");
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
        }
        
        private void OnConfirmedWithRope()
        {
            Debug.Log("<color=green>[ROPE ANCHOR] Deployment confirmed, starting fade...</color>");
            
            if (ScreenFadeManager.Instance != null)
            {
                ScreenFadeManager.Instance.FadeToBlack(fadeDuration, () =>
                {
                    ConsumeRopeFromInventory();
                    DeployRope();
                    
                    ScreenFadeManager.Instance.FadeFromBlack(fadeDuration, null);
                });
            }
            else
            {
                Debug.LogWarning("<color=yellow>[ROPE ANCHOR] ScreenFadeManager not found, deploying without fade</color>");
                ConsumeRopeFromInventory();
                DeployRope();
            }
            
            ClearPending();
        }
        
        private void OnCancelled()
        {
            Debug.Log("<color=yellow>[ROPE ANCHOR] Action cancelled</color>");
            ClearPending();
        }
        
        private void ClearPending()
        {
            pendingInteractor = null;
            usedRopeItem = null;
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
            
            SetInteractable(false);
            
            Debug.Log($"<color=green>[ROPE ANCHOR] Rope deployed successfully! Length: {ropeLength}</color>");
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
            if (topSpawnPoint != null)
            {
                topSpawnPoint.gameObject.SetActive(false);
                Debug.Log("<color=cyan>[ROPE ANCHOR] Top spawn point disabled</color>");
            }
            
            if (bottomSpawnPoint != null)
            {
                bottomSpawnPoint.gameObject.SetActive(false);
                Debug.Log("<color=cyan>[ROPE ANCHOR] Bottom spawn point disabled</color>");
            }
        }
        
        private void EnableSpawnPoints()
        {
            Debug.Log("<color=cyan>[ROPE ANCHOR] === EnableSpawnPoints() called ===</color>");
            
            if (topSpawnPoint != null)
            {
                Debug.Log($"<color=cyan>[ROPE ANCHOR] Top spawn point found: {topSpawnPoint.gameObject.name}</color>");
                Debug.Log($"<color=cyan>[ROPE ANCHOR]   - Current active state: {topSpawnPoint.gameObject.activeSelf}</color>");
                topSpawnPoint.gameObject.SetActive(true);
                Debug.Log($"<color=green>[ROPE ANCHOR]   - ✓ Set to ACTIVE</color>");
            }
            else
            {
                Debug.LogWarning("<color=yellow>[ROPE ANCHOR] ✗ Top spawn point is NULL!</color>");
            }
            
            if (bottomSpawnPoint != null)
            {
                Debug.Log($"<color=cyan>[ROPE ANCHOR] Bottom spawn point found: {bottomSpawnPoint.gameObject.name}</color>");
                Debug.Log($"<color=cyan>[ROPE ANCHOR]   - Current active state: {bottomSpawnPoint.gameObject.activeSelf}</color>");
                bottomSpawnPoint.gameObject.SetActive(true);
                Debug.Log($"<color=green>[ROPE ANCHOR]   - ✓ Set to ACTIVE</color>");
            }
            else
            {
                Debug.LogWarning("<color=yellow>[ROPE ANCHOR] ✗ Bottom spawn point is NULL!</color>");
            }
            
            Debug.Log("<color=green>[ROPE ANCHOR] === EnableSpawnPoints() completed ===</color>");
        }
        
        public void RetractRope()
        {
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
            
            SetInteractable(true);
            
            Debug.Log("<color=orange>[ROPE ANCHOR] Rope retracted</color>");
        }
    }
}
