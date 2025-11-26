using System;
using UnityEngine;
using UnityEngine.InputSystem;

namespace TheHunt.Interaction
{
    public class PlayerInteractionController : MonoBehaviour, IInteractor
    {
        [Header("Detection Settings")]
        [SerializeField] private float detectionRadius = 2f;
        [SerializeField] private LayerMask interactionLayer;
        
        [Header("Input")]
        [SerializeField] private InputActionReference interactAction;
        
        private IInteractable currentInteractable;
        private Collider2D[] detectionResults = new Collider2D[10];
        
        public IInteractable CurrentInteractable => currentInteractable;
        public bool CanInteract => currentInteractable != null && currentInteractable.CanInteract(gameObject);
        
        public event Action<IInteractable> OnInteractableDetected;
        public event Action OnInteractableCleared;
        public event Action<IInteractable> OnInteracted;
        
        void OnEnable()
        {
            if (interactAction != null)
            {
                interactAction.action.performed += OnInteractPerformed;
            }
        }
        
        void OnDisable()
        {
            if (interactAction != null)
            {
                interactAction.action.performed -= OnInteractPerformed;
            }
        }
        
        void Update()
        {
            DetectNearbyInteractables();
        }
        
        void DetectNearbyInteractables()
        {
            int numFound = Physics2D.OverlapCircleNonAlloc(
                transform.position,
                detectionRadius,
                detectionResults,
                interactionLayer
            );
            
            IInteractable closestInteractable = null;
            float closestDistance = float.MaxValue;
            
            for (int i = 0; i < numFound; i++)
            {
                IInteractable interactable = detectionResults[i].GetComponent<IInteractable>();
                
                if (interactable != null && interactable.IsInteractable)
                {
                    float distance = Vector2.Distance(transform.position, detectionResults[i].transform.position);
                    
                    if (distance < closestDistance)
                    {
                        closestDistance = distance;
                        closestInteractable = interactable;
                    }
                }
            }
            
            if (closestInteractable != currentInteractable)
            {
                if (currentInteractable != null)
                {
                    ClearInteractable();
                }
                
                if (closestInteractable != null)
                {
                    SetInteractable(closestInteractable);
                }
            }
        }
        
        public void SetInteractable(IInteractable interactable)
        {
            currentInteractable = interactable;
            OnInteractableDetected?.Invoke(interactable);
            
            Debug.Log($"<color=cyan>[INTERACTION] Detected: {interactable.InteractionPrompt}</color>");
        }
        
        public void ClearInteractable()
        {
            currentInteractable = null;
            OnInteractableCleared?.Invoke();
            
            Debug.Log($"<color=cyan>[INTERACTION] Cleared</color>");
        }
        
        public void TryInteract()
        {
            if (!CanInteract)
            {
                Debug.Log($"<color=yellow>[INTERACTION] Cannot interact</color>");
                return;
            }
            
            Debug.Log($"<color=green>[INTERACTION] Interacting with: {currentInteractable.InteractionPrompt}</color>");
            
            currentInteractable.Interact(gameObject);
            OnInteracted?.Invoke(currentInteractable);
        }
        
        void OnInteractPerformed(InputAction.CallbackContext context)
        {
            TryInteract();
        }
        
        void OnDrawGizmosSelected()
        {
            Gizmos.color = Color.cyan;
            Gizmos.DrawWireSphere(transform.position, detectionRadius);
        }
    }
}
