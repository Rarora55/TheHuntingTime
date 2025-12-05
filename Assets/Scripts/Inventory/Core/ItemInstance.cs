using System;
using System.Collections.Generic;

namespace TheHunt.Inventory
{
    [Serializable]
    public class ItemInstance
    {
        public ItemData itemData;
        public int quantity;
        public string instanceID;
        public Dictionary<string, object> metadata;

        public bool CanStack => itemData.IsStackable && quantity < InventorySystem.MAX_STACK_SIZE;
        public bool IsFull => quantity >= InventorySystem.MAX_STACK_SIZE;
        public string DisplayName => itemData.IsStackable ? $"{itemData.ItemName} x{quantity}" : itemData.ItemName;

        public ItemInstance(ItemData data, int amount = 1)
        {
            itemData = data;
            quantity = amount;
            instanceID = Guid.NewGuid().ToString();
            metadata = new Dictionary<string, object>();
        }
    }
}
