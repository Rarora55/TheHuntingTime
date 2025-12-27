using System;
using System.Collections.Generic;
using UnityEngine;

namespace TheHunt.Inventory
{
    public class InventorySystem : MonoBehaviour
    {
        public const int MAX_SLOTS = 6;
        public const int MAX_STACK_SIZE = 6;

        [Header("Data Reference")]
        [SerializeField] private InventoryDataSO inventoryData;

        private AmmoInventoryManager ammoManager;
        private WeaponInventoryManager weaponManager;

        public ItemInstance CurrentItem => inventoryData != null && inventoryData.SelectedIndex >= 0 && inventoryData.SelectedIndex < MAX_SLOTS 
            ? inventoryData.GetItem(inventoryData.SelectedIndex) 
            : null;
        public bool IsFull => FindEmptySlot() == -1;
        public bool HasSpace => !IsFull;
        public int SelectedSlot => inventoryData != null ? inventoryData.SelectedIndex : 0;
        public ItemInstance[] Items => inventoryData != null ? inventoryData.Items : new ItemInstance[MAX_SLOTS];

        public event Action<int, ItemInstance> OnItemAdded;
        public event Action<int, ItemInstance> OnItemRemoved;
        public event Action<ItemInstance> OnItemUsed;
        public event Action<int, int> OnSelectionChanged;
        public event Action OnInventoryFull;

        void Awake()
        {
            ammoManager = GetComponent<AmmoInventoryManager>();
            weaponManager = GetComponent<WeaponInventoryManager>();
            
            if (inventoryData == null)
            {
                Debug.LogError("<color=red>[INVENTORY] InventoryDataSO reference is missing! Please assign it in the Inspector.</color>");
            }
        }

        public bool TryAddItem(ItemData itemData)
        {
            if (itemData == null)
            {
                Debug.LogWarning("[INVENTORY] Cannot add null item");
                return false;
            }
            
            if (inventoryData == null)
            {
                Debug.LogError("[INVENTORY] InventoryDataSO is not assigned!");
                return false;
            }

            int quantityToAdd = 1;
            
            if (itemData is AmmoItemData ammoData)
            {
                quantityToAdd = ammoData.AmmoAmount;
            }

            if (itemData.IsStackable)
            {
                for (int i = 0; i < MAX_SLOTS; i++)
                {
                    ItemInstance item = inventoryData.GetItem(i);
                    if (item != null &&
                        item.itemData == itemData &&
                        item.quantity < MAX_STACK_SIZE)
                    {
                        int spaceLeft = MAX_STACK_SIZE - item.quantity;
                        int toAdd = Mathf.Min(quantityToAdd, spaceLeft);
                        
                        item.quantity += toAdd;
                        inventoryData.SetItem(i, item);
                        OnItemAdded?.Invoke(i, item);
                        
                        quantityToAdd -= toAdd;
                        
                        Debug.Log($"<color=green>[INVENTORY] Stacked {toAdd} {itemData.ItemName}. Total: {item.quantity}</color>");
                        
                        if (quantityToAdd <= 0)
                            return true;
                    }
                }
            }

            while (quantityToAdd > 0)
            {
                int emptySlot = FindEmptySlot();
                if (emptySlot == -1)
                {
                    OnInventoryFull?.Invoke();
                    Debug.Log("<color=yellow>[INVENTORY] Inventory is full!</color>");
                    return quantityToAdd < (itemData is AmmoItemData ammo ? ammo.AmmoAmount : 1);
                }

                int stackSize = itemData.IsStackable ? Mathf.Min(quantityToAdd, MAX_STACK_SIZE) : 1;
                
                ItemInstance newItem = new ItemInstance(itemData, stackSize);
                inventoryData.SetItem(emptySlot, newItem);
                OnItemAdded?.Invoke(emptySlot, newItem);
                
                Debug.Log($"<color=green>[INVENTORY] Added {stackSize} {itemData.ItemName} to slot {emptySlot}</color>");
                
                quantityToAdd -= stackSize;
                
                if (!itemData.IsStackable)
                    break;
            }

            return true;
        }

        public void RemoveItem(int slotIndex, int quantity = 1)
        {
            if (inventoryData == null)
                return;
                
            if (slotIndex < 0 || slotIndex >= MAX_SLOTS)
                return;
                
            ItemInstance item = inventoryData.GetItem(slotIndex);
            if (item == null || item.itemData == null)
                return;

            item.quantity -= quantity;

            if (item.quantity <= 0)
            {
                inventoryData.SetItem(slotIndex, null);
                Debug.Log($"<color=orange>[INVENTORY] Removed {item.itemData.ItemName} completely from slot {slotIndex}</color>");
            }
            else
            {
                inventoryData.SetItem(slotIndex, item);
                Debug.Log($"<color=orange>[INVENTORY] Removed {quantity} {item.itemData.ItemName}. Remaining: {item.quantity}</color>");
            }

            OnItemRemoved?.Invoke(slotIndex, item);
        }
        
