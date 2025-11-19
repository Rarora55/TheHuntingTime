using UnityEngine;

public interface IItem
{
    string ItemName { get; }
    string Description { get; }
    Sprite Icon { get; }
    ItemType ItemType { get; }
    bool IsStackable { get; }
    int MaxStackSize { get; }
}
