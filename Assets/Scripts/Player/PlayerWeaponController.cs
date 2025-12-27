using UnityEngine;
using TheHunt.Inventory;

public class PlayerWeaponController : MonoBehaviour
{
    [Header("References")]
    [SerializeField] private WeaponInventoryManager weaponManager;
    [SerializeField] private InventorySystem inventorySystem;
    
    [Header("Current Weapon State")]
    [SerializeField] private EquipSlot activeWeaponSlot = EquipSlot.Primary;
    private int currentMagazineAmmo;
    
    [Header("Shooting Settings")]
    [SerializeField] private Transform firePoint;
    [SerializeField] private float fireRate = 0.5f;
    private float nextFireTime;
    
    public WeaponItemData ActiveWeapon => weaponManager.GetEquippedWeapon(activeWeaponSlot);
    public int CurrentMagazineAmmo => currentMagazineAmmo;
    public int ReserveAmmo => ActiveWeapon != null ? inventorySystem.GetAmmoCount(ActiveWeapon.RequiredAmmo) : 0;
    public EquipSlot ActiveWeaponSlot => activeWeaponSlot;
    
    public System.Action<int, int> OnAmmoChanged;
    public System.Action OnWeaponFired;
    public System.Action OnWeaponEmpty;
    public System.Action OnReloadComplete;
    
    void Awake()
    {
        if (weaponManager == null)
            weaponManager = GetComponent<WeaponInventoryManager>();
            
        if (inventorySystem == null)
            inventorySystem = GetComponent<InventorySystem>();
    }
    
    void OnEnable()
    {
        if (weaponManager != null)
        {
            weaponManager.OnWeaponEquipped += OnWeaponEquipped;
            weaponManager.OnWeaponUnequipped += OnWeaponUnequipped;
        }
    }
    
    void OnDisable()
    {
        if (weaponManager != null)
        {
            weaponManager.OnWeaponEquipped -= OnWeaponEquipped;
            weaponManager.OnWeaponUnequipped -= OnWeaponUnequipped;
        }
    }
    
    void Start()
    {
        InitializeWeapon();
        
        StartCoroutine(DelayedUIUpdate());
    }
    
    System.Collections.IEnumerator DelayedUIUpdate()
    {
        yield return new WaitForSeconds(0.5f);
        
        Debug.Log("<color=cyan>[WEAPON CONTROLLER] Forcing initial UI update...</color>");
        NotifyAmmoChanged();
    }
    
    void InitializeWeapon()
    {
        Debug.Log($"<color=cyan>[WEAPON CONTROLLER] InitializeWeapon called. ActiveWeapon: {ActiveWeapon?.ItemName ?? "NULL"}</color>");
        
        if (ActiveWeapon != null)
        {
            currentMagazineAmmo = ActiveWeapon.MagazineSize;
            Debug.Log($"<color=green>[WEAPON CONTROLLER] ✓ Weapon initialized: Magazine set to {currentMagazineAmmo}/{ActiveWeapon.MagazineSize}</color>");
            NotifyAmmoChanged();
        }
        else
        {
            Debug.LogWarning("<color=yellow>[WEAPON CONTROLLER] Cannot initialize weapon - ActiveWeapon is NULL</color>");
        }
    }
    
    public bool CanShoot()
    {
        if (ActiveWeapon == null)
            return false;
            
        if (Time.time < nextFireTime)
            return false;
            
        if (currentMagazineAmmo <= 0)
            return false;
            
        return true;
    }
    
    public void Shoot()
    {
        if (!CanShoot())
        {
            if (ActiveWeapon != null && currentMagazineAmmo <= 0)
            {
                OnWeaponEmpty?.Invoke();
                Debug.Log("<color=yellow>[WEAPON] Magazine empty! Press R to reload</color>");
            }
            return;
        }
        
        currentMagazineAmmo -= ActiveWeapon.AmmoPerShot;
        nextFireTime = Time.time + fireRate;
        
        PerformShot();
        
        NotifyAmmoChanged();
        OnWeaponFired?.Invoke();
        
        Debug.Log($"<color=green>[WEAPON] Fired! Magazine: {currentMagazineAmmo}/{ActiveWeapon.MagazineSize} | Reserve: {ReserveAmmo}</color>");
    }
    
