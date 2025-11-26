using UnityEngine;

namespace TheHunt.Interaction
{
    [RequireComponent(typeof(Collider2D))]
    public class PickupInteractable : InteractableObject
    {
        [Header("Pickup Settings")]
        [SerializeField] private GameObject itemPrefab;
        [SerializeField] private string itemName = "Item";
        [SerializeField] private bool destroyOnPickup = true;
        
        [Header("Feedback")]
        [SerializeField] private GameObject pickupVFX;
        [SerializeField] private AudioClip pickupSound;
        
        void Awake()
        {
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
            Debug.Log($"<color=green>[PICKUP] {interactor.name} picked up {itemName}</color>");
            return true;
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
