using UnityEngine;

namespace TheHunt.Interaction.Examples
{
    public class CooldownInteraction : ConditionalInteraction
    {
        [Header("Cooldown Settings")]
        [SerializeField] private float cooldownTime = 5f;
        [SerializeField] private bool showCooldownMessage = true;
        
        private float lastInteractionTime = -999f;
        private bool IsOnCooldown => Time.time - lastInteractionTime < cooldownTime;
        private float RemainingCooldown => Mathf.Max(0, cooldownTime - (Time.time - lastInteractionTime));
        
        protected override bool CheckCondition(GameObject interactor)
        {
            bool canInteract = !IsOnCooldown;
            
            if (!canInteract && showCooldownMessage)
            {
                Debug.Log($"<color=yellow>[COOLDOWN] Still on cooldown: {RemainingCooldown:F1}s remaining</color>");
            }
            
            return canInteract;
        }
        
        protected override void HandleSuccess(GameObject interactor)
        {
            lastInteractionTime = Time.time;
            Debug.Log($"<color=green>[COOLDOWN] Interaction successful. Next available in {cooldownTime}s</color>");
            
            base.HandleSuccess(interactor);
        }
        
        protected override void HandleFailure(GameObject interactor)
        {
            if (showCooldownMessage)
            {
                string cooldownMsg = $"Please wait {Mathf.CeilToInt(RemainingCooldown)} seconds before using again.";
                
                if (dialogService != null)
                {
                    dialogService.ShowInfo(failureTitle, cooldownMsg);
                }
            }
        }
    }
}
