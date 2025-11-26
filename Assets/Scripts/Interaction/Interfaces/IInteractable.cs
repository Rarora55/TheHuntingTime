using UnityEngine;

namespace TheHunt.Interaction
{
    public interface IInteractable
    {
        string InteractionPrompt { get; }
        
        bool CanInteract(GameObject interactor);
        
        void Interact(GameObject interactor);
        
        bool IsInteractable { get; }
    }
}
