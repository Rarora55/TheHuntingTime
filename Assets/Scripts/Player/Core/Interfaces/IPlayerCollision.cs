using UnityEngine;

public interface IPlayerCollision
{
    bool CheckIsGrounded();
    bool CheckIfTouchingWall();
    bool CheckTouchingLedge();
    bool CheckForCeiling();
    Vector2 DetermineCornerPosition();
    void SetColliderHeight(float height);
}
