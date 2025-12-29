using UnityEngine;

namespace TheHunt.Environment
{
    public interface ISlideable
    {
        bool CanSlide(global::Player player);
        float GetSlideSpeed();
        float GetSurfaceAngle();
        Transform GetTransform();
    }
}