    void PerformShot()
    {
        if (ActiveWeapon == null)
            return;
        
        Debug.Log($"<color=cyan>[WEAPON] Shot performed with {ActiveWeapon.ItemName}</color>");
    }
    
    public bool CanReload()
    {
        if (ActiveWeapon == null)
            return false;
            
        if (currentMagazineAmmo >= ActiveWeapon.MagazineSize)
            return false;
            
        if (ActiveWeapon.RequiredAmmo == AmmoType.None)
            return false;
            
        if (ReserveAmmo <= 0)
            return false;
            
        return true;
    }
    
    public void Reload()
    {
        if (!CanReload())
        {
            if (ReserveAmmo <= 0)
            {
                Debug.Log($"<color=red>[WEAPON] No reserve ammo for {ActiveWeapon.RequiredAmmo}!</color>");
            }
            return;
        }
        
        int ammoNeeded = ActiveWeapon.MagazineSize - currentMagazineAmmo;
        int ammoToReload = Mathf.Min(ammoNeeded, ReserveAmmo);
        
        if (inventorySystem.ConsumeAmmo(ActiveWeapon.RequiredAmmo, ammoToReload))
        {
            currentMagazineAmmo += ammoToReload;
            OnReloadComplete?.Invoke();
            NotifyAmmoChanged();
            
            Debug.Log($"<color=green>[WEAPON] Reloaded! Magazine: {currentMagazineAmmo}/{ActiveWeapon.MagazineSize} | Reserve: {ReserveAmmo}</color>");
        }
    }
    
    public void SwitchWeapon(EquipSlot slot)
    {
        if (slot == activeWeaponSlot)
            return;
            
        activeWeaponSlot = slot;
        InitializeWeapon();
        
        Debug.Log($"<color=cyan>[WEAPON] Switched to {activeWeaponSlot} weapon</color>");
    }
    
    public void SwapWeapons()
    {
        activeWeaponSlot = activeWeaponSlot == EquipSlot.Primary ? EquipSlot.Secondary : EquipSlot.Primary;
        InitializeWeapon();
        
        Debug.Log($"<color=cyan>[WEAPON] Swapped to {activeWeaponSlot} weapon</color>");
    }
    
    void OnWeaponEquipped(EquipSlot slot, WeaponItemData weapon)
    {
        Debug.Log($"<color=cyan>[WEAPON CONTROLLER] OnWeaponEquipped: slot={slot}, weapon={weapon?.ItemName ?? "NULL"}, activeWeaponSlot={activeWeaponSlot}</color>");
        
        if (slot == activeWeaponSlot)
        {
            Debug.Log($"<color=green>[WEAPON CONTROLLER] ✓ Weapon equipped to active slot - Initializing...</color>");
            InitializeWeapon();
        }
        else
        {
            Debug.Log($"<color=yellow>[WEAPON CONTROLLER] Weapon equipped to non-active slot - Skipping initialization</color>");
        }
    }
    
    void OnWeaponUnequipped(EquipSlot slot)
    {
        if (slot == activeWeaponSlot)
        {
            currentMagazineAmmo = 0;
            NotifyAmmoChanged();
        }
    }
    
    void NotifyAmmoChanged()
    {
        Debug.Log($"<color=magenta>[WEAPON CONTROLLER] NotifyAmmoChanged: Magazine={currentMagazineAmmo}, Reserve={ReserveAmmo}</color>");
        
        if (OnAmmoChanged != null)
        {
            Debug.Log($"<color=magenta>[WEAPON CONTROLLER] ✓ Invoking OnAmmoChanged event (subscribers: {OnAmmoChanged.GetInvocationList().Length})</color>");
            OnAmmoChanged?.Invoke(currentMagazineAmmo, ReserveAmmo);
        }
        else
        {
            Debug.LogWarning("<color=yellow>[WEAPON CONTROLLER] OnAmmoChanged has no subscribers!</color>");
        }
    }
    
    void OnDrawGizmosSelected()
    {
        if (firePoint != null && ActiveWeapon != null)
        {
            Gizmos.color = Color.red;
            Gizmos.DrawWireSphere(firePoint.position, 0.1f);
            Gizmos.DrawLine(firePoint.position, firePoint.position + firePoint.right * ActiveWeapon.AttackRange);
        }
    }
}
