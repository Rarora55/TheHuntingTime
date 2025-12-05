using System;
using System.Collections.Generic;
using UnityEngine;

namespace TheHunt.Inventory
{
    public class InventorySystem : MonoBehaviour
    {
        public const int MAX_SLOTS = 6;
        public const int MAX_STACK_SIZE = 6;
        public const int EQUIPMENT_SLOTS = 2;

        private ItemInstance[] items = new ItemInstance[MAX_SLOTS];
        private int selectedIndex = 0;
        private WeaponItemData primaryWeapon;
        private WeaponItemData secondaryWeapon;
        private Dictionary<AmmoType, int> ammoInventory = new Dictionary<AmmoType, int>
        {
            { AmmoType.Pistol_9mm, 0 },
            { AmmoType.Shotgun_Shell, 0 },
            { AmmoType.Rifle_762, 0 },
            { AmmoType.Special, 0 }
        };

        public ItemInstance CurrentItem => selectedIndex >= 0 && selectedIndex < MAX_SLOTS ? items[selectedIndex] : null;
        public bool IsFull => FindEmptySlot() == -1;
        public bool HasSpace => !IsFull;
        public int SelectedSlot => selectedIndex;
        public WeaponItemData PrimaryWeapon => primaryWeapon;
        public WeaponItemData SecondaryWeapon => secondaryWeapon;
        public ItemInstance[] Items => items;

        public event Action<int, ItemInstance> OnItemAdded;
        public event Action<int, ItemInstance> OnItemRemoved;
        public event Action<ItemInstance> OnItemUsed;
        public event Action<int, int> OnSelectionChanged;
        public event Action OnInventoryFull;
        public event Action<EquipSlot, WeaponItemData> OnWeaponEquipped;
        public event Action<EquipSlot> OnWeaponUnequipped;
        public event Action<AmmoType, int> OnAmmoChanged;

        public bool TryAddItem(ItemData itemData)
        {
            if (itemData == null)
            {
                Debug.LogWarning("[INVENTORY] Cannot add null item");
                return false;
            }

            if (itemData is AmmoItemData ammoData)
            {
                AddAmmo(ammoData.AmmoType, ammoData.AmmoAmount);
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

        public void EquipWeapon(WeaponItemData weapon, EquipSlot slot)
        {
            if (weapon == null)
                return;

            bool hasWeapon = false;
            for (int i = 0; i < MAX_SLOTS; i++)
            {
                if (items[i] != null && items[i].itemData == weapon)
                {
                    hasWeapon = true;
                    break;
                }
            }

            if (!hasWeapon)
            {
                Debug.LogWarning("[INVENTORY] Cannot equip weapon not in inventory");
                return;
            }

            if (slot == EquipSlot.Primary)
            {
                if (primaryWeapon != null)
                    UnequipWeapon(EquipSlot.Primary);

                primaryWeapon = weapon;
            }
            else
            {
                if (secondaryWeapon != null)
                    UnequipWeapon(EquipSlot.Secondary);

                secondaryWeapon = weapon;
            }

            weapon.Equip(gameObject);
            OnWeaponEquipped?.Invoke(slot, weapon);
            Debug.Log($"<color=green>[INVENTORY] Equipped {weapon.ItemName} in {slot} slot</color>");
        }

        public void UnequipWeapon(EquipSlot slot)
        {
            WeaponItemData weapon = slot == EquipSlot.Primary ? primaryWeapon : secondaryWeapon;

            if (weapon == null)
                return;

            weapon.Unequip(gameObject);

            if (slot == EquipSlot.Primary)
                primaryWeapon = null;
            else
                secondaryWeapon = null;

            OnWeaponUnequipped?.Invoke(slot);
            Debug.Log($"<color=orange>[INVENTORY] Unequipped weapon from {slot} slot</color>");
        }

        public void SwapWeapons()
        {
            WeaponItemData temp = primaryWeapon;
            primaryWeapon = secondaryWeapon;
            secondaryWeapon = temp;

            if (primaryWeapon != null)
                OnWeaponEquipped?.Invoke(EquipSlot.Primary, primaryWeapon);
            if (secondaryWeapon != null)
                OnWeaponEquipped?.Invoke(EquipSlot.Secondary, secondaryWeapon);

            Debug.Log("<color=cyan>[INVENTORY] Swapped weapons</color>");
        }

        public WeaponItemData GetEquippedWeapon(EquipSlot slot)
        {
            return slot == EquipSlot.Primary ? primaryWeapon : secondaryWeapon;
        }

        public void AddAmmo(AmmoType type, int amount)
        {
            if (type == AmmoType.None)
                return;

            ammoInventory[type] += amount;
            OnAmmoChanged?.Invoke(type, ammoInventory[type]);
            Debug.Log($"<color=green>[AMMO] Added {amount} {type}. Total: {ammoInventory[type]}</color>");
        }

        public bool RemoveAmmo(AmmoType type, int amount)
        {
            if (!HasAmmo(type, amount))
                return false;

            ammoInventory[type] -= amount;
            OnAmmoChanged?.Invoke(type, ammoInventory[type]);
            return true;
        }

        public int GetAmmoCount(AmmoType type)
        {
            return type == AmmoType.None ? 0 : ammoInventory[type];
        }

        public bool HasAmmo(AmmoType type, int required)
        {
            if (type == AmmoType.None)
                return true;

            return ammoInventory[type] >= required;
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
