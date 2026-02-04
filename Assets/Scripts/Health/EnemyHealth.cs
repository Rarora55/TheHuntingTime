using UnityEngine;

public class EnemyHealth : HealthController
{
    [Header("Enemy Configuration")]
    [SerializeField] private EnemyData enemyData;
    
    [Header("Visual Feedback")]
    [SerializeField] private SpriteRenderer spriteRenderer;
    [SerializeField] private Color damageFlashColor = Color.red;
    [SerializeField] private float flashDuration = 0.1f;
    
    private Color originalColor;
    private Rigidbody2D rb;
    
    void Start()
    {
        if (spriteRenderer == null)
            spriteRenderer = GetComponent<SpriteRenderer>();
        
        if (spriteRenderer != null)
        {
            originalColor = spriteRenderer.color;
            
            if (enemyData != null && enemyData.enemySprite != null)
                spriteRenderer.sprite = enemyData.enemySprite;
        }
        
        rb = GetComponent<Rigidbody2D>();
        
        OnDamaged += HandleDamageVisual;
        OnDeath += HandleDeath;
    }
    
    void OnDestroy()
    {
        OnDamaged -= HandleDamageVisual;
        OnDeath -= HandleDeath;
    }
    
    void HandleDamageVisual(DamageData damageData)
    {
        if (spriteRenderer != null)
            StartCoroutine(FlashRoutine());
        
        if (rb != null && damageData.damageDirection != Vector2.zero)
            ApplyKnockback(damageData.damageDirection);
    }
    
    void ApplyKnockback(Vector2 direction)
    {
        Vector2 knockback = direction.normalized * (enemyData != null ? enemyData.knockbackForce : 5f);
        rb.linearVelocity = knockback;
    }
    
    System.Collections.IEnumerator FlashRoutine()
    {
        if (spriteRenderer != null)
        {
            spriteRenderer.color = damageFlashColor;
            yield return new WaitForSeconds(flashDuration);
            spriteRenderer.color = originalColor;
        }
    }
    
    void HandleDeath()
    {
        Debug.Log($"<color=red>[ENEMY] {enemyData?.enemyName ?? gameObject.name} defeated!</color>");
        
        Destroy(gameObject, 0.5f);
    }
    
    void OnCollisionEnter2D(Collision2D collision)
    {
        if (collision.gameObject.CompareTag("Player") && enemyData != null)
        {
            IDamageable player = collision.gameObject.GetComponent<IDamageable>();
            if (player != null && player.IsAlive)
            {
                Vector2 knockbackDirection = (collision.transform.position - transform.position).normalized;
                DamageData damageData = new DamageData(
                    enemyData.contactDamage,
                    DamageType.Physical,
                    knockbackDirection,
                    gameObject
                );
                
                player.TakeDamage(damageData);
            }
        }
    }
    
    public EnemyData GetEnemyData() => enemyData;
}
