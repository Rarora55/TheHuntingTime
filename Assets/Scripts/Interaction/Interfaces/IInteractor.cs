using System;
using UnityEngine;

namespace TheHunt.Interaction
{
    public interface IInteractor
    {
        IInteractable CurrentInteractable { get; }
        
        bool CanInteract { get; }
        
        void SetInteractable(IInteractable interactable);
        
        void ClearInteractable();
        
        void TryInteract();
        
        event Action<IInteractable> OnInteractableDetected;
        event Action OnInteractableCleared;
        event Action<IInteractable> OnInteracted;
    }
}
