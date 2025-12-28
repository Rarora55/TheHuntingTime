using UnityEngine;

public interface IWeaponReloading
{
    bool CanReload();
    void Reload();
    
    event System.Action OnReloadComplete;
}
