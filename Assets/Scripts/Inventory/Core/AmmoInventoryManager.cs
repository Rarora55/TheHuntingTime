using System;
using System.Collections.Generic;
using UnityEngine;

namespace TheHunt.Inventory
{
    public class AmmoInventoryManager : MonoBehaviour
    {
        [Header("Data Reference")]
        [SerializeField] private InventoryDataSO inventoryData;

        public event Action<AmmoType, int> OnAmmoChanged;
        public event Action<AmmoType, int> OnAmmoAdded;
        public event Action<AmmoType, int> OnAmmoRemoved;
        public event Action<AmmoType> OnAmmoEmpty;
        
        void Awake()
        {
            if (inventoryData == null)
            {
                Debug.LogError("<color=red>[AMMO] InventoryDataSO reference is missing! Please assign it in the Inspector.</color>");
            }
        }

        public void AddAmmo(AmmoType type, int amount)
        {
            if (type == AmmoType.None || inventoryData == null)
                return;

            int current = inventoryData.GetAmmo(type);
            inventoryData.SetAmmo(type, current + amount);
            
            OnAmmoChanged?.Invoke(type, inventoryData.GetAmmo(type));
            OnAmmoAdded?.Invoke(type, amount);
            
            Debug.Log($"<color=green>[AMMO] Added {amount} {type}. Total: {inventoryData.GetAmmo(type)}</color>");
        }

        public bool RemoveAmmo(AmmoType type, int amount)
        {
            if (!HasAmmo(type, amount))
                return false;

            int current = inventoryData.GetAmmo(type);
            inventoryData.SetAmmo(type, current - amount);
            
            OnAmmoChanged?.Invoke(type, inventoryData.GetAmmo(type));
            OnAmmoRemoved?.Invoke(type, amount);
            
            if (inventoryData.GetAmmo(type) == 0)
            {
                OnAmmoEmpty?.Invoke(type);
            }
            
            Debug.Log($"<color=yellow>[AMMO] Removed {amount} {type}. Remaining: {inventoryData.GetAmmo(type)}</color>");
            return true;
        }

        public int GetAmmoCount(AmmoType type)
        {
            if (type == AmmoType.None || inventoryData == null)
                return 0;
                
            return inventoryData.GetAmmo(type);
        }

        public bool HasAmmo(AmmoType type, int required)
        {
            if (type == AmmoType.None)
                return true;
                
            if (inventoryData == null)
                return false;

            return inventoryData.GetAmmo(type) >= required;
        }

        public void SetAmmo(AmmoType type, int amount)
        {
            if (type == AmmoType.None || inventoryData == null)
                return;

            inventoryData.SetAmmo(type, Mathf.Max(0, amount));
            OnAmmoChanged?.Invoke(type, inventoryData.GetAmmo(type));
            
            Debug.Log($"<color=cyan>[AMMO] Set {type} to {inventoryData.GetAmmo(type)}</color>");
        }

        public Dictionary<AmmoType, int> GetAllAmmo()
        {
            Dictionary<AmmoType, int> allAmmo = new Dictionary<AmmoType, int>
            {
                { AmmoType.Pistol_9mm, GetAmmoCount(AmmoType.Pistol_9mm) },
                { AmmoType.Shotgun_Shell, GetAmmoCount(AmmoType.Shotgun_Shell) },
                { AmmoType.Rifle_762, GetAmmoCount(AmmoType.Rifle_762) },
                { AmmoType.Special, GetAmmoCount(AmmoType.Special) }
            };
            
            return allAmmo;
        }

        public int GetTotalAmmoCount()
        {
            if (inventoryData == null)
                return 0;
                
            int total = 0;
            total += inventoryData.GetAmmo(AmmoType.Pistol_9mm);
            total += inventoryData.GetAmmo(AmmoType.Shotgun_Shell);
            total += inventoryData.GetAmmo(AmmoType.Rifle_762);
            total += inventoryData.GetAmmo(AmmoType.Special);
            
            return total;
        }
    }
}