        public bool HasItem(ItemData itemData)
        {
            if (itemData == null || inventoryData == null)
                return false;
            
            for (int i = 0; i < MAX_SLOTS; i++)
            {
                ItemInstance item = inventoryData.GetItem(i);
                if (item != null && item.itemData == itemData)
                {
                    return true;
                }
            }
            
            return false;
        }
        
        public int GetItemCount(ItemData itemData)
        {
            if (itemData == null || inventoryData == null)
                return 0;
            
            int count = 0;
            for (int i = 0; i < MAX_SLOTS; i++)
            {
                ItemInstance item = inventoryData.GetItem(i);
                if (item != null && item.itemData == itemData)
                {
                    count += item.quantity;
                }
            }
            
            return count;
        }
        
        public bool RemoveItem(ItemData itemData, int quantity = 1)
        {
            if (itemData == null || quantity <= 0 || inventoryData == null)
                return false;
            
            int remaining = quantity;
            
            for (int i = 0; i < MAX_SLOTS && remaining > 0; i++)
            {
                ItemInstance item = inventoryData.GetItem(i);
                if (item != null && item.itemData == itemData)
                {
                    int toRemove = Mathf.Min(remaining, item.quantity);
                    RemoveItem(i, toRemove);
                    remaining -= toRemove;
                }
            }
            
            bool success = remaining == 0;
            
            if (success)
            {
                Debug.Log($"<color=green>[INVENTORY] Successfully removed {quantity} {itemData.ItemName}</color>");
            }
            else
            {
                Debug.LogWarning($"<color=yellow>[INVENTORY] Could not remove {quantity} {itemData.ItemName}, only had {quantity - remaining}</color>");
            }
            
            return success;
        }

        public int GetAmmoCount(AmmoType ammoType)
        {
            if (inventoryData == null || ammoType == AmmoType.None)
                return 0;

            int totalAmmo = 0;

            for (int i = 0; i < MAX_SLOTS; i++)
            {
                ItemInstance item = inventoryData.GetItem(i);
                if (item != null && item.itemData is AmmoItemData ammoData)
                {
                    if (ammoData.AmmoType == ammoType)
                    {
                        totalAmmo += item.quantity;
                    }
                }
            }

            return totalAmmo;
        }

        public bool ConsumeAmmo(AmmoType ammoType, int amount)
        {
            if (inventoryData == null || ammoType == AmmoType.None || amount <= 0)
                return false;

            int totalAvailable = GetAmmoCount(ammoType);

            if (totalAvailable < amount)
            {
                Debug.LogWarning($"<color=yellow>[INVENTORY] Not enough {ammoType} ammo. Need: {amount}, Have: {totalAvailable}</color>");
                return false;
            }

            int remaining = amount;

            for (int i = 0; i < MAX_SLOTS && remaining > 0; i++)
            {
                ItemInstance item = inventoryData.GetItem(i);
                if (item != null && item.itemData is AmmoItemData ammoData)
                {
                    if (ammoData.AmmoType == ammoType)
                    {
                        int toRemove = Mathf.Min(remaining, item.quantity);
                        RemoveItem(i, toRemove);
                        remaining -= toRemove;
                    }
                }
            }

            Debug.Log($"<color=green>[INVENTORY] Consumed {amount} {ammoType} ammo. Remaining: {GetAmmoCount(ammoType)}</color>");
            return true;
        }

        public void UseCurrentItem()
        {
            if (CurrentItem == null)
            {
                Debug.Log("<color=yellow>[INVENTORY] No item selected</color>");
                return;
            }

            if (CurrentItem.itemData is IUsable usable)
            {
                if (!usable.CanUse(gameObject))
                {
                    Debug.Log($"<color=yellow>[INVENTORY] Cannot use {CurrentItem.itemData.ItemName}</color>");
                    return;
                }

                CurrentItem.itemData.Use(gameObject);
                OnItemUsed?.Invoke(CurrentItem);

                if (usable.RemoveOnUse)
                {
                    RemoveItem(inventoryData.SelectedIndex, 1);
                }
            }
            else
            {
                Debug.Log($"<color=yellow>[INVENTORY] {CurrentItem.itemData.ItemName} is not usable</color>");
            }
        }

        public void DropCurrentItem()
        {
            if (CurrentItem == null)
                return;

            ItemData droppedItemData = CurrentItem.itemData;
            
            Debug.Log($"<color=cyan>[INVENTORY] Dropped {droppedItemData.ItemName}</color>");
            
            RemoveItem(inventoryData.SelectedIndex, 1);
            
            if (droppedItemData.PickupPrefab != null)
            {
                Vector3 dropPosition = CalculateDropPosition(droppedItemData.PickupPrefab);
                GameObject droppedObject = Instantiate(droppedItemData.PickupPrefab, dropPosition, Quaternion.identity);
                
                Debug.Log($"<color=green>[INVENTORY] Spawned {droppedItemData.ItemName} at {dropPosition}</color>");
            }
            else
            {
                Debug.LogWarning($"<color=yellow>[INVENTORY] No pickup prefab assigned for {droppedItemData.ItemName}</color>");
            }
        }

