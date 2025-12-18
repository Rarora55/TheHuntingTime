using System;
using System.Collections.Generic;
using UnityEngine;

namespace TheHunt.Inventory
{
    public class AmmoInventoryManager : MonoBehaviour
    {
        private Dictionary<AmmoType, int> ammoInventory = new Dictionary<AmmoType, int>
        {
            { AmmoType.Pistol_9mm, 0 },
            { AmmoType.Shotgun_Shell, 0 },
            { AmmoType.Rifle_762, 0 },
            { AmmoType.Special, 0 }
        };

        public event Action<AmmoType, int> OnAmmoChanged;
        public event Action<AmmoType, int> OnAmmoAdded;
        public event Action<AmmoType, int> OnAmmoRemoved;
        public event Action<AmmoType> OnAmmoEmpty;

        public void AddAmmo(AmmoType type, int amount)
        {
            if (type == AmmoType.None)
                return;

            ammoInventory[type] += amount;
            
            OnAmmoChanged?.Invoke(type, ammoInventory[type]);
            OnAmmoAdded?.Invoke(type, amount);
            
            Debug.Log($"<color=green>[AMMO] Added {amount} {type}. Total: {ammoInventory[type]}</color>");
        }

        public bool RemoveAmmo(AmmoType type, int amount)
        {
            if (!HasAmmo(type, amount))
                return false;

            ammoInventory[type] -= amount;
            
            OnAmmoChanged?.Invoke(type, ammoInventory[type]);
            OnAmmoRemoved?.Invoke(type, amount);
            
            if (ammoInventory[type] == 0)
            {
                OnAmmoEmpty?.Invoke(type);
            }
            
            Debug.Log($"<color=yellow>[AMMO] Removed {amount} {type}. Remaining: {ammoInventory[type]}</color>");
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

        public void SetAmmo(AmmoType type, int amount)
        {
            if (type == AmmoType.None)
                return;

            ammoInventory[type] = Mathf.Max(0, amount);
            OnAmmoChanged?.Invoke(type, ammoInventory[type]);
            
            Debug.Log($"<color=cyan>[AMMO] Set {type} to {ammoInventory[type]}</color>");
        }

        public Dictionary<AmmoType, int> GetAllAmmo()
        {
            return new Dictionary<AmmoType, int>(ammoInventory);
        }

        public int GetTotalAmmoCount()
        {
            int total = 0;
            foreach (var ammo in ammoInventory.Values)
            {
                total += ammo;
            }
            return total;
        }
    }
}
