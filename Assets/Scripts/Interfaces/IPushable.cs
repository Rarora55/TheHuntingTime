using UnityEngine;

namespace TheHunt.Environment
{
    public interface IPushable
    {
        PushableObjectData PushableData { get; }
        bool CanBePushed { get; }
        bool CanBePulled { get; }
        
        void OnPushStart();
        void OnPushEnd();
        void OnPullStart();
        void OnPullEnd();
        
        Transform GetObjectTransform();
        Rigidbody2D GetRigidbody();
    }
}
