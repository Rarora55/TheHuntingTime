using UnityEngine;

public enum ConsumableEffect
{
    RestoreHealth,
    RestoreHealthOverTime,
    RestoreStamina,
    Buff,
    Antidote
}

[CreateAssetMenu(fileName = "New Consumable", menuName = "Items/Consumable Item")]
public class ConsumableItemData : ScriptableObject, IItem, IUsableItem
{
    [Header("Item Info")]
    [SerializeField] private string itemName = "Health Potion";
    [SerializeField] private string description = "Restores health";
    [SerializeField] private Sprite icon;
    [SerializeField] private ItemType itemType = ItemType.Consumable;
    
    [Header("Stack Settings")]
    [SerializeField] private bool isStackable = true;
    [SerializeField] private int maxStackSize = 99;
    
    [Header("Consumable Effect")]
    [SerializeField] private ConsumableEffect effectType = ConsumableEffect.RestoreHealth;
    [SerializeField] private float effectValue = 50f;
    
    [Header("Over Time Settings")]
    [SerializeField] private bool isOverTime = false;
    [SerializeField] private float duration = 5f;
    [SerializeField] private float tickRate = 1f;
    
    [Header("Feedback")]
    [SerializeField] private GameObject useVFX;
    [SerializeField] private AudioClip useSound;
    
    public string ItemName => itemName;
    public string Description => description;
    public Sprite Icon => icon;
    public ItemType ItemType => itemType;
    public bool IsStackable => isStackable;
    public int MaxStackSize => maxStackSize;
    public bool IsConsumedOnUse => true;
    
    public ConsumableEffect EffectType => effectType;
    public float EffectValue => effectValue;
    public bool IsOverTime => isOverTime;
    public float Duration => duration;
    public float TickRate => tickRate;
    public GameObject UseVFX => useVFX;
    public AudioClip UseSound => useSound;
    
    public bool CanUse(GameObject user)
    {
        switch (effectType)
        {
            case ConsumableEffect.RestoreHealth:
            case ConsumableEffect.RestoreHealthOverTime:
                IHealable healable = user.GetComponent<IHealable>();
                return healable != null && healable.CanHeal;
                
            default:
                return true;
        }
    }
    
    public void Use(GameObject user)
    {
        if (!CanUse(user))
        {
            return;
        }
        
        ApplyEffect(user);
        PlayFeedback(user);
    }
    
    void ApplyEffect(GameObject target)
    {
        switch (effectType)
        {
            case ConsumableEffect.RestoreHealth:
                ApplyHealthRestore(target);
                break;
                
            case ConsumableEffect.RestoreHealthOverTime:
                ApplyHealthOverTime(target);
                break;
                
            case ConsumableEffect.RestoreStamina:
                break;
        }
    }
    
    void ApplyHealthRestore(GameObject target)
    {
        IHealable healable = target.GetComponent<IHealable>();
        if (healable != null)
        {
            healable.Heal(effectValue);
        }
    }
    
    void ApplyHealthOverTime(GameObject target)
    {
        ConsumableEffectHandler handler = target.GetComponent<ConsumableEffectHandler>();
        if (handler == null)
        {
            handler = target.AddComponent<ConsumableEffectHandler>();
        }
        
        handler.StartHealthOverTime(effectValue, duration, tickRate);
    }
    
    void PlayFeedback(GameObject target)
    {
        if (useVFX != null)
        {
            Object.Instantiate(useVFX, target.transform.position, Quaternion.identity);
        }
        
        if (useSound != null)
        {
        }
    }
}
