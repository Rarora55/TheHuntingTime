using UnityEngine;

public interface IUsableItem : IItem
{
    bool CanUse(GameObject user);
    void Use(GameObject user);
    bool IsConsumedOnUse { get; }
}
