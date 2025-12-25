using UnityEngine;
using TheHunt.Inventory;

namespace TheHunt.Interaction
{
    public class LockedDoorInteraction : ConditionalInteraction
    {
        [Header("Lock Settings")]
        [SerializeField] private ItemData requiredKey;
        [SerializeField] private bool consumeKeyOnUse = false;
        
        [Header("Door Behavior")]
        [SerializeField] private Animator doorAnimator;
        [SerializeField] private string openAnimationTrigger = "Open";
        [SerializeField] private bool disableAfterOpen = true;
        
        private bool isOpen = false;
        
        protected override bool CheckCondition(GameObject interactor)
        {
            if (isOpen)
            {
                Debug.Log("<color=yellow>[LOCKED DOOR] Door is already open</color>");
                return false;
            }
            
            if (requiredKey == null)
            {
                Debug.LogWarning("[LOCKED DOOR] No key required - door will always open");
                return true;
            }
            
            InventorySystem inventory = interactor.GetComponent<InventorySystem>();
            
            if (inventory == null)
            {
                Debug.LogError("[LOCKED DOOR] Interactor does not have InventorySystem!");
                return false;
            }
            
            bool hasKey = inventory.HasItem(requiredKey);
            
            Debug.Log($"<color=cyan>[LOCKED DOOR] Player has key '{requiredKey.ItemName}': {hasKey}</color>");
            
            return hasKey;
        }
        
        protected override void HandleSuccess(GameObject interactor)
        {
            Debug.Log("<color=green>[LOCKED DOOR] Opening door!</color>");
            
            if (consumeKeyOnUse && requiredKey != null)
            {
                InventorySystem inventory = interactor.GetComponent<InventorySystem>();
                if (inventory != null)
                {
                    inventory.RemoveItem(requiredKey, 1);
                    Debug.Log($"<color=yellow>[LOCKED DOOR] Consumed key: {requiredKey.ItemName}</color>");
                }
            }
            
            OpenDoor();
            
            base.HandleSuccess(interactor);
        }
        
        private void OpenDoor()
        {
            isOpen = true;
            
            if (doorAnimator != null && !string.IsNullOrEmpty(openAnimationTrigger))
            {
                doorAnimator.SetTrigger(openAnimationTrigger);
                Debug.Log($"<color=cyan>[LOCKED DOOR] Triggered animation: {openAnimationTrigger}</color>");
            }
            
            if (disableAfterOpen)
            {
                enabled = false;
                Debug.Log("<color=yellow>[LOCKED DOOR] Interaction disabled</color>");
            }
        }
    }
}
