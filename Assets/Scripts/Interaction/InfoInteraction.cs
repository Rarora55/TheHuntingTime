using UnityEngine;
using TheHunt.UI;

namespace TheHunt.Interaction
{
    public class InfoInteraction : MonoBehaviour, IInteractable
    {
        [Header("Info Message")]
        [SerializeField] private string messageTitle = "Information";
        [SerializeField] [TextArea(3, 10)] private string messageText = "This is an informative message.";
        
        private DialogService dialogService;
        
        void Start()
        {
            dialogService = FindFirstObjectByType<DialogService>();
            
            if (dialogService == null)
            {
                Debug.LogError("[INFO INTERACTION] DialogService not found in scene!");
            }
        }
        
        public void Interact(GameObject interactor)
        {
            Debug.Log($"<color=cyan>[INFO INTERACTION] Showing info: {messageTitle}</color>");
            
            if (dialogService != null)
            {
                dialogService.ShowInfo(messageTitle, messageText, OnInfoClosed);
            }
            else
            {
                Debug.LogError("[INFO INTERACTION] DialogService is null!");
            }
        }
        
        private void OnInfoClosed()
        {
            Debug.Log("<color=green>[INFO INTERACTION] Info dialog closed by player</color>");
        }
    }
}
