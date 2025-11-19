using System.Collections;
using UnityEngine;

public class HealingOverTime : MonoBehaviour
{
    private IHealable healable;
    private Coroutine healingCoroutine;
    
    void Awake()
    {
        healable = GetComponent<IHealable>();
    }
    
    public void StartHealing(HealingItemData itemData)
    {
        if (healingCoroutine != null)
        {
            StopCoroutine(healingCoroutine);
        }
        
        healingCoroutine = StartCoroutine(HealingRoutine(itemData));
    }
    
    IEnumerator HealingRoutine(HealingItemData itemData)
    {
        float elapsed = 0f;
        float healPerTick = itemData.healAmount / (itemData.duration / itemData.tickRate);
        
        Debug.Log($"<color=green>[HOT] Starting heal over time: {itemData.healAmount} HP " +
                  $"over {itemData.duration}s ({healPerTick:F1} per {itemData.tickRate}s)</color>");
        
        while (elapsed < itemData.duration && healable.CanHeal)
        {
            healable.Heal(healPerTick);
            elapsed += itemData.tickRate;
            yield return new WaitForSeconds(itemData.tickRate);
        }
        
        Debug.Log($"<color=green>[HOT] Healing complete!</color>");
        healingCoroutine = null;
    }
    
    public void StopHealing()
    {
        if (healingCoroutine != null)
        {
            StopCoroutine(healingCoroutine);
            healingCoroutine = null;
        }
    }
}
