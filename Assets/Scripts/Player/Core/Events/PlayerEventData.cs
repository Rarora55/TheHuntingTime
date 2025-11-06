using UnityEngine;

public class PlayerEventData
{
    public Vector2 Velocity;
    public Vector2 Position;
    public int Direction;
    public bool IsGrounded;
    public bool IsTouchingWall;
    public bool IsTouchingLedge;
    public float DeltaTime;
}

public class PlayerStateChangeData
{
    public string PreviousStateName;
    public string NewStateName;
    public float TimeInPreviousState;
}

public class PlayerCollisionData
{
    public bool WasGrounded;
    public bool IsGrounded;
    public Vector2 CollisionPoint;
    public Vector2 CollisionNormal;
}

public class PlayerAnimationEventData
{
    public string TriggerName;
    public string StateName;
}
