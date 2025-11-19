using UnityEngine;

[CreateAssetMenu(fileName = "New Health Data", menuName = "Health System/Health Data")]
public class HealthData : ScriptableObject
{
    [Header("Health Configuration")]
    [Tooltip("Maximum health points")]
    public float maxHealth = 100f;
    
    [Tooltip("Starting health (if different from max)")]
    public float startingHealth = 100f;
    
    [Header("Regeneration")]
    [Tooltip("Enable health regeneration over time")]
    public bool canRegenerate = false;
    
    [Tooltip("Health points recovered per second")]
    public float regenerationRate = 1f;
    
    [Tooltip("Delay before regeneration starts after taking damage")]
    public float regenerationDelay = 3f;
    
    [Header("Invulnerability")]
    [Tooltip("Duration of invulnerability after taking damage")]
    public float invulnerabilityDuration = 1f;
    
    [Header("Fall Damage")]
    [Tooltip("Enable fall damage")]
    public bool canTakeFallDamage = true;
    
    [Tooltip("Minimum fall height to start taking damage")]
    public float fallDamageThreshold = 5f;
    
    [Tooltip("Damage multiplier per unit of height above threshold")]
    public float fallDamageMultiplier = 10f;
    
    [Tooltip("Maximum fall damage cap")]
    public float maxFallDamage = 50f;
}
