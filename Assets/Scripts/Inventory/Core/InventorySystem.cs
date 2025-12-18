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

            Debug.Log($"<color=cyan>[INVENTORY] Dropped {CurrentItem.itemData.ItemName}</color>");
            RemoveItem(selectedIndex, 1);
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

            Debug.Log($"<color=cyan>[EXAMINE] {CurrentItem.itemData.ItemName}\n{CurrentItem.itemData.ExaminationText}</color>");
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
