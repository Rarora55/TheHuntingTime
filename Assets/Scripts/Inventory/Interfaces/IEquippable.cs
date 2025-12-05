using UnityEngine;

namespace TheHunt.Inventory
{
    public interface IEquippable
    {
        void Equip(GameObject user);
        void Unequip(GameObject user);
        GameObject EquipPrefab { get; }
    }
}
