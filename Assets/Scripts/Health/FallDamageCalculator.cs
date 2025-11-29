using UnityEngine;

public class FallDamageCalculator : MonoBehaviour
{
    private HealthController healthController;
    private Rigidbody2D rb;
    
    private bool wasGroundedLastFrame;
    private bool isFalling;
    private float fallStartHeight;
    private float maxFallSpeed;
    private float highestPointDuringFall;
    
    [Header("Fall Detection")]
    [SerializeField] private LayerMask groundLayer;
    [SerializeField] private Vector2 groundCheckSize = new Vector2(0.9f, 0.1f);
    [SerializeField] private Vector2 groundCheckOffset = new Vector2(0f, -0.5f);
    
    void Awake()
    {
        healthController = GetComponent<HealthController>();
        rb = GetComponent<Rigidbody2D>();
        
        if (groundLayer == 0)
        {
            groundLayer = LayerMask.GetMask("Ground");
        }
    }
    
    void Update()
    {
        TrackFalling();
    }
    
    void TrackFalling()
    {
        if (rb == null)
            return;
        
        bool isGrounded = CheckGrounded();
        float verticalVelocity = rb.linearVelocity.y;
        
        if (wasGroundedLastFrame && !isGrounded)
        {
            StartFalling();
        }
        
        if (isFalling)
        {
            maxFallSpeed = Mathf.Min(maxFallSpeed, verticalVelocity);
            highestPointDuringFall = Mathf.Max(highestPointDuringFall, transform.position.y);
        }
        
        wasGroundedLastFrame = isGrounded;
    }
    
    bool CheckGrounded()
    {
        Vector2 checkPosition = (Vector2)transform.position + groundCheckOffset;
        Collider2D hit = Physics2D.OverlapBox(checkPosition, groundCheckSize, 0f, groundLayer);
        return hit != null;
    }
    
    void StartFalling()
    {
        isFalling = true;
        fallStartHeight = transform.position.y;
        highestPointDuringFall = transform.position.y;
        maxFallSpeed = 0f;
        
        Debug.Log($"<color=cyan>[FALL] Started falling from height: {fallStartHeight:F2}</color>");
    }
    
    public void OnLanded()
    {
        if (!isFalling)
        {
            Debug.Log($"<color=yellow>[FALL] OnLanded called but wasn't falling</color>");
            return;
        }
        
        float fallEndHeight = transform.position.y;
        float fallDistance = highestPointDuringFall - fallEndHeight;
        
        Debug.Log($"<color=cyan>[FALL] Landed! Distance: {fallDistance:F2}m | " +
                  $"Max speed: {Mathf.Abs(maxFallSpeed):F2} | Start: {fallStartHeight:F2} | Highest: {highestPointDuringFall:F2}</color>");
        
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
    
    void OnDrawGizmosSelected()
    {
        Vector2 checkPosition = (Vector2)transform.position + groundCheckOffset;
        
        Gizmos.color = CheckGrounded() ? Color.green : Color.red;
        Gizmos.DrawWireCube(checkPosition, groundCheckSize);
    }
}
