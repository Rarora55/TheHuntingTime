using UnityEngine;

public enum HealingType
{
    Instant,
    OverTime
}

[CreateAssetMenu(fileName = "New Healing Item", menuName = "Health System/Healing Item")]
public class HealingItemData : ScriptableObject
{
    [Header("Item Info")]
    public string itemName = "Health Pack";
    public Sprite icon;
    
    [Header("Healing Configuration")]
    public HealingType healingType = HealingType.Instant;
    
    [Tooltip("Amount of health restored")]
    public float healAmount = 25f;
    
    [Header("Over Time Settings")]
    [Tooltip("Duration of healing effect (for OverTime type)")]
    public float duration = 5f;
    
    [Tooltip("How often to apply healing tick (for OverTime type)")]
    public float tickRate = 1f;
    
    [Header("Effects")]
    public GameObject pickupVFX;
    public AudioClip pickupSound;
}
