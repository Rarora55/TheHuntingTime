using UnityEngine;

public interface IPlayerCollision
{
    bool CheckIsGrounded();
    bool CheckIfTouchingWall();
    bool CheckTouchingLedge();
    bool CheckForCeiling();
    bool CheckGroundEdgeAhead();
    bool ShouldAutoGrabLedge();
    bool CheckCanGrabLedgeFromAbove();
    Vector2 DetermineCornerPosition();
    Vector2 DetermineCornerPositionFromAbove();
    bool IsValidLedge(float minHeight);
    void SetColliderHeight(float height);
}
