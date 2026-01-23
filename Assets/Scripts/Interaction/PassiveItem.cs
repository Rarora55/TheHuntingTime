using UnityEngine;

namespace TheHunt.Interaction
{
    public abstract class PassiveItem : InteractableObject
    {
        protected override void OnInteract(GameObject interactor)
        {
            ExecutePassiveAction(interactor);
        }
        
        protected abstract void ExecutePassiveAction(GameObject interactor);
        
        public override bool CanInteract(GameObject interactor)
        {
            return base.CanInteract(interactor) && CanExecuteAction(interactor);
        }
        
        protected virtual bool CanExecuteAction(GameObject interactor)
        {
            return true;
        }
    }
}
