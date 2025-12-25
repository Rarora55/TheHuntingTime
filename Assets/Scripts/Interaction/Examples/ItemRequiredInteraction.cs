using UnityEngine;
using TheHunt.Inventory;

namespace TheHunt.Interaction.Examples
{
    public class ItemRequiredInteraction : ConditionalInteraction
    {
        [Header("Item Requirement")]
        [SerializeField] private ItemData requiredItem;
        [SerializeField] private int requiredQuantity = 1;
        [SerializeField] private bool consumeItemOnUse = false;
        
        protected override bool CheckCondition(GameObject interactor)
        {
            if (requiredItem == null)
            {
                Debug.LogWarning($"[ITEM REQUIRED] No item required on {gameObject.name}");
                return true;
            }
            
            InventorySystem inventory = interactor.GetComponent<InventorySystem>();
            
            if (inventory == null)
            {
                Debug.LogError("[ITEM REQUIRED] Interactor does not have InventorySystem!");
                return false;
            }
            
            int count = inventory.GetItemCount(requiredItem);
            bool hasEnough = count >= requiredQuantity;
            
            Debug.Log($"<color=cyan>[ITEM REQUIRED] Player has {count}/{requiredQuantity} of '{requiredItem.ItemName}': {hasEnough}</color>");
            
            return hasEnough;
        }
        
        protected override void HandleSuccess(GameObject interactor)
        {
            if (consumeItemOnUse && requiredItem != null)
            {
                InventorySystem inventory = interactor.GetComponent<InventorySystem>();
                if (inventory != null)
                {
                    bool removed = inventory.RemoveItem(requiredItem, requiredQuantity);
                    if (removed)
                    {
                        Debug.Log($"<color=yellow>[ITEM REQUIRED] Consumed {requiredQuantity}x {requiredItem.ItemName}</color>");
                    }
                }
            }
            
            base.HandleSuccess(interactor);
        }
    }
}
