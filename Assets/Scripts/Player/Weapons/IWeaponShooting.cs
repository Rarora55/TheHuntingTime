using UnityEngine;

public interface IWeaponShooting
{
    bool CanShoot();
    void Shoot();
    
    event System.Action OnWeaponFired;
    event System.Action OnWeaponEmpty;
}
