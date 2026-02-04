using UnityEngine;

public class PlayerPhysicsController : IPlayerPhysics
{
    private readonly Rigidbody2D rb;
    private readonly PlayerEvents events;
    private readonly Player player;
    private Vector2 workSpace;
    
    public Vector2 CurrentVelocity { get; private set; }
    
    public PlayerPhysicsController(Rigidbody2D rigidbody, PlayerEvents playerEvents, Player playerComponent)
    {
        rb = rigidbody;
        events = playerEvents;
        player = playerComponent;
    }
    
    public void UpdateVelocity()
    {
        CurrentVelocity = rb.linearVelocity;
    }
    
    public void SetVelocity(Vector2 velocity)
    {
        workSpace = velocity;
        ApplyVelocity();
    }
    
    public void SetVelocity(float x, float y)
    {
        workSpace.Set(x, y);
        ApplyVelocity();
    }
    
    public void SetVelocityX(float velocityX)
    {
        workSpace.Set(velocityX, rb.linearVelocity.y);
        ApplyVelocity();
    }
    
    public void SetVelocityY(float velocityY)
    {
        workSpace.Set(rb.linearVelocity.x, velocityY);
        ApplyVelocity();
    }
    
    public void SetVelocityZero()
    {
        workSpace = Vector2.zero;
        ApplyVelocity();
    }
    
    private void ApplyVelocity()
    {
        if (player?.KnockbackController != null && player.KnockbackController.IsInKnockback)
        {
            CurrentVelocity = rb.linearVelocity;
            return;
        }
        
        rb.linearVelocity = workSpace;
        CurrentVelocity = workSpace;
        events?.InvokeVelocityChanged(CurrentVelocity);
    }
}
