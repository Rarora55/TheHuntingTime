using System;
using UnityEngine;

namespace TheHunt.Interaction
{
    public abstract class InteractableObject : MonoBehaviour, IInteractable
    {
        [Header("Interaction Settings")]
        [SerializeField] protected string interactionPrompt = "Press E to interact";
        [SerializeField] protected bool isInteractable = true;
        
        public string InteractionPrompt => interactionPrompt;
        public bool IsInteractable => isInteractable;
        
        public event Action<GameObject> OnInteractedWith;
        
        public virtual bool CanInteract(GameObject interactor)
        {
            return isInteractable;
        }
        
        public void Interact(GameObject interactor)
        {
            if (!CanInteract(interactor))
            {
                Debug.LogWarning($"<color=yellow>[INTERACTABLE] Cannot interact with {gameObject.name}</color>");
                return;
            }
            
            Debug.Log($"<color=green>[INTERACTABLE] {interactor.name} interacted with {gameObject.name}</color>");
            
            OnInteract(interactor);
            OnInteractedWith?.Invoke(interactor);
        }
        
        protected abstract void OnInteract(GameObject interactor);
        
        public void SetInteractable(bool value)
        {
            isInteractable = value;
        }
    }
}
