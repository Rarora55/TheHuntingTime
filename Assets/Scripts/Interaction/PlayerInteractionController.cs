using System;
using System.Collections.Generic;
using UnityEngine;

namespace TheHunt.Interaction
{
    public class PlayerInteractionController : MonoBehaviour, IInteractor
    {
        [Header("Detection Settings")]
        [SerializeField] private float detectionRadius = 2f;
        [SerializeField] private LayerMask interactionLayer;
        [SerializeField] private float detectionInterval = 0.1f;
        
        [Header("Debug")]
        [SerializeField] private bool enableDebugLogs = false;
        
        private IInteractable currentInteractable;
        private Collider2D[] detectionResults = new Collider2D[10];
        private ContactFilter2D contactFilter;
        private float lastDetectionTime;
        private Dictionary<Collider2D, IInteractable> interactableCache = new Dictionary<Collider2D, IInteractable>();
        
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
            if (Time.time - lastDetectionTime >= detectionInterval)
            {
                DetectNearbyInteractables();
                lastDetectionTime = Time.time;
            }
        }
        
        void DetectNearbyInteractables()
        {
            int numFound = Physics2D.OverlapCircle(transform.position, detectionRadius, contactFilter, detectionResults);
            
            if (enableDebugLogs && numFound > 0)
            {
                Debug.Log($"<color=cyan>[PLAYER INTERACTION] Found {numFound} objects in detection radius</color>");
            }
            
            IInteractable closestInteractable = null;
            float closestDistance = float.MaxValue;
            
            for (int i = 0; i < numFound; i++)
            {
                Collider2D col = detectionResults[i];
                
                if (enableDebugLogs)
                {
                    Debug.Log($"<color=cyan>[PLAYER INTERACTION] Checking object: {col.gameObject.name}</color>");
                }
                
                if (!interactableCache.TryGetValue(col, out IInteractable interactable))
                {
                    interactable = col.GetComponent<IInteractable>();
                    interactableCache[col] = interactable;
                }
                
                if (interactable == null)
                {
                    if (enableDebugLogs)
                    {
                        Debug.Log($"<color=yellow>[PLAYER INTERACTION] {col.gameObject.name} has no IInteractable component</color>");
                    }
                    continue;
                }
                
                if (enableDebugLogs)
                {
                    Debug.Log($"<color=green>[PLAYER INTERACTION] {col.gameObject.name} has IInteractable, IsInteractable: {interactable.IsInteractable}</color>");
                }
                
                if (interactable.IsInteractable)
                {
                    float distance = Vector2.Distance(transform.position, col.transform.position);
                    
                    if (enableDebugLogs)
                    {
                        Debug.Log($"<color=green>[PLAYER INTERACTION] {col.gameObject.name} is interactable, distance: {distance}</color>");
                    }
                    
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
                    if (enableDebugLogs)
                    {
                        Debug.Log($"<color=yellow>[PLAYER INTERACTION] Clearing current interactable</color>");
                    }
                    ClearInteractable();
                }
                
                if (closestInteractable != null)
                {
                    if (enableDebugLogs)
                    {
                        Debug.Log($"<color=green>[PLAYER INTERACTION] Setting new interactable: {((MonoBehaviour)closestInteractable).gameObject.name}</color>");
                    }
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
