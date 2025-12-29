using UnityEngine;

namespace TheHunt.Environment
{
    public interface IJumpZone
    {
        bool CanJumpInDirection(JumpDirection direction);
        Vector2 GetJumpForce(JumpDirection direction);
        JumpType GetJumpType();
        Transform GetTargetPosition();
        Transform GetTransform();
    }
}
