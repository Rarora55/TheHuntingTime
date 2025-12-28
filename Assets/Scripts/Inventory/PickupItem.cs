using UnityEngine;
using TheHunt.Interaction;

namespace TheHunt.Inventory
{
    public class PickupItem : MonoBehaviour, IInteractable
    {
        [SerializeField] private ItemData itemData;
        [SerializeField] private string interactionPrompt = "Pick up";
        [SerializeField] private bool destroyOnPickup = true;

        public bool IsInteractable => itemData != null;
        public string InteractionPrompt => $"{interactionPrompt} {itemData?.ItemName}";
        public ItemData ItemData => itemData;

        public bool CanInteract(GameObject interactor)
        {
            Debug.Log($"<color=cyan>[PICKUP ITEM] CanInteract called for {itemData?.ItemName}</color>");
            
            InventorySystem inventory = interactor.GetComponent<InventorySystem>();
            if (inventory == null)
            {
                Debug.LogWarning($"<color=yellow>[PICKUP ITEM] Interactor does not have InventorySystem</color>");
                return false;
            }

            Debug.Log($"<color=green>[PICKUP ITEM] Returning TRUE - Let TryAddItem decide if it can be added</color>");
            return true;
        }

        public void Interact(GameObject interactor)
        {
            InventorySystem inventory = interactor.GetComponent<InventorySystem>();
            if (inventory == null)
            {
                Debug.LogWarning("[PICKUP] Interactor does not have InventorySystem");
                return;
            }

            if (inventory.TryAddItem(itemData))
            {
                Debug.Log($"<color=green>[PICKUP] Picked up {itemData.ItemName}</color>");

                if (destroyOnPickup)
                {
                    Destroy(gameObject);
                }
            }
            else
            {
                Debug.Log($"<color=yellow>[PICKUP] Cannot pick up {itemData.ItemName} - Inventory full</color>");
            }
        }
    }
}
