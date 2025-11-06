using UnityEngine;

public class PlayerPhysicsController : IPlayerPhysics
{
    private readonly Rigidbody2D rb;
    private readonly PlayerEvents events;
    private Vector2 workSpace;
    
    public Vector2 CurrentVelocity { get; private set; }
    
    public PlayerPhysicsController(Rigidbody2D rigidbody, PlayerEvents playerEvents)
    {
        rb = rigidbody;
        events = playerEvents;
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
        rb.linearVelocity = workSpace;
        CurrentVelocity = workSpace;
        events?.InvokeVelocityChanged(CurrentVelocity);
    }
}
