using UnityEngine;
using TheHunt.Inventory;

public interface IWeaponState
{
    WeaponItemData ActiveWeapon { get; }
    int CurrentMagazineAmmo { get; }
    int ReserveAmmo { get; }
    EquipSlot ActiveWeaponSlot { get; }
    
    event System.Action<int, int> OnAmmoChanged;
}
