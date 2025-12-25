using UnityEngine;

namespace TheHunt.Interaction.Examples
{
    public class HealthCheckInteraction : ConditionalInteraction
    {
        [Header("Health Requirement")]
        [SerializeField] private float minimumHealthRequired = 50f;
        [SerializeField] private bool requiresFullHealth = false;
        
        protected override bool CheckCondition(GameObject interactor)
        {
            HealthController healthController = interactor.GetComponent<HealthController>();
            
            if (healthController == null)
            {
                Debug.LogError("[HEALTH CHECK] Interactor does not have HealthController!");
                return false;
            }
            
            float currentHealth = healthController.CurrentHealth;
            float maxHealth = healthController.MaxHealth;
            
            bool conditionMet;
            
            if (requiresFullHealth)
            {
                conditionMet = Mathf.Approximately(currentHealth, maxHealth);
                Debug.Log($"<color=cyan>[HEALTH CHECK] Full health required. Current: {currentHealth}/{maxHealth} = {conditionMet}</color>");
            }
            else
            {
                conditionMet = currentHealth >= minimumHealthRequired;
                Debug.Log($"<color=cyan>[HEALTH CHECK] Min health: {minimumHealthRequired}. Current: {currentHealth} = {conditionMet}</color>");
            }
            
            return conditionMet;
        }
    }
}
