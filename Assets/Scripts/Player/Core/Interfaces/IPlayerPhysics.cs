using UnityEngine;

public interface IPlayerPhysics
{
    Vector2 CurrentVelocity { get; }
    
    void UpdateVelocity();
    void SetVelocity(Vector2 velocity);
    void SetVelocity(float x, float y);
    void SetVelocityX(float velocityX);
    void SetVelocityY(float velocityY);
    void SetVelocityZero();
}
