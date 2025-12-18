using System;
using UnityEngine;

namespace TheHunt.Inventory
{
    public class KeyInventoryManager : MonoBehaviour
    {
        private InventorySystem inventorySystem;

        public event Action<string, KeyItemData> OnKeyFound;
        public event Action<string, KeyItemData> OnKeyConsumed;

        void Awake()
        {
            inventorySystem = GetComponent<InventorySystem>();
            
            if (inventorySystem == null)
            {
                Debug.LogError("<color=red>[KEY MANAGER] InventorySystem component not found!</color>");
            }
        }

        public bool HasKeyItem(string keyID)
        {
            if (inventorySystem == null)
            {
                Debug.LogError("<color=red>[KEY MANAGER] InventorySystem not initialized!</color>");
                return false;
            }

            ItemInstance[] items = inventorySystem.Items;

            for (int i = 0; i < InventorySystem.MAX_SLOTS; i++)
            {
                if (items[i] != null && items[i].itemData is KeyItemData keyData)
                {
                    if (keyData.Unlocks != null)
                    {
                        foreach (string unlockID in keyData.Unlocks)
                        {
                            if (unlockID == keyID)
                            {
                                Debug.Log($"<color=cyan>[KEY MANAGER] Found key for '{keyID}': {keyData.ItemName}</color>");
                                OnKeyFound?.Invoke(keyID, keyData);
                                return true;
                            }
                        }
                    }
                }
            }

            Debug.Log($"<color=yellow>[KEY MANAGER] No key found for '{keyID}'</color>");
            return false;
        }

        public bool ConsumeKeyItem(string keyID)
        {
            if (inventorySystem == null)
            {
                Debug.LogError("<color=red>[KEY MANAGER] InventorySystem not initialized!</color>");
                return false;
            }

            ItemInstance[] items = inventorySystem.Items;

            for (int i = 0; i < InventorySystem.MAX_SLOTS; i++)
            {
                if (items[i] != null && items[i].itemData is KeyItemData keyData)
                {
                    if (keyData.Unlocks != null)
                    {
                        foreach (string unlockID in keyData.Unlocks)
                        {
                            if (unlockID == keyID)
                            {
                                inventorySystem.RemoveItem(i, 1);
                                
                                Debug.Log($"<color=green>[KEY MANAGER] Consumed key: {keyData.ItemName}</color>");
                                OnKeyConsumed?.Invoke(keyID, keyData);
                                return true;
                            }
                        }
                    }
                }
            }

            Debug.Log($"<color=red>[KEY MANAGER] Could not consume key '{keyID}' (not found)</color>");
            return false;
        }

        public KeyItemData GetKeyData(string keyID)
        {
            if (inventorySystem == null)
                return null;

            ItemInstance[] items = inventorySystem.Items;

            for (int i = 0; i < InventorySystem.MAX_SLOTS; i++)
            {
                if (items[i] != null && items[i].itemData is KeyItemData keyData)
                {
                    if (keyData.Unlocks != null)
                    {
                        foreach (string unlockID in keyData.Unlocks)
                        {
                            if (unlockID == keyID)
                            {
                                return keyData;
                            }
                        }
                    }
                }
            }

            return null;
        }

        public int GetKeyCount()
        {
            if (inventorySystem == null)
                return 0;

            ItemInstance[] items = inventorySystem.Items;
            int count = 0;

            for (int i = 0; i < InventorySystem.MAX_SLOTS; i++)
            {
                if (items[i] != null && items[i].itemData is KeyItemData)
                {
                    count++;
                }
            }

            return count;
        }
    }
}
