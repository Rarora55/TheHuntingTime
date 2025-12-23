using UnityEngine;
using TheHunt.UI;
using TheHunt.Inventory;

namespace TheHunt.Interaction
{
    public class ConfirmableInteraction : MonoBehaviour
    {
        [Header("References")]
        [SerializeField] private PlayerInteractionController interactionController;
        [SerializeField] private ConfirmationDialog confirmationDialog;
        
        [Header("Settings")]
        [SerializeField] private bool requireConfirmation = true;
        [SerializeField] private string yesButtonLabel = "Yes";
        [SerializeField] private string noButtonLabel = "No";
        
        private IInteractable pendingInteractable;
        private GameObject pendingInteractor;
        
        void Awake()
        {
            if (interactionController == null)
                interactionController = GetComponent<PlayerInteractionController>();
        }
        
        public void RequestInteraction()
        {
            if (interactionController == null || confirmationDialog == null)
            {
                Debug.LogWarning($"<color=yellow>[CONFIRMABLE] Missing references</color>");
                return;
            }
            
            if (!interactionController.CanInteract)
            {
                Debug.Log($"<color=yellow>[CONFIRMABLE] Cannot interact</color>");
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
            pendingInteractable = interactable;
            pendingInteractor = gameObject;
            
            string title = GetInteractionTitle(interactable);
            string description = GetInteractionDescription(interactable);
            
            confirmationDialog.SetButtonLabels(yesButtonLabel, noButtonLabel);
            confirmationDialog.Show(title, description, OnConfirmed, OnCancelled);
            
            Debug.Log($"<color=cyan>[CONFIRMABLE] Showing confirmation for: {interactable.InteractionPrompt}</color>");
        }
        
        private void OnConfirmed()
        {
            if (pendingInteractable != null)
            {
                Debug.Log($"<color=green>[CONFIRMABLE] Interaction confirmed</color>");
                pendingInteractable.Interact(pendingInteractor);
            }
            
            ClearPending();
        }
        
        private void OnCancelled()
        {
            Debug.Log($"<color=yellow>[CONFIRMABLE] Interaction cancelled</color>");
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
            ItemData itemData = GetItemDataFromPickup(pickup);
            
            if (itemData == null)
                return "Do you want to pick up this item?";
            
            string description = itemData.Description;
            
            if (string.IsNullOrEmpty(description))
                return $"Pick up {itemData.ItemName}?";
            
            return description;
        }
        
        private ItemData GetItemDataFromPickup(PickupItem pickup)
        {
            if (pickup == null)
                return null;
            
            var field = typeof(PickupItem).GetField("itemData", 
                System.Reflection.BindingFlags.NonPublic | 
                System.Reflection.BindingFlags.Instance);
            
            if (field != null)
                return field.GetValue(pickup) as ItemData;
            
            return null;
        }
        
        public void SetRequireConfirmation(bool require)
        {
            requireConfirmation = require;
        }
    }
}
