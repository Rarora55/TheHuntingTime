using UnityEngine;

public class FallDamageCalculator : MonoBehaviour
{
    private HealthController healthController;
    private Rigidbody2D rb;
    
    private bool isFalling;
    private float fallStartHeight;
    private float maxFallSpeed;
    
    [Header("Fall Detection")]
    [SerializeField] private float fallSpeedThreshold = 5f;
    
    void Awake()
    {
        healthController = GetComponent<HealthController>();
        rb = GetComponent<Rigidbody2D>();
    }
    
    void Update()
    {
        TrackFalling();
    }
    
    void TrackFalling()
    {
        if (rb == null)
            return;
        
        float verticalVelocity = rb.linearVelocity.y;
        
        if (verticalVelocity < -fallSpeedThreshold && !isFalling)
        {
            StartFalling();
        }
        
        if (isFalling)
        {
            maxFallSpeed = Mathf.Min(maxFallSpeed, verticalVelocity);
        }
    }
    
    void StartFalling()
    {
        isFalling = true;
        fallStartHeight = transform.position.y;
        maxFallSpeed = 0f;
        
        Debug.Log($"<color=cyan>[FALL] Started falling from height: {fallStartHeight:F2}</color>");
    }
    
    public void OnLanded()
    {
        if (!isFalling)
            return;
        
        float fallEndHeight = transform.position.y;
        float fallDistance = fallStartHeight - fallEndHeight;
        
        Debug.Log($"<color=cyan>[FALL] Landed! Distance: {fallDistance:F2}m | " +
                  $"Max speed: {Mathf.Abs(maxFallSpeed):F2}</color>");
        
        if (healthController != null)
        {
            healthController.TakeFallDamage(fallDistance);
        }
        
        isFalling = false;
        maxFallSpeed = 0f;
    }
    
    public void CancelFall()
    {
        isFalling = false;
        maxFallSpeed = 0f;
    }
}
