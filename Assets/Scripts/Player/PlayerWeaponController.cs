using UnityEngine;
using TheHunt.Inventory;

public class PlayerWeaponController : MonoBehaviour, IWeaponState, IWeaponShooting, IWeaponReloading
{
    #region Dependencies
    [Header("References")]
    [SerializeField] private WeaponInventoryManager weaponManager;
    [SerializeField] private InventorySystem inventorySystem;
    
    [Header("Shooting Settings")]
    [SerializeField] private Transform firePoint;
    [SerializeField] private float fireRate = 0.5f;
    #endregion
    
    #region State
    [Header("Current Weapon State")]
    [SerializeField] private EquipSlot activeWeaponSlot = EquipSlot.Primary;
    private int currentMagazineAmmo;
    private float nextFireTime;
    #endregion
    
    #region Public Properties (IWeaponState)
    public WeaponItemData ActiveWeapon => weaponManager.GetEquippedWeapon(activeWeaponSlot);
    public int CurrentMagazineAmmo => currentMagazineAmmo;
    public int ReserveAmmo => ActiveWeapon != null ? inventorySystem.GetAmmoCount(ActiveWeapon.RequiredAmmo) : 0;
    public EquipSlot ActiveWeaponSlot => activeWeaponSlot;
    #endregion
    
    #region Events
    public event System.Action<int, int> OnAmmoChanged;
    public event System.Action OnWeaponFired;
    public event System.Action OnWeaponEmpty;
    public event System.Action OnReloadComplete;
    #endregion
    
    #region Unity Lifecycle
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
            weaponManager.OnWeaponEquipped += HandleWeaponEquipped;
            weaponManager.OnWeaponUnequipped += HandleWeaponUnequipped;
        }
    }
    
    void OnDisable()
    {
        if (weaponManager != null)
        {
            weaponManager.OnWeaponEquipped -= HandleWeaponEquipped;
            weaponManager.OnWeaponUnequipped -= HandleWeaponUnequipped;
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
        NotifyAmmoChanged();
    }
    #endregion
    
    #region Weapon State Management
    void InitializeWeapon()
    {
        if (ActiveWeapon != null)
        {
            currentMagazineAmmo = ActiveWeapon.MagazineSize;
            NotifyAmmoChanged();
        }
    }
    
    public void SwitchWeapon(EquipSlot slot)
    {
        if (slot == activeWeaponSlot)
            return;
            
        activeWeaponSlot = slot;
        InitializeWeapon();
    }
    
    public void SwapWeapons()
    {
        activeWeaponSlot = activeWeaponSlot == EquipSlot.Primary ? EquipSlot.Secondary : EquipSlot.Primary;
        InitializeWeapon();
    }
    
    void HandleWeaponEquipped(EquipSlot slot, WeaponItemData weapon)
    {
        if (slot == activeWeaponSlot)
        {
            InitializeWeapon();
        }
    }
    
    void HandleWeaponUnequipped(EquipSlot slot)
    {
        if (slot == activeWeaponSlot)
        {
            currentMagazineAmmo = 0;
            NotifyAmmoChanged();
        }
    }
    
    void NotifyAmmoChanged()
    {
        OnAmmoChanged?.Invoke(currentMagazineAmmo, ReserveAmmo);
    }
    #endregion
    
    #region Shooting System (IWeaponShooting)
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
            }
            return;
        }
        
        currentMagazineAmmo -= ActiveWeapon.AmmoPerShot;
        nextFireTime = Time.time + fireRate;
        
        PerformShot();
        
        NotifyAmmoChanged();
        OnWeaponFired?.Invoke();
    }
    
    void PerformShot()
    {
        if (ActiveWeapon == null)
            return;
    }
    #endregion
    
    #region Reload System (IWeaponReloading)
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
            return;
        }
        
        int ammoNeeded = ActiveWeapon.MagazineSize - currentMagazineAmmo;
        int ammoToReload = Mathf.Min(ammoNeeded, ReserveAmmo);
        
        if (inventorySystem.ConsumeAmmo(ActiveWeapon.RequiredAmmo, ammoToReload))
        {
            currentMagazineAmmo += ammoToReload;
            OnReloadComplete?.Invoke();
            NotifyAmmoChanged();
        }
    }
    #endregion
    
    #region Debug Visualization
    void OnDrawGizmosSelected()
    {
        if (firePoint != null && ActiveWeapon != null)
        {
            Gizmos.color = Color.red;
            Gizmos.DrawWireSphere(firePoint.position, 0.1f);
            Gizmos.DrawLine(firePoint.position, firePoint.position + firePoint.right * ActiveWeapon.AttackRange);
        }
    }
    #endregion
}
