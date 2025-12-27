using UnityEngine;

namespace TheHunt.Inventory
{
    public class AmmoInventoryQuery
    {
        private readonly InventoryDataSO inventoryData;
        private readonly System.Action<int, int> onItemRemovedCallback;

        public AmmoInventoryQuery(InventoryDataSO inventoryData, System.Action<int, int> onItemRemovedCallback = null)
        {
            this.inventoryData = inventoryData;
            this.onItemRemovedCallback = onItemRemovedCallback;
        }

        public int GetAmmoCount(AmmoType ammoType)
        {
            if (inventoryData == null || ammoType == AmmoType.None)
                return 0;

            int totalAmmo = 0;

            for (int i = 0; i < InventorySystem.MAX_SLOTS; i++)
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

        public bool HasEnoughAmmo(AmmoType ammoType, int amount)
        {
            return GetAmmoCount(ammoType) >= amount;
        }

        public bool ConsumeAmmo(AmmoType ammoType, int amount)
        {
            if (inventoryData == null || ammoType == AmmoType.None || amount <= 0)
                return false;

            int totalAvailable = GetAmmoCount(ammoType);

            if (totalAvailable < amount)
            {
                Debug.LogWarning($"<color=yellow>[AMMO QUERY] Not enough {ammoType} ammo. Need: {amount}, Have: {totalAvailable}</color>");
                return false;
            }

            int remaining = amount;

            for (int i = 0; i < InventorySystem.MAX_SLOTS && remaining > 0; i++)
            {
                ItemInstance item = inventoryData.GetItem(i);
                if (item != null && item.itemData is AmmoItemData ammoData)
                {
                    if (ammoData.AmmoType == ammoType)
                    {
                        int toRemove = Mathf.Min(remaining, item.quantity);
                        RemoveAmmoFromSlot(i, item, toRemove);
                        remaining -= toRemove;
                    }
                }
            }

            Debug.Log($"<color=green>[AMMO QUERY] Consumed {amount} {ammoType} ammo. Remaining: {GetAmmoCount(ammoType)}</color>");
            return true;
        }

        private void RemoveAmmoFromSlot(int slotIndex, ItemInstance item, int quantity)
        {
            item.quantity -= quantity;

            if (item.quantity <= 0)
            {
                inventoryData.SetItem(slotIndex, null);
                Debug.Log($"<color=orange>[AMMO QUERY] Removed {item.itemData.ItemName} completely from slot {slotIndex}</color>");
            }
            else
            {
                inventoryData.SetItem(slotIndex, item);
                Debug.Log($"<color=orange>[AMMO QUERY] Removed {quantity} {item.itemData.ItemName}. Remaining: {item.quantity}</color>");
            }

            onItemRemovedCallback?.Invoke(slotIndex, quantity);
        }
    }
}
