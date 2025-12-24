using UnityEngine;
using TheHunt.UI;
using TheHunt.Inventory;

namespace TheHunt.Interaction
{
    public class SimpleConfirmableInteraction : MonoBehaviour
    {
        [Header("References")]
        [SerializeField] private PlayerInteractionController interactionController;
        [SerializeField] private DialogService dialogService;
        
        [Header("Settings")]
        [SerializeField] private bool requireConfirmation = true;
        
        private IInteractable pendingInteractable;
        private GameObject pendingInteractor;
        
        void Awake()
        {
            if (interactionController == null)
                interactionController = GetComponent<PlayerInteractionController>();
            
            if (dialogService == null)
                dialogService = FindFirstObjectByType<DialogService>();
            
            if (dialogService == null)
            {
                Debug.LogError("<color=red>[SIMPLE CONFIRMABLE] DialogService not found in scene!</color>");
            }
        }
        
        public void RequestInteraction()
        {
            if (dialogService != null && dialogService.IsDialogOpen)
            {
                Debug.Log($"<color=yellow>[SIMPLE CONFIRMABLE] Dialog already open, ignoring interaction</color>");
                return;
            }
            
            if (interactionController == null)
            {
                Debug.LogWarning($"<color=yellow>[SIMPLE CONFIRMABLE] Missing interactionController</color>");
                return;
            }
            
            if (!interactionController.CanInteract)
            {
                Debug.Log($"<color=yellow>[SIMPLE CONFIRMABLE] Cannot interact</color>");
                return;
            }
            
            IInteractable interactable = interactionController.CurrentInteractable;
            
            if (interactable == null)
                return;
            
            if (!requireConfirmation)
            {
                interactionController.TryInteract();
                return;
            }
            
            ShowConfirmationDialog(interactable);
        }
        
        private void ShowConfirmationDialog(IInteractable interactable)
        {
            if (dialogService == null)
            {
                Debug.LogError("<color=red>[SIMPLE CONFIRMABLE] DialogService is null!</color>");
                return;
            }
            
            pendingInteractable = interactable;
            pendingInteractor = gameObject;
            
            string title = GetInteractionTitle(interactable);
            string description = GetInteractionDescription(interactable);
            
            dialogService.ShowConfirmation(title, description, OnConfirmed, OnCancelled);
            
            Debug.Log($"<color=cyan>[SIMPLE CONFIRMABLE] Showing confirmation for: {interactable.InteractionPrompt}</color>");
        }
        
        private void OnConfirmed()
        {
            if (pendingInteractable != null)
            {
                Debug.Log($"<color=green>[SIMPLE CONFIRMABLE] Interaction confirmed</color>");
                Debug.Log($"<color=green>[SIMPLE CONFIRMABLE] ===== CALLING Interact() =====</color>");
                pendingInteractable.Interact(pendingInteractor);
                Debug.Log($"<color=green>[SIMPLE CONFIRMABLE] ===== FINISHED Interact() =====</color>");
            }
            
            ClearPending();
        }
        
        private void OnCancelled()
        {
            Debug.Log($"<color=yellow>[SIMPLE CONFIRMABLE] Interaction cancelled - NOT picking up item</color>");
            ClearPending();
        }
        
        private void ClearPending()
        {
            pendingInteractable = null;
            pendingInteractor = null;
        }
        
        private string GetInteractionTitle(IInteractable interactable)
        {
            return interactable.InteractionPrompt;
        }
        
        private string GetInteractionDescription(IInteractable interactable)
        {
            MonoBehaviour interactableMono = interactable as MonoBehaviour;
            
            if (interactableMono == null)
                return "Do you want to proceed?";
            
            PickupItem pickup = interactableMono.GetComponent<PickupItem>();
            if (pickup != null)
            {
                return GetPickupDescription(pickup);
            }
            
            return "Do you want to interact with this object?";
        }
        
        private string GetPickupDescription(PickupItem pickup)
        {
            ItemData itemData = pickup?.ItemData;
            
            if (itemData == null)
                return "Do you want to pick up this item?";
            
            string description = itemData.Description;
            
            if (string.IsNullOrEmpty(description))
                return $"Pick up {itemData.ItemName}?";
            
            return description;
        }
        
        public void SetRequireConfirmation(bool require)
        {
            requireConfirmation = require;
        }
    }
}
