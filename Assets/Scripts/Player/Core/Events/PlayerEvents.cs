using System;
using UnityEngine;

public class PlayerEvents
{
    public event Action<PlayerStateChangeData> OnStateChanged;
    public event Action<PlayerCollisionData> OnGroundedChanged;
    public event Action<int> OnFlipped;
    public event Action<Vector2> OnVelocityChanged;
    public event Action<PlayerAnimationEventData> OnAnimationTrigger;
    
    public void InvokeStateChanged(PlayerStateChangeData data)
    {
        OnStateChanged?.Invoke(data);
    }
    
    public void InvokeGroundedChanged(PlayerCollisionData data)
    {
        OnGroundedChanged?.Invoke(data);
    }
    
    public void InvokeFlipped(int newDirection)
    {
        OnFlipped?.Invoke(newDirection);
    }
    
    public void InvokeVelocityChanged(Vector2 newVelocity)
    {
        OnVelocityChanged?.Invoke(newVelocity);
    }
    
    public void InvokeAnimationTrigger(PlayerAnimationEventData data)
    {
        OnAnimationTrigger?.Invoke(data);
    }
}
