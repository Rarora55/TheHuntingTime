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
        
        private InventoryDropHandler dropHandler;
        private AmmoInventoryQuery ammoQuery;
        private InventorySlotNavigator slotNavigator;

        public ItemInstance CurrentItem => inventoryData != null && inventoryData.SelectedIndex >= 0 && inventoryData.SelectedIndex < MAX_SLOTS 
            ? inventoryData.GetItem(inventoryData.SelectedIndex) 
            : null;
        public bool IsFull => FindEmptySlot() == -1;
        public bool HasSpace => !IsFull;
        public int SelectedSlot => slotNavigator != null ? slotNavigator.CurrentSlot : 0;
        public ItemInstance[] Items => inventoryData != null ? inventoryData.Items : new ItemInstance[MAX_SLOTS];
        public InventoryDataSO InventoryData => inventoryData;

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
                return;
            }
            
            InitializeComponents();
        }
        
        private void InitializeComponents()
        {
            dropHandler = new InventoryDropHandler(transform);
            ammoQuery = new AmmoInventoryQuery(inventoryData, OnAmmoRemovedFromSlot);
            slotNavigator = new InventorySlotNavigator(inventoryData);
            
            slotNavigator.OnSelectionChanged += (oldIndex, newIndex) => 
            {
                OnSelectionChanged?.Invoke(oldIndex, newIndex);
            };
        }
        
        private void OnAmmoRemovedFromSlot(int slotIndex, int quantity)
        {
            ItemInstance item = inventoryData.GetItem(slotIndex);
            OnItemRemoved?.Invoke(slotIndex, item);
        }

        public bool TryAddItem(ItemData itemData)
        {
            return TryAddItemWithMetadata(itemData, Vector3.one);
        }

        public bool TryAddItemWithMetadata(ItemData itemData, Vector3 originalScale)
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
                        item.quantity < itemData.MaxStackSize)
                    {
                        int spaceLeft = itemData.MaxStackSize - item.quantity;
                        int toAdd = Mathf.Min(quantityToAdd, spaceLeft);
                        
                        item.quantity += toAdd;
                        inventoryData.SetItem(i, item);
                        OnItemAdded?.Invoke(i, item);
                        
                        quantityToAdd -= toAdd;
                        
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
                    
                    return quantityToAdd < (itemData is AmmoItemData ammo ? ammo.AmmoAmount : 1);
                }

                int stackSize = itemData.IsStackable ? Mathf.Min(quantityToAdd, itemData.MaxStackSize) : 1;
                
                ItemInstance newItem = new ItemInstance(itemData, stackSize);
                
                // Guardar escala original en metadata
                newItem.metadata["originalScale"] = originalScale;
                
                inventoryData.SetItem(emptySlot, newItem);
                OnItemAdded?.Invoke(emptySlot, newItem);
                
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
            }
            else
            {
                inventoryData.SetItem(slotIndex, item);
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
                if (item != null && item.itemData != null && 
                    (item.itemData == itemData || item.itemData.ItemID == itemData.ItemID))
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
                if (item != null && item.itemData != null && 
                    (item.itemData == itemData || item.itemData.ItemID == itemData.ItemID))
                {
                    int toRemove = Mathf.Min(remaining, item.quantity);
                    RemoveItem(i, toRemove);
                    remaining -= toRemove;
                }
            }
            
            bool success = remaining == 0;
            
            return success;
        }

        public int GetAmmoCount(AmmoType ammoType)
        {
            return ammoQuery?.GetAmmoCount(ammoType) ?? 0;
        }

        public bool ConsumeAmmo(AmmoType ammoType, int amount)
        {
            return ammoQuery?.ConsumeAmmo(ammoType, amount) ?? false;
        }

        public void UseCurrentItem()
        {
            if (CurrentItem == null)
            {
                return;
            }

            if (CurrentItem.itemData is IUsable usable)
            {
                if (!usable.CanUse(gameObject))
                {
                    return;
                }

                CurrentItem.itemData.Use(gameObject);
                OnItemUsed?.Invoke(CurrentItem);

                if (usable.RemoveOnUse)
                {
                    RemoveItem(inventoryData.SelectedIndex, 1);
                }
            }
        }

        public void DropCurrentItem()
        {
            if (CurrentItem == null)
                return;

            ItemInstance droppedItem = CurrentItem;
            ItemData droppedItemData = droppedItem.itemData;
            
            Debug.Log($"<color=cyan>[INVENTORY] Dropped {droppedItemData.ItemName}</color>");
            
            RemoveItem(inventoryData.SelectedIndex, 1);
            
            dropHandler?.DropItem(droppedItemData, droppedItem);
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
            slotNavigator?.SelectNext();
        }

        public void SelectPrevious()
        {
            slotNavigator?.SelectPrevious();
        }

        public void SelectSlot(int index)
        {
            slotNavigator?.SelectSlot(index);
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
