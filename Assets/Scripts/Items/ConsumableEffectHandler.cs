using System.Collections;
using UnityEngine;

public class ConsumableEffectHandler : MonoBehaviour
{
    private IHealable healable;
    private Coroutine healthOverTimeCoroutine;
    
    void Awake()
    {
        healable = GetComponent<IHealable>();
    }
    
    public void StartHealthOverTime(float totalAmount, float duration, float tickRate)
    {
        if (healthOverTimeCoroutine != null)
        {
            StopCoroutine(healthOverTimeCoroutine);
        }
        
        healthOverTimeCoroutine = StartCoroutine(HealthOverTimeRoutine(totalAmount, duration, tickRate));
    }
    
    IEnumerator HealthOverTimeRoutine(float totalAmount, float duration, float tickRate)
    {
        float elapsed = 0f;
        float healPerTick = totalAmount / (duration / tickRate);
        
        while (elapsed < duration && healable != null && healable.CanHeal)
        {
            healable.Heal(healPerTick);
            elapsed += tickRate;
            yield return new WaitForSeconds(tickRate);
        }
        
        healthOverTimeCoroutine = null;
    }
    
    public void StopHealthOverTime()
    {
        if (healthOverTimeCoroutine != null)
        {
            StopCoroutine(healthOverTimeCoroutine);
            healthOverTimeCoroutine = null;
        }
    }
}
