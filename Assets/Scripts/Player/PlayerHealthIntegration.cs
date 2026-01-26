using UnityEngine;

public class PlayerHealthIntegration : MonoBehaviour
{
    private Player player;
    private HealthController healthController;
    private FallDamageCalculator fallDamageCalculator;
    
    void Awake()
    {
        player = GetComponent<Player>();
        healthController = GetComponent<HealthController>();
        fallDamageCalculator = GetComponent<FallDamageCalculator>();
        
        SubscribeToEvents();
    }
    
    void OnDestroy()
    {
        UnsubscribeFromEvents();
    }
    
    void SubscribeToEvents()
    {
        if (healthController != null)
        {
            healthController.OnDeath += HandleDeath;
            healthController.OnDamaged += HandleDamaged;
            healthController.OnHealed += HandleHealed;
        }
    }
    
    void UnsubscribeFromEvents()
    {
        if (healthController != null)
        {
            healthController.OnDeath -= HandleDeath;
            healthController.OnDamaged -= HandleDamaged;
            healthController.OnHealed -= HandleHealed;
        }
    }
    
    void HandleDeath()
    {
        Debug.Log("<color=red>[PLAYER DEATH] Player has died!</color>");
        
        Debug.Log($"<color=magenta>[PLAYER DEATH] Setting Animator 'death' parameter to TRUE. Current value: {player.anim.GetBool("death")}</color>");
        player.anim.SetBool("death", true);
        Debug.Log($"<color=magenta>[PLAYER DEATH] Animator 'death' parameter set. New value: {player.anim.GetBool("death")}</color>");
    }
    
    void HandleDamaged(DamageData damageData)
    {
        // TODO: Verify correct animator parameter name for damage animation
        // player.anim.SetTrigger("damaged");
        
        if (damageData.damageDirection != Vector2.zero)
        {
            ApplyKnockback(damageData.damageDirection, damageData.amount);
        }
    }
    
    void HandleHealed(float amount)
    {
        Debug.Log($"<color=green>[PLAYER HEAL] Healed {amount:F1} HP</color>");
    }
    
    void ApplyKnockback(Vector2 direction, float damageAmount)
    {
        float knockbackForce = Mathf.Min(damageAmount * 0.5f, 10f);
        Vector2 knockback = direction.normalized * knockbackForce;
        
        player.RB.linearVelocity = Vector2.zero;
        player.RB.AddForce(knockback, ForceMode2D.Impulse);
    }
    
    public void OnPlayerLanded()
    {
        if (fallDamageCalculator != null)
        {
            fallDamageCalculator.OnLanded();
        }
    }
}
