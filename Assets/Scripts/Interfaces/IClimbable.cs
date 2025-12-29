using UnityEngine;

namespace TheHunt.Environment
{
    public interface IClimbable
    {
        ClimbType GetClimbType();
        float GetClimbSpeed();
        bool AllowsWallSlide();
        bool CanBePickedUp();
        Transform GetTransform();
    }
}
