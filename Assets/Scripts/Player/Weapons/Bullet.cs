using UnityEngine;

[RequireComponent(typeof(Rigidbody2D))]
public class Bullet : MonoBehaviour
{
    [SerializeField] private BulletData bulletData;
    [SerializeField] private SpriteRenderer spriteRenderer;
    [SerializeField] private TrailRenderer trailRenderer;
    
    private Rigidbody2D rb;
    private float spawnTime;
    private Vector2 direction;
    private GameObject owner;
    private BulletPool pool;
    
    public BulletData BulletData => bulletData;
    
    void Awake()
    {
        rb = GetComponent<Rigidbody2D>();
        
        if (spriteRenderer == null)
            spriteRenderer = GetComponent<SpriteRenderer>();
        
        if (trailRenderer == null)
            trailRenderer = GetComponent<TrailRenderer>();
        
        rb.gravityScale = 0f;
        rb.collisionDetectionMode = CollisionDetectionMode2D.Continuous;
    }
    
    public void Initialize(BulletData data, Vector2 fireDirection, GameObject bulletOwner, BulletPool bulletPool)
    {
        bulletData = data;
        direction = fireDirection.normalized;
        owner = bulletOwner;
        pool = bulletPool;
        spawnTime = Time.time;
        
        if (bulletData != null)
        {
            rb.linearVelocity = direction * bulletData.speed;
            
            if (spriteRenderer != null && bulletData.bulletSprite != null)
            {
                spriteRenderer.sprite = bulletData.bulletSprite;
                transform.localScale = bulletData.bulletScale;
            }
            
            if (trailRenderer != null)
            {
                trailRenderer.startColor = bulletData.trailColor;
                trailRenderer.endColor = new Color(bulletData.trailColor.r, bulletData.trailColor.g, bulletData.trailColor.b, 0f);
                trailRenderer.Clear();
            }
            
            float angle = Mathf.Atan2(direction.y, direction.x) * Mathf.Rad2Deg;
            transform.rotation = Quaternion.Euler(0f, 0f, angle);
        }
    }
    
    void Update()
    {
        if (bulletData != null && Time.time - spawnTime >= bulletData.lifetime)
        {
            ReturnToPool();
        }
    }
    
    void OnTriggerEnter2D(Collider2D other)
    {
        if (other.gameObject == owner)
            return;
        
        IDamageable damageable = other.GetComponent<IDamageable>();
        if (damageable != null && damageable.IsAlive)
        {
            DamageData damageData = new DamageData(
                bulletData.damage,
                bulletData.damageType,
                direction * bulletData.knockbackForce,
                owner
            );
            
            damageable.TakeDamage(damageData);
            ReturnToPool();
            return;
        }
        
        if (other.gameObject.layer == LayerMask.NameToLayer("Ground") || 
            other.gameObject.layer == LayerMask.NameToLayer("Wall"))
        {
            ReturnToPool();
        }
    }
    
    void ReturnToPool()
    {
        if (trailRenderer != null)
            trailRenderer.Clear();
        
        rb.linearVelocity = Vector2.zero;
        pool?.ReturnBullet(this);
    }
    
    void OnDisable()
    {
        if (trailRenderer != null)
            trailRenderer.Clear();
    }
}
