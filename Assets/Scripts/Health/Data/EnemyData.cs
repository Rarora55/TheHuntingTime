using UnityEngine;

[CreateAssetMenu(fileName = "New Enemy Data", menuName = "Enemy System/Enemy Data")]
public class EnemyData : ScriptableObject
{
    [Header("Enemy Identity")]
    [Tooltip("Name of the enemy type")]
    public string enemyName = "Enemy";
    
    [Tooltip("Enemy type identifier")]
    public EnemyType enemyType = EnemyType.Basic;
    
    [Header("Health Settings")]
    [Tooltip("Reference to health configuration")]
    public HealthData healthData;
    
    [Header("Combat Stats")]
    [Tooltip("Damage dealt to player on contact")]
    public float contactDamage = 10f;
    
    [Tooltip("Knockback force applied on contact")]
    public float knockbackForce = 5f;
    
    [Header("Visual Settings")]
    [Tooltip("Sprite for this enemy type")]
    public Sprite enemySprite;
    
    [Header("FSM Settings (Future)")]
    [Tooltip("Movement speed of the enemy")]
    public float moveSpeed = 2f;
    
    [Tooltip("Detection range for player")]
    public float detectionRange = 5f;
    
    [Tooltip("Attack range")]
    public float attackRange = 1.5f;
}

public enum EnemyType
{
    Basic,
    Flying,
    Heavy,
    Boss
}
