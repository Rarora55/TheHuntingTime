using UnityEngine;

namespace TheHunt.Interaction
{
    [RequireComponent(typeof(Collider2D))]
    public class PickupInteractable : InteractableObject
    {
        [Header("Pickup Settings")]
        [SerializeField] private Inventory.ItemData itemData;
        [SerializeField] private string itemName = "Item";
        [SerializeField] private bool destroyOnPickup = true;
        
        [Header("Feedback")]
        [SerializeField] private GameObject pickupVFX;
        [SerializeField] private AudioClip pickupSound;
        
        void Awake()
        {
            if (itemData != null && string.IsNullOrEmpty(itemName))
            {
                itemName = itemData.ItemName;
            }
            
            interactionPrompt = $"Press E to pick up {itemName}";
            
            Collider2D col = GetComponent<Collider2D>();
            if (col != null)
            {
                col.isTrigger = true;
            }
        }
        
        protected override void OnInteract(GameObject interactor)
        {
            bool addedToInventory = AddToInventory(interactor);
            
            if (addedToInventory)
            {
                PlayFeedback();
                
                if (destroyOnPickup)
                {
                    Destroy(gameObject);
                }
                else
                {
                    SetInteractable(false);
                    gameObject.SetActive(false);
                }
            }
        }
        
        bool AddToInventory(GameObject interactor)
        {
            Inventory.InventorySystem inventory = interactor.GetComponent<Inventory.InventorySystem>();
            
            if (inventory == null)
            {
                Debug.LogError($"<color=red>[PICKUP] {interactor.name} has no InventorySystem component!</color>");
                return false;
            }
            
            if (itemData == null)
            {
                Debug.LogError($"<color=red>[PICKUP] {gameObject.name} has no ItemData assigned!</color>");
                return false;
            }
            
            bool added = inventory.TryAddItem(itemData);
            
            if (added)
            {
                Debug.Log($"<color=green>[PICKUP] {interactor.name} picked up {itemName}</color>");
            }
            else
            {
                Debug.Log($"<color=yellow>[PICKUP] Could not add {itemName} to inventory (full?)</color>");
            }
            
            return added;
        }
        
        void PlayFeedback()
        {
            if (pickupVFX != null)
            {
                Instantiate(pickupVFX, transform.position, Quaternion.identity);
            }
            
            if (pickupSound != null)
            {
                AudioSource.PlayClipAtPoint(pickupSound, transform.position);
            }
        }
    }
}
