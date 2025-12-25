using UnityEngine;
using UnityEngine.Events;
using TheHunt.UI;

namespace TheHunt.Interaction
{
    public class ConditionalInteraction : MonoBehaviour, IInteractable
    {
        [Header("Interaction Settings")]
        [SerializeField] protected string interactionPrompt = "Press E to interact";
        [SerializeField] protected bool isInteractable = true;
        
        [Header("Condition Check")]
        [SerializeField] private bool requiresCondition = true;
        
        [Header("Success (Condition Met)")]
        [SerializeField] private bool showSuccessMessage = false;
        [SerializeField] private string successTitle = "Success";
        [SerializeField] [TextArea(2, 5)] private string successMessage = "Action completed!";
        [SerializeField] private UnityEvent onSuccess;
        
        [Header("Failure (Condition Not Met)")]
        [SerializeField] private bool showFailureMessage = true;
        [SerializeField] protected string failureTitle = "Cannot Interact";
        [SerializeField] [TextArea(2, 5)] protected string failureMessage = "You cannot do this yet.";
        [SerializeField] private UnityEvent onFailure;
        
        protected DialogService dialogService;
        
        public string InteractionPrompt => interactionPrompt;
        public bool IsInteractable => isInteractable;
        
        protected virtual void Start()
        {
            dialogService = FindFirstObjectByType<DialogService>();
            
            if (dialogService == null)
            {
                Debug.LogError($"[CONDITIONAL INTERACTION] DialogService not found in scene! ({gameObject.name})");
            }
        }
        
        public virtual bool CanInteract(GameObject interactor)
        {
            return isInteractable;
        }
        
        public void Interact(GameObject interactor)
        {
            bool conditionMet = CheckCondition(interactor);
            
            Debug.Log($"<color=cyan>[CONDITIONAL INTERACTION] {gameObject.name} - Condition met: {conditionMet}</color>");
            
            if (conditionMet)
            {
                HandleSuccess(interactor);
            }
            else
            {
                HandleFailure(interactor);
            }
        }
        
        protected virtual bool CheckCondition(GameObject interactor)
        {
            return !requiresCondition;
        }
        
        protected virtual void HandleSuccess(GameObject interactor)
        {
            Debug.Log($"<color=green>[CONDITIONAL INTERACTION] Success! ({gameObject.name})</color>");
            
            if (showSuccessMessage && dialogService != null)
            {
                dialogService.ShowInfo(successTitle, successMessage, () => {
                    onSuccess?.Invoke();
                });
            }
            else
            {
                onSuccess?.Invoke();
            }
        }
        
        protected virtual void HandleFailure(GameObject interactor)
        {
            Debug.Log($"<color=yellow>[CONDITIONAL INTERACTION] Failure - condition not met ({gameObject.name})</color>");
            
            if (showFailureMessage && dialogService != null)
            {
                dialogService.ShowInfo(failureTitle, failureMessage);
            }
            
            onFailure?.Invoke();
        }
    }
}
