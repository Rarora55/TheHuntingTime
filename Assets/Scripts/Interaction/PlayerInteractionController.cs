using System;
using UnityEngine;

namespace TheHunt.Interaction
{
    public class PlayerInteractionController : MonoBehaviour, IInteractor
    {
        [Header("Detection Settings")]
        [SerializeField] private float detectionRadius = 2f;
        [SerializeField] private LayerMask interactionLayer;
        
        private IInteractable currentInteractable;
        private Collider2D[] detectionResults = new Collider2D[10];
        private ContactFilter2D contactFilter;
        
        public IInteractable CurrentInteractable => currentInteractable;
        public bool CanInteract => currentInteractable != null && currentInteractable.CanInteract(gameObject);
        
        public event Action<IInteractable> OnInteractableDetected;
        public event Action OnInteractableCleared;
        public event Action<IInteractable> OnInteracted;
        
        void Awake()
        {
            contactFilter = new ContactFilter2D
            {
                layerMask = interactionLayer,
                useLayerMask = true,
                useTriggers = true
            };
        }
        
        void Update()
        {
            DetectNearbyInteractables();
        }
        
        void DetectNearbyInteractables()
        {
            int numFound = Physics2D.OverlapCircle( transform.position, detectionRadius, contactFilter,detectionResults);
            
            if (numFound > 0)
            {
                Debug.Log($"<color=cyan>[PLAYER INTERACTION] Found {numFound} objects in detection radius</color>");
            }
            
            IInteractable closestInteractable = null;
            float closestDistance = float.MaxValue;
            
            for (int i = 0; i < numFound; i++)
            {
                Debug.Log($"<color=cyan>[PLAYER INTERACTION] Checking object: {detectionResults[i].gameObject.name}, layer: {LayerMask.LayerToName(detectionResults[i].gameObject.layer)}</color>");
                
                IInteractable interactable = detectionResults[i].GetComponent<IInteractable>();
                
                if (interactable == null)
                {
                    Debug.Log($"<color=yellow>[PLAYER INTERACTION] {detectionResults[i].gameObject.name} has no IInteractable component</color>");
                    continue;
                }
                
                Debug.Log($"<color=green>[PLAYER INTERACTION] {detectionResults[i].gameObject.name} has IInteractable, IsInteractable: {interactable.IsInteractable}</color>");
                
                if (interactable != null && interactable.IsInteractable)
                {
                    float distance = Vector2.Distance(transform.position, detectionResults[i].transform.position);
                    
                    Debug.Log($"<color=green>[PLAYER INTERACTION] {detectionResults[i].gameObject.name} is interactable, distance: {distance}</color>");
                    
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
                    Debug.Log($"<color=yellow>[PLAYER INTERACTION] Clearing current interactable</color>");
                    ClearInteractable();
                }
                
                if (closestInteractable != null)
                {
                    Debug.Log($"<color=green>[PLAYER INTERACTION] Setting new interactable: {((MonoBehaviour)closestInteractable).gameObject.name}</color>");
                    SetInteractable(closestInteractable);
                }
            }
        }
        
        public void SetInteractable(IInteractable interactable)
        {
            currentInteractable = interactable;
            OnInteractableDetected?.Invoke(interactable);
        }
        
        public void ClearInteractable()
        {
            currentInteractable = null;
            OnInteractableCleared?.Invoke();
        }
        
        public void TryInteract()
        {
            Debug.Log($"<color=magenta>[PLAYER INTERACTION] TryInteract() called - currentInteractable: {(currentInteractable != null ? ((MonoBehaviour)currentInteractable).gameObject.name : "NULL")}</color>");
            
            if (!CanInteract)
            {
                Debug.Log($"<color=yellow>[PLAYER INTERACTION] CanInteract is FALSE - currentInteractable null: {currentInteractable == null}, can interact: {(currentInteractable != null ? currentInteractable.CanInteract(gameObject).ToString() : "N/A")}</color>");
                return;
            }
            
            Debug.Log($"<color=green>[PLAYER INTERACTION] Calling Interact() on {((MonoBehaviour)currentInteractable).gameObject.name}</color>");
            currentInteractable.Interact(gameObject);
            OnInteracted?.Invoke(currentInteractable);
        }
        
        void OnDrawGizmosSelected()
        {
            Gizmos.color = Color.cyan;
            Gizmos.DrawWireSphere(transform.position, detectionRadius);
        }
    }
}
