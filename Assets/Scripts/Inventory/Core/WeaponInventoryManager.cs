using System;
using UnityEngine;

namespace TheHunt.Inventory
{
    public class WeaponInventoryManager : MonoBehaviour
    {
        private const int EQUIPMENT_SLOTS = 2;

        private InventorySystem inventorySystem;
        private WeaponItemData primaryWeapon;
        private WeaponItemData secondaryWeapon;

        public WeaponItemData PrimaryWeapon => primaryWeapon;
        public WeaponItemData SecondaryWeapon => secondaryWeapon;

        public event Action<EquipSlot, WeaponItemData> OnWeaponEquipped;
        public event Action<EquipSlot> OnWeaponUnequipped;
        public event Action OnWeaponsSwapped;

        void Awake()
        {
            inventorySystem = GetComponent<InventorySystem>();
            
            if (inventorySystem == null)
            {
                Debug.LogError("<color=red>[WEAPON MANAGER] InventorySystem component not found!</color>");
            }
        }

        public void EquipWeapon(WeaponItemData weapon, EquipSlot slot)
        {
            if (weapon == null)
                return;

            if (inventorySystem == null)
            {
                Debug.LogError("<color=red>[WEAPON MANAGER] InventorySystem not initialized!</color>");
                return;
            }

            if (!IsWeaponInInventory(weapon))
            {
                Debug.LogWarning($"<color=yellow>[WEAPON MANAGER] Cannot equip {weapon.ItemName} - not in inventory</color>");
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
            
            Debug.Log($"<color=green>[WEAPON MANAGER] Equipped {weapon.ItemName} in {slot} slot</color>");
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
            
            Debug.Log($"<color=orange>[WEAPON MANAGER] Unequipped weapon from {slot} slot</color>");
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

            OnWeaponsSwapped?.Invoke();
            
            Debug.Log("<color=cyan>[WEAPON MANAGER] Swapped weapons</color>");
        }

        public WeaponItemData GetEquippedWeapon(EquipSlot slot)
        {
            return slot == EquipSlot.Primary ? primaryWeapon : secondaryWeapon;
        }

        public bool HasWeaponEquipped(EquipSlot slot)
        {
            return GetEquippedWeapon(slot) != null;
        }

        public bool IsWeaponEquipped(WeaponItemData weapon)
        {
            return weapon != null && (primaryWeapon == weapon || secondaryWeapon == weapon);
        }

        public EquipSlot? GetWeaponSlot(WeaponItemData weapon)
        {
            if (weapon == null)
                return null;

            if (primaryWeapon == weapon)
                return EquipSlot.Primary;
            if (secondaryWeapon == weapon)
                return EquipSlot.Secondary;

            return null;
        }

        bool IsWeaponInInventory(WeaponItemData weapon)
        {
            if (inventorySystem == null)
                return false;

            ItemInstance[] items = inventorySystem.Items;

            for (int i = 0; i < InventorySystem.MAX_SLOTS; i++)
            {
                if (items[i] != null && items[i].itemData == weapon)
                {
                    return true;
                }
            }

            return false;
        }
    }
}
