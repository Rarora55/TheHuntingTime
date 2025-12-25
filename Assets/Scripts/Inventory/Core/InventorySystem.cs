using System;
using System.Collections.Generic;
using UnityEngine;

namespace TheHunt.Inventory
{
    public class InventorySystem : MonoBehaviour
    {
        public const int MAX_SLOTS = 6;
        public const int MAX_STACK_SIZE = 6;

        private ItemInstance[] items = new ItemInstance[MAX_SLOTS];
        private int selectedIndex = 0;

        private AmmoInventoryManager ammoManager;
        private WeaponInventoryManager weaponManager;

        public ItemInstance CurrentItem => selectedIndex >= 0 && selectedIndex < MAX_SLOTS ? items[selectedIndex] : null;
        public bool IsFull => FindEmptySlot() == -1;
        public bool HasSpace => !IsFull;
        public int SelectedSlot => selectedIndex;
        public ItemInstance[] Items => items;

        public event Action<int, ItemInstance> OnItemAdded;
        public event Action<int, ItemInstance> OnItemRemoved;
        public event Action<ItemInstance> OnItemUsed;
        public event Action<int, int> OnSelectionChanged;
        public event Action OnInventoryFull;

        void Awake()
        {
            ammoManager = GetComponent<AmmoInventoryManager>();
            weaponManager = GetComponent<WeaponInventoryManager>();
        }

        public bool TryAddItem(ItemData itemData)
        {
            if (itemData == null)
            {
                Debug.LogWarning("[INVENTORY] Cannot add null item");
                return false;
            }

            if (itemData is AmmoItemData ammoData)
            {
                if (ammoManager != null)
                {
                    ammoManager.AddAmmo(ammoData.AmmoType, ammoData.AmmoAmount);
                }
                else
                {
                    Debug.LogWarning("<color=yellow>[INVENTORY] AmmoInventoryManager not found!</color>");
                }
                return true;
            }

            if (itemData.IsStackable)
            {
                for (int i = 0; i < MAX_SLOTS; i++)
                {
                    if (items[i] != null &&
                        items[i].itemData == itemData &&
                        items[i].quantity < MAX_STACK_SIZE)
                    {
                        items[i].quantity++;
                        OnItemAdded?.Invoke(i, items[i]);
                        Debug.Log($"<color=green>[INVENTORY] Stacked {itemData.ItemName}. Total: {items[i].quantity}</color>");
                        return true;
                    }
                }
            }

            int emptySlot = FindEmptySlot();
            if (emptySlot == -1)
            {
                OnInventoryFull?.Invoke();
                Debug.Log("<color=yellow>[INVENTORY] Inventory is full!</color>");
                return false;
            }

            items[emptySlot] = new ItemInstance(itemData, 1);
            OnItemAdded?.Invoke(emptySlot, items[emptySlot]);
            Debug.Log($"<color=green>[INVENTORY] Added {itemData.ItemName} to slot {emptySlot}</color>");

            return true;
        }

        public void RemoveItem(int slotIndex, int quantity = 1)
        {
            if (slotIndex < 0 || slotIndex >= MAX_SLOTS || items[slotIndex] == null)
                return;

            ItemInstance item = items[slotIndex];
            item.quantity -= quantity;

            if (item.quantity <= 0)
            {
                items[slotIndex] = null;
                Debug.Log($"<color=orange>[INVENTORY] Removed {item.itemData.ItemName} completely from slot {slotIndex}</color>");
            }
            else
            {
                Debug.Log($"<color=orange>[INVENTORY] Removed {quantity} {item.itemData.ItemName}. Remaining: {item.quantity}</color>");
            }

            OnItemRemoved?.Invoke(slotIndex, item);
        }
        
        public bool HasItem(ItemData itemData)
        {
            if (itemData == null)
                return false;
            
            for (int i = 0; i < MAX_SLOTS; i++)
            {
                if (items[i] != null && items[i].itemData == itemData)
                {
                    return true;
                }
            }
            
            return false;
        }
        
        public int GetItemCount(ItemData itemData)
        {
            if (itemData == null)
                return 0;
            
            int count = 0;
            for (int i = 0; i < MAX_SLOTS; i++)
            {
                if (items[i] != null && items[i].itemData == itemData)
                {
                    count += items[i].quantity;
                }
            }
            
            return count;
        }
        
        public bool RemoveItem(ItemData itemData, int quantity = 1)
        {
            if (itemData == null || quantity <= 0)
                return false;
            
            int remaining = quantity;
            
            for (int i = 0; i < MAX_SLOTS && remaining > 0; i++)
            {
                if (items[i] != null && items[i].itemData == itemData)
                {
                    int toRemove = Mathf.Min(remaining, items[i].quantity);
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
                    RemoveItem(selectedIndex, 1);
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
            
            RemoveItem(selectedIndex, 1);
            
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
            int oldIndex = selectedIndex;
            selectedIndex = (selectedIndex + 1) % MAX_SLOTS;
            OnSelectionChanged?.Invoke(oldIndex, selectedIndex);
            Debug.Log($"<color=cyan>[INVENTORY] Selected slot {selectedIndex}</color>");
        }

        public void SelectPrevious()
        {
            int oldIndex = selectedIndex;
            selectedIndex--;
            if (selectedIndex < 0)
                selectedIndex = MAX_SLOTS - 1;
            OnSelectionChanged?.Invoke(oldIndex, selectedIndex);
            Debug.Log($"<color=cyan>[INVENTORY] Selected slot {selectedIndex}</color>");
        }

        public void SelectSlot(int index)
        {
            if (index < 0 || index >= MAX_SLOTS)
                return;

            int oldIndex = selectedIndex;
            selectedIndex = index;
            OnSelectionChanged?.Invoke(oldIndex, selectedIndex);
        }

        private int FindEmptySlot()
        {
            for (int i = 0; i < MAX_SLOTS; i++)
            {
                if (items[i] == null)
                    return i;
            }
            return -1;
        }
    }
}
