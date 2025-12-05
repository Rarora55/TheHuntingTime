using UnityEngine;

namespace TheHunt.Inventory
{
    public interface IUsable
    {
        bool CanUse(GameObject user);
        void Use(GameObject user);
        bool RemoveOnUse { get; }
    }
}
