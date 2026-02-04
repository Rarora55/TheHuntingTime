using UnityEngine;

[CreateAssetMenu(fileName = "New Bullet Data", menuName = "Weapons/Bullet Data")]
public class BulletData : ScriptableObject
{
    [Header("Bullet Identity")]
    [Tooltip("Name of the bullet type")]
    public string bulletName = "Bullet";
    
    [Header("Damage Settings")]
    [Tooltip("Damage dealt on impact")]
    public float damage = 10f;
    
    [Tooltip("Type of damage dealt")]
    public DamageType damageType = DamageType.Physical;
    
    [Tooltip("Knockback force applied on hit")]
    public float knockbackForce = 2f;
    
    [Header("Movement Settings")]
    [Tooltip("Speed of the bullet")]
    public float speed = 20f;
    
    [Tooltip("Lifetime of the bullet in seconds")]
    public float lifetime = 3f;
    
    [Header("Visual Settings")]
    [Tooltip("Sprite for this bullet type")]
    public Sprite bulletSprite;
    
    [Tooltip("Scale of the bullet sprite")]
    public Vector3 bulletScale = Vector3.one;
    
    [Tooltip("Trail renderer color (optional)")]
    public Color trailColor = Color.yellow;
}
