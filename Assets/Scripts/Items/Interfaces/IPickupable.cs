using UnityEngine;

public interface IPickupable
{
    IItem ItemData { get; }
    void OnPickedUp(GameObject picker);
}
