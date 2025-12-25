using System;
using UnityEngine;

namespace TheHunt.Inventory
{
    public class WeaponInventoryManager : MonoBehaviour
    {
        private const int EQUIPMENT_SLOTS = 2;

        [Header("Data Reference")]
        [SerializeField] private InventoryDataSO inventoryData;
        
        private InventorySystem inventorySystem;

        public WeaponItemData PrimaryWeapon => inventoryData != null ? inventoryData.PrimaryWeapon : null;
        public WeaponItemData SecondaryWeapon => inventoryData != null ? inventoryData.SecondaryWeapon : null;

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
            
            if (inventoryData == null)
            {
                Debug.LogError("<color=red>[WEAPON MANAGER] InventoryDataSO reference is missing! Please assign it in the Inspector.</color>");
            }
        }

        public void EquipWeapon(WeaponItemData weapon, EquipSlot slot)
        {
            if (weapon == null || inventoryData == null)
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
                if (inventoryData.PrimaryWeapon != null)
                    UnequipWeapon(EquipSlot.Primary);

                inventoryData.PrimaryWeapon = weapon;
            }
            else
            {
                if (inventoryData.SecondaryWeapon != null)
                    UnequipWeapon(EquipSlot.Secondary);

                inventoryData.SecondaryWeapon = weapon;
            }

            weapon.Equip(gameObject);
            OnWeaponEquipped?.Invoke(slot, weapon);
            
            Debug.Log($"<color=green>[WEAPON MANAGER] Equipped {weapon.ItemName} in {slot} slot</color>");
        }

        public void UnequipWeapon(EquipSlot slot)
        {
            if (inventoryData == null)
                return;
                
            WeaponItemData weapon = slot == EquipSlot.Primary ? inventoryData.PrimaryWeapon : inventoryData.SecondaryWeapon;

            if (weapon == null)
                return;

            weapon.Unequip(gameObject);

            if (slot == EquipSlot.Primary)
                inventoryData.PrimaryWeapon = null;
            else
                inventoryData.SecondaryWeapon = null;

            OnWeaponUnequipped?.Invoke(slot);
            
            Debug.Log($"<color=orange>[WEAPON MANAGER] Unequipped weapon from {slot} slot</color>");
        }

        public void SwapWeapons()
        {
            if (inventoryData == null)
                return;
                
            WeaponItemData temp = inventoryData.PrimaryWeapon;
            inventoryData.PrimaryWeapon = inventoryData.SecondaryWeapon;
            inventoryData.SecondaryWeapon = temp;

            if (inventoryData.PrimaryWeapon != null)
                OnWeaponEquipped?.Invoke(EquipSlot.Primary, inventoryData.PrimaryWeapon);
            if (inventoryData.SecondaryWeapon != null)
                OnWeaponEquipped?.Invoke(EquipSlot.Secondary, inventoryData.SecondaryWeapon);

            OnWeaponsSwapped?.Invoke();
            
            Debug.Log("<color=cyan>[WEAPON MANAGER] Swapped weapons</color>");
        }

        public WeaponItemData GetEquippedWeapon(EquipSlot slot)
        {
            if (inventoryData == null)
                return null;
                
            return slot == EquipSlot.Primary ? inventoryData.PrimaryWeapon : inventoryData.SecondaryWeapon;
        }

        public bool HasWeaponEquipped(EquipSlot slot)
        {
            return GetEquippedWeapon(slot) != null;
        }

        public bool IsWeaponEquipped(WeaponItemData weapon)
        {
            if (weapon == null || inventoryData == null)
                return false;
                
            return inventoryData.PrimaryWeapon == weapon || inventoryData.SecondaryWeapon == weapon;
        }

        public EquipSlot? GetWeaponSlot(WeaponItemData weapon)
        {
            if (weapon == null || inventoryData == null)
                return null;

            if (inventoryData.PrimaryWeapon == weapon)
                return EquipSlot.Primary;
            if (inventoryData.SecondaryWeapon == weapon)
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