        private Vector3 CalculateDropPosition(GameObject prefab)
        {
            const float HORIZONTAL_OFFSET = 1.5f;
            const float RAYCAST_DISTANCE = 50f;
            
            Vector3 horizontalOffset = transform.right * HORIZONTAL_OFFSET;
            Vector3 startPosition = transform.position + horizontalOffset;
            
            int groundLayer = LayerMask.GetMask("Ground");
            RaycastHit2D hit = Physics2D.Raycast(startPosition, Vector2.down, RAYCAST_DISTANCE, groundLayer);
            
            Vector3 groundContactPoint;
            if (hit.collider != null)
            {
                groundContactPoint = hit.point;
                Debug.Log($"<color=cyan>[DROP] Ground found at {groundContactPoint}</color>");
            }
            else
            {
                groundContactPoint = startPosition;
                Debug.LogWarning($"<color=yellow>[DROP] No ground found, using horizontal position</color>");
            }
            
            Vector3 detectionOffset = GetDetectionPointOffset(prefab);
            
            Vector3 finalPosition = new Vector3(
                groundContactPoint.x,
                groundContactPoint.y - detectionOffset.y,
                groundContactPoint.z
            );
            
            Debug.Log($"<color=green>[DROP] Detection offset: {detectionOffset}, Final position: {finalPosition}</color>");
            return finalPosition;
        }
        
        private Vector3 GetDetectionPointOffset(GameObject prefab)
        {
            Transform detectionPoint = FindDetectionPointInPrefab(prefab);
            
            if (detectionPoint != null)
            {
                Vector3 localPos = detectionPoint.localPosition;
                Vector3 scale = prefab.transform.localScale;
                
                Vector3 worldOffset = new Vector3(
                    localPos.x * scale.x,
                    localPos.y * scale.y,
                    localPos.z * scale.z
                );
                
                Debug.Log($"<color=cyan>[DROP] Found detection point: local={localPos}, scale={scale}, worldOffset={worldOffset}</color>");
                return worldOffset;
            }
            
            Debug.LogWarning($"<color=yellow>[DROP] No 'detectionGround' found in prefab, using zero offset</color>");
            return Vector3.zero;
        }
        
        private Transform FindDetectionPointInPrefab(GameObject prefab)
        {
            Transform root = prefab.transform;
            
            Transform detectionPoint = root.Find("detectionGround");
            if (detectionPoint != null)
                return detectionPoint;
            
            detectionPoint = root.Find("detectioGround");
            if (detectionPoint != null)
                return detectionPoint;
            
            detectionPoint = root.Find("DetectionGround");
            if (detectionPoint != null)
                return detectionPoint;
            
            return null;
        }

        public void ExamineCurrentItem()
        {
            if (CurrentItem == null)
                return;

            if (!CurrentItem.itemData.CanBeExamined)
            {
                Debug.Log($"<color=yellow>[INVENTORY] {CurrentItem.itemData.ItemName} cannot be examined</color>");
                return;
            }

            ItemExaminationPanel examinationPanel = FindFirstObjectByType<ItemExaminationPanel>();
            if (examinationPanel != null)
            {
                examinationPanel.ShowItem(CurrentItem.itemData);
            }
            else
            {
                Debug.Log($"<color=cyan>[EXAMINE] {CurrentItem.itemData.ItemName}\n{CurrentItem.itemData.ExaminationText}</color>");
            }
        }

        public void SelectNext()
        {
            if (inventoryData == null)
                return;
                
            int oldIndex = inventoryData.SelectedIndex;
            inventoryData.SelectedIndex = (inventoryData.SelectedIndex + 1) % MAX_SLOTS;
            OnSelectionChanged?.Invoke(oldIndex, inventoryData.SelectedIndex);
            Debug.Log($"<color=cyan>[INVENTORY] Selected slot {inventoryData.SelectedIndex}</color>");
        }

        public void SelectPrevious()
        {
            if (inventoryData == null)
                return;
                
            int oldIndex = inventoryData.SelectedIndex;
            inventoryData.SelectedIndex--;
            if (inventoryData.SelectedIndex < 0)
                inventoryData.SelectedIndex = MAX_SLOTS - 1;
            OnSelectionChanged?.Invoke(oldIndex, inventoryData.SelectedIndex);
            Debug.Log($"<color=cyan>[INVENTORY] Selected slot {inventoryData.SelectedIndex}</color>");
        }

        public void SelectSlot(int index)
        {
            if (index < 0 || index >= MAX_SLOTS || inventoryData == null)
                return;

            int oldIndex = inventoryData.SelectedIndex;
            inventoryData.SelectedIndex = index;
            OnSelectionChanged?.Invoke(oldIndex, inventoryData.SelectedIndex);
        }

        private int FindEmptySlot()
        {
            if (inventoryData == null)
                return -1;
                
            for (int i = 0; i < MAX_SLOTS; i++)
            {
                ItemInstance item = inventoryData.GetItem(i);
                if (item == null || item.itemData == null)
                    return i;
            }
            return -1;
        }
    }
}
