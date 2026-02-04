using UnityEngine;
using System;
using System.Collections.Generic;

namespace TheHunt.Inventory
{
    [CreateAssetMenu(fileName = "InventoryData", menuName = "Inventory/Inventory Data")]
    public class InventoryDataSO : ScriptableObject
    {
        [Header("Main Inventory")]
        [SerializeField] private ItemInstance[] items = new ItemInstance[InventorySystem.MAX_SLOTS];
        [SerializeField] private int selectedIndex = 0;
        
        [Header("Ammo Inventory")]
        [SerializeField] private List<AmmoData> ammoInventory = new List<AmmoData>();
        
        [Header("Weapon Equipment")]
        [SerializeField] private WeaponItemData primaryWeapon;
        [SerializeField] private WeaponItemData secondaryWeapon;
        
        public ItemInstance[] Items => items;
        
        public int SelectedIndex 
        { 
            get => selectedIndex; 
            set => selectedIndex = Mathf.Clamp(value, 0, InventorySystem.MAX_SLOTS - 1); 
        }
        
        public void SetItem(int index, ItemInstance item)
        {
            if (index >= 0 && index < items.Length)
            {
                items[index] = item;
            }
        }
        
        public ItemInstance GetItem(int index)
        {
            if (index >= 0 && index < items.Length)
                return items[index];
            return null;
        }
        
        public void SetAmmo(AmmoType type, int amount)
        {
            if (type == AmmoType.None)
                return;
                
            AmmoData ammo = ammoInventory.Find(a => a.type == type);
            if (ammo != null)
            {
                ammo.amount = Mathf.Max(0, amount);
            }
            else
            {
                ammoInventory.Add(new AmmoData { type = type, amount = Mathf.Max(0, amount) });
            }
        }
        
        public int GetAmmo(AmmoType type)
        {
            if (type == AmmoType.None)
                return 0;
                
            AmmoData ammo = ammoInventory.Find(a => a.type == type);
            return ammo != null ? ammo.amount : 0;
        }
        
        public WeaponItemData PrimaryWeapon 
        { 
            get => primaryWeapon; 
            set => primaryWeapon = value; 
        }
        
        public WeaponItemData SecondaryWeapon 
        { 
            get => secondaryWeapon; 
            set => secondaryWeapon = value; 
        }
        
        public void ResetInventory()
        {
            items = new ItemInstance[InventorySystem.MAX_SLOTS];
            selectedIndex = 0;
            
            foreach (var ammo in ammoInventory)
            {
                ammo.amount = 0;
            }
            
            primaryWeapon = null;
            secondaryWeapon = null;
            
            Debug.Log("<color=yellow>[INVENTORY DATA] Inventory reset to default state</color>");
        }
        
        public bool HasAnyItems()
        {
            if (items != null)
            {
                for (int i = 0; i < items.Length; i++)
                {
                    if (items[i] != null && items[i].itemData != null)
                        return true;
                }
            }
            
            if (ammoInventory != null)
            {
                foreach (var ammo in ammoInventory)
                {
                    if (ammo.amount > 0)
                        return true;
                }
            }
            
            if (primaryWeapon != null || secondaryWeapon != null)
                return true;
            
            return false;
        }
        
        #if UNITY_EDITOR
        private void OnValidate()
        {
            if (items == null || items.Length != InventorySystem.MAX_SLOTS)
            {
                items = new ItemInstance[InventorySystem.MAX_SLOTS];
            }
            
            if (ammoInventory == null || ammoInventory.Count == 0)
            {
                ammoInventory = new List<AmmoData>
                {
                    new AmmoData { type = AmmoType.Pistol_9mm, amount = 0 },
                    new AmmoData { type = AmmoType.Shotgun_Shell, amount = 0 },
                    new AmmoData { type = AmmoType.Rifle_762, amount = 0 },
                    new AmmoData { type = AmmoType.Special, amount = 0 }
                };
            }
            
            selectedIndex = Mathf.Clamp(selectedIndex, 0, InventorySystem.MAX_SLOTS - 1);
        }
        #endif
    }
    
    [Serializable]
    public class AmmoData
    {
        public AmmoType type;
        public int amount;
    }
}
