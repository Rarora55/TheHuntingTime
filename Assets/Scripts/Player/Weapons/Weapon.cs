using System;
using UnityEngine;

public class Weapon : MonoBehaviour, IWeaponShooting
{
    [Header("Weapon Configuration")]
    [SerializeField] private BulletData bulletData;
    [SerializeField] private Transform firePoint;
    [SerializeField] private BulletPool bulletPool;
    
    [Header("Fire Rate")]
    [SerializeField] private float fireRate = 0.2f;
    [SerializeField] private bool automaticFire = true;
    
    [Header("Ammo (Optional)")]
    [SerializeField] private bool hasAmmo = false;
    [SerializeField] private int currentAmmo = 30;
    [SerializeField] private int maxAmmo = 30;
    
    private float lastFireTime;
    private bool isFiring;
    
    public event Action OnWeaponFired;
    public event Action OnWeaponEmpty;
    
    public bool HasAmmo => !hasAmmo || currentAmmo > 0;
    public int CurrentAmmo => currentAmmo;
    public int MaxAmmo => maxAmmo;
    
    void Awake()
    {
        if (firePoint == null)
        {
            GameObject firePointObj = new GameObject("FirePoint");
            firePoint = firePointObj.transform;
            firePoint.SetParent(transform);
            firePoint.localPosition = Vector3.right;
        }
    }
    
    void Update()
    {
        if (automaticFire && isFiring && CanShoot())
        {
            Shoot();
        }
    }
    
    public bool CanShoot()
    {
        if (bulletPool == null)
        {
            Debug.LogWarning("[WEAPON] BulletPool not assigned!");
            return false;
        }
        
        if (Time.time - lastFireTime < fireRate)
            return false;
        
        if (hasAmmo && currentAmmo <= 0)
            return false;
        
        return true;
    }
    
    public void Shoot()
    {
        if (!CanShoot())
        {
            if (hasAmmo && currentAmmo <= 0)
                OnWeaponEmpty?.Invoke();
            return;
        }
        
        Vector2 fireDirection = firePoint.right;
        
        Bullet bullet = bulletPool.GetBullet(bulletData, firePoint.position, fireDirection, gameObject);
        
        if (bullet != null)
        {
            lastFireTime = Time.time;
            
            if (hasAmmo)
                currentAmmo--;
            
            OnWeaponFired?.Invoke();
            
            Debug.Log($"<color=green>[WEAPON] Fired {bulletData.bulletName}. Ammo: {currentAmmo}/{maxAmmo}</color>");
        }
    }
    
    public void StartFiring()
    {
        isFiring = true;
        
        if (!automaticFire && CanShoot())
            Shoot();
    }
    
    public void StopFiring()
    {
        isFiring = false;
    }
    
    public void Reload(int amount = -1)
    {
        if (!hasAmmo)
            return;
        
        int ammoToAdd = amount < 0 ? maxAmmo - currentAmmo : Mathf.Min(amount, maxAmmo - currentAmmo);
        currentAmmo += ammoToAdd;
        
        Debug.Log($"<color=cyan>[WEAPON] Reloaded. Ammo: {currentAmmo}/{maxAmmo}</color>");
    }
    
    public void SetBulletPool(BulletPool pool)
    {
        bulletPool = pool;
    }
    
    public void SetAimDirection(Vector2 direction)
    {
        if (direction != Vector2.zero)
        {
            float angle = Mathf.Atan2(direction.y, direction.x) * Mathf.Rad2Deg;
            firePoint.rotation = Quaternion.Euler(0f, 0f, angle);
        }
    }
    
    void OnDrawGizmosSelected()
    {
        if (firePoint != null)
        {
            Gizmos.color = Color.red;
            Gizmos.DrawLine(firePoint.position, firePoint.position + firePoint.right * 2f);
            Gizmos.DrawWireSphere(firePoint.position, 0.1f);
        }
    }
}
