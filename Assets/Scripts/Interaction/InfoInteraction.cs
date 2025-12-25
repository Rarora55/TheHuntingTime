using UnityEngine;
using TheHunt.UI;

namespace TheHunt.Interaction
{
    public class InfoInteraction : MonoBehaviour, IInteractable
    {
        [Header("Info Message")]
        [SerializeField] private string messageTitle = "Information";
        [SerializeField] [TextArea(3, 10)] private string messageText = "This is an informative message.";
        
        [Header("Interaction Settings")]
        [SerializeField] private string interactionPrompt = "Read";
        [SerializeField] private bool isInteractable = true;
        
        private DialogService dialogService;
        
        public string InteractionPrompt => interactionPrompt;
        public bool IsInteractable => isInteractable;
        
        void Start()
        {
            dialogService = FindFirstObjectByType<DialogService>();
            
            if (dialogService == null)
            {
                Debug.LogError("[INFO INTERACTION] DialogService not found in scene!");
            }
        }
        
        public bool CanInteract(GameObject interactor)
        {
            return isInteractable && dialogService != null;
        }
        
        public void Interact(GameObject interactor)
        {
            if (!CanInteract(interactor))
            {
                Debug.LogWarning("[INFO INTERACTION] Cannot interact - conditions not met");
                return;
            }
            
            Debug.Log($"<color=cyan>[INFO INTERACTION] Showing info: {messageTitle}</color>");
            dialogService.ShowInfo(messageTitle, messageText, OnInfoClosed);
        }
        
        private void OnInfoClosed()
        {
            Debug.Log("<color=green>[INFO INTERACTION] Info dialog closed by player</color>");
        }
    }
}
