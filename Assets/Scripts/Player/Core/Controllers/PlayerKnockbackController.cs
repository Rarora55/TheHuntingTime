using UnityEngine;
using System.Collections;

public class PlayerKnockbackController : MonoBehaviour
{
    private Rigidbody2D rb;
    private Player player;
    private PlayerData playerData;
    
    private bool isInKnockback;
    private float knockbackEndTime;
    private Vector2 knockbackVelocity;
    private Coroutine delayedKnockbackCoroutine;
    
    public bool IsInKnockback => isInKnockback;
    
    void Awake()
    {
        rb = GetComponent<Rigidbody2D>();
        player = GetComponent<Player>();
    }
    
    void Start()
    {
        playerData = FindObjectOfType<Player>()?.GetType()
            .GetField("PlayerData", System.Reflection.BindingFlags.NonPublic | System.Reflection.BindingFlags.Instance)
            ?.GetValue(player) as PlayerData;
    }
    
    void FixedUpdate()
    {
        if (isInKnockback)
        {
            if (Time.time >= knockbackEndTime)
            {
                EndKnockback();
            }
            else
            {
                rb.linearVelocity = knockbackVelocity;
            }
        }
    }
    
    public void ApplyShootKnockback()
    {
        if (playerData == null)
            return;
        
        if (delayedKnockbackCoroutine != null)
        {
            StopCoroutine(delayedKnockbackCoroutine);
        }
        
        delayedKnockbackCoroutine = StartCoroutine(ApplyShootKnockbackDelayed());
    }
    
    private IEnumerator ApplyShootKnockbackDelayed()
    {
        yield return new WaitForSeconds(playerData.shootKnockbackDelay);
        
        int direction = -player.FacingRight;
        Vector2 knockback = new Vector2(direction * playerData.shootKnockbackForce, 0f);
        
        ApplyKnockback(knockback, playerData.knockbackDuration);
        delayedKnockbackCoroutine = null;
    }
    
    public void ApplyWallCollisionKnockback()
    {
        if (playerData == null)
            return;
        
        int direction = -player.FacingRight;
        Vector2 knockback = new Vector2(direction * playerData.wallCollisionKnockbackForce, playerData.wallCollisionKnockbackForce * 0.5f);
        
        ApplyKnockback(knockback, playerData.knockbackDuration);
    }
    
    private void ApplyKnockback(Vector2 knockback, float duration)
    {
        isInKnockback = true;
        knockbackVelocity = knockback;
        knockbackEndTime = Time.time + duration;
    }
    
    private void EndKnockback()
    {
        isInKnockback = false;
        knockbackVelocity = Vector2.zero;
    }
    
    public void CancelKnockback()
    {
        if (delayedKnockbackCoroutine != null)
        {
            StopCoroutine(delayedKnockbackCoroutine);
            delayedKnockbackCoroutine = null;
        }
        
        EndKnockback();
    }
}
