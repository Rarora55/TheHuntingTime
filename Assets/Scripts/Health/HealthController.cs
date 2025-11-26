using System;
using System.Collections;
using UnityEngine;

public class HealthController : MonoBehaviour, IHealth, IDamageable, IHealable
{
    [SerializeField] private HealthData healthData;
    
    private float currentHealth;
    private bool isInvulnerable;
    private float lastDamageTime;
    private Coroutine regenerationCoroutine;
    
    //Propiedades de IHealth
    public float CurrentHealth => currentHealth;
    public float MaxHealth => healthData.maxHealth;
    public float HealthPercentage => currentHealth / MaxHealth;
    public bool IsAlive => currentHealth > 0;
    public bool IsDead => !IsAlive;
    public bool IsInvulnerable => isInvulnerable;
    public bool CanHeal => IsAlive && currentHealth < MaxHealth;
    
    public event Action<float, float> OnHealthChanged;
    public event Action<DamageData> OnDamaged;
    public event Action<float> OnHealed;
    public event Action OnDeath;
    
    void Awake()
    {
        InitializeHealth();
    }
    
    void InitializeHealth()
    {
        currentHealth = healthData.startingHealth;
        isInvulnerable = false;
        
        if (healthData.canRegenerate)
        {
            regenerationCoroutine = StartCoroutine(RegenerationRoutine());
        }
    }
    
    public void TakeDamage(float amount)
    {
        TakeDamage(new DamageData(amount, DamageType.Physical));
    }
    
    public void TakeDamage(DamageData damageData)
    {
        if (IsDead || isInvulnerable)
            return;
        
        float previousHealth = currentHealth;
        currentHealth = Mathf.Max(0, currentHealth - damageData.amount);
        lastDamageTime = Time.time;
        
        Debug.Log($"<color=orange>[HEALTH] {gameObject.name} took {damageData.amount:F1} " +
                  $"{damageData.damageType} damage. Health: {currentHealth:F1}/{MaxHealth}</color>");
        
        OnHealthChanged?.Invoke(currentHealth, previousHealth);
        OnDamaged?.Invoke(damageData);
        
        if (healthData.invulnerabilityDuration > 0)
        {
            StartCoroutine(InvulnerabilityRoutine());
        }
        
        if (IsDead)
        {
            Die();
        }
    }
    
    public void Heal(float amount)
    {
        if (!CanHeal)
            return;
        
        float previousHealth = currentHealth;
        currentHealth = Mathf.Min(MaxHealth, currentHealth + amount);
        
        Debug.Log($"<color=green>[HEALTH] {gameObject.name} healed {amount:F1}. " +
                  $"Health: {currentHealth:F1}/{MaxHealth}</color>");
        
        OnHealthChanged?.Invoke(currentHealth, previousHealth);
        OnHealed?.Invoke(amount);
    }
    
    public void HealToFull()
    {
        Heal(MaxHealth - currentHealth);
    }
    
    public void TakeFallDamage(float fallHeight)
    {
        if (!healthData.canTakeFallDamage)
            return;
        
        if (fallHeight < healthData.fallDamageThreshold)
            return;
        
        float excessHeight = fallHeight - healthData.fallDamageThreshold;
        float damage = Mathf.Min( excessHeight * healthData.fallDamageMultiplier, healthData.maxFallDamage);
        
        Debug.Log($"<color=yellow>[FALL DAMAGE] Height: {fallHeight:F1}m | " +
                  $"Excess: {excessHeight:F1}m | Damage: {damage:F1}</color>");
        
        TakeDamage(new DamageData(damage, DamageType.Fall));
    }
    
    void Die()
    {
        Debug.Log($"<color=red>[HEALTH] {gameObject.name} has died!</color>");
        
        if (regenerationCoroutine != null)
        {
            StopCoroutine(regenerationCoroutine);
            regenerationCoroutine = null;
        }
        
        OnDeath?.Invoke();
    }
    
    IEnumerator InvulnerabilityRoutine()
    {
        isInvulnerable = true;
        yield return new WaitForSeconds(healthData.invulnerabilityDuration);
        isInvulnerable = false;
    }
    
    IEnumerator RegenerationRoutine()
    {
        while (true)
        {
            yield return new WaitForSeconds(0.1f);
            
            if (IsAlive && currentHealth < MaxHealth && Time.time - lastDamageTime >= healthData.regenerationDelay)
            {
                float regenAmount = healthData.regenerationRate * 0.1f;
                Heal(regenAmount);
            }
        }
    }
    
    public void ResetHealth()
    {
        currentHealth = healthData.startingHealth;
        isInvulnerable = false;
        OnHealthChanged?.Invoke(currentHealth, currentHealth);
    }
    
    void OnValidate()
    {
        if (healthData != null && currentHealth > healthData.maxHealth)
        {
            currentHealth = healthData.maxHealth;
        }
    }
}
