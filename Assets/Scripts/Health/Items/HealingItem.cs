using System.Collections;
using UnityEngine;

[RequireComponent(typeof(Collider2D))]
public class HealingItem : MonoBehaviour
{
    [SerializeField] private HealingItemData itemData;
    
    void OnTriggerEnter2D(Collider2D other)
    {
        IHealable healable = other.GetComponent<IHealable>();
        
        if (healable != null && healable.CanHeal)
        {
            ApplyHealing(healable, other.gameObject);
        }
    }
    
    void ApplyHealing(IHealable target, GameObject targetObject)
    {
        switch (itemData.healingType)
        {
            case HealingType.Instant:
                target.Heal(itemData.healAmount);
                break;
                
            case HealingType.OverTime:
                HealingOverTime hot = targetObject.GetComponent<HealingOverTime>();
                if (hot == null)
                {
                    hot = targetObject.AddComponent<HealingOverTime>();
                }
                hot.StartHealing(itemData);
                break;
        }
        
        PlayEffects();
        Destroy(gameObject);
    }
    
    void PlayEffects()
    {
        if (itemData.pickupVFX != null)
        {
            Instantiate(itemData.pickupVFX, transform.position, Quaternion.identity);
        }
        
        if (itemData.pickupSound != null)
        {
        }
    }
}
