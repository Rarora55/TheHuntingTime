using UnityEngine;

public class PlayerOrientationController : IPlayerOrientation
{
    private readonly Transform playerTransform;
    private readonly PlayerEvents events;
    
    public int FacingDirection { get; private set; }
    
    public PlayerOrientationController(Transform transform, PlayerEvents playerEvents, int initialDirection = 1)
    {
        playerTransform = transform;
        events = playerEvents;
        FacingDirection = initialDirection;
    }
    
    public void Flip()
    {
        FacingDirection *= -1;
        playerTransform.Rotate(0.0f, 180f, 0.0f);
        events?.InvokeFlipped(FacingDirection);
    }
    
    public void CheckFlip(int xInput)
    {
        if (xInput != 0 && xInput != FacingDirection)
        {
            Flip();
        }
    }
}
