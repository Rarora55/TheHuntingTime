using UnityEngine;
using UnityEngine.UI;
using TMPro;
using TheHunt.Interaction;

namespace TheHunt.UI
{
    public class InteractionPromptUI : MonoBehaviour
    {
        [Header("References")]
        [SerializeField] private PlayerInteractionController interactionController;
        [SerializeField] private TextMeshProUGUI promptText;
        [SerializeField] private GameObject promptPanel;
        
        void Awake()
        {
            if (interactionController == null)
            {
                interactionController = FindFirstObjectByType<PlayerInteractionController>();
            }
            
            if (interactionController == null)
            {
                Debug.LogError("<color=red>[INTERACTION PROMPT UI] No PlayerInteractionController found!</color>");
                return;
            }
            
            interactionController.OnInteractableDetected += ShowPrompt;
            interactionController.OnInteractableCleared += HidePrompt;
            
            HidePrompt();
        }
        
        void OnDestroy()
        {
            if (interactionController != null)
            {
                interactionController.OnInteractableDetected -= ShowPrompt;
                interactionController.OnInteractableCleared -= HidePrompt;
            }
        }
        
        void ShowPrompt(IInteractable interactable)
        {
            if (interactable == null)
            {
                HidePrompt();
                return;
            }
            
            if (promptText != null)
            {
                promptText.text = interactable.InteractionPrompt;
            }
            
            if (promptPanel != null)
            {
                promptPanel.SetActive(true);
            }
            
            Debug.Log($"<color=cyan>[INTERACTION PROMPT UI] Showing prompt: {interactable.InteractionPrompt}</color>");
        }
        
        void HidePrompt()
        {
            if (promptPanel != null)
            {
                promptPanel.SetActive(false);
            }
            
            Debug.Log("<color=yellow>[INTERACTION PROMPT UI] Hiding prompt</color>");
        }
    }
}
