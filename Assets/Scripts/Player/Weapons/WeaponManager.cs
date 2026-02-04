using UnityEngine;

public class WeaponManager : MonoBehaviour
{
    [Header("Dependencies")]
    [SerializeField] private BulletPool bulletPool;
    
    [Header("Weapons")]
    [SerializeField] private Weapon[] weapons;
    
    private int currentWeaponIndex = 0;
    
    void Awake()
    {
        InitializeWeapons();
    }
    
    void InitializeWeapons()
    {
        if (bulletPool == null)
        {
            Debug.LogError("[WEAPON MANAGER] BulletPool not assigned!");
            return;
        }
        
        if (weapons == null || weapons.Length == 0)
        {
            weapons = GetComponentsInChildren<Weapon>(true);
        }
        
        foreach (Weapon weapon in weapons)
        {
            if (weapon != null)
            {
                weapon.SetBulletPool(bulletPool);
            }
        }
        
        ActivateWeapon(0);
    }
    
    public void ActivateWeapon(int index)
    {
        if (weapons == null || weapons.Length == 0)
            return;
        
        index = Mathf.Clamp(index, 0, weapons.Length - 1);
        
        for (int i = 0; i < weapons.Length; i++)
        {
            if (weapons[i] != null)
            {
                weapons[i].gameObject.SetActive(i == index);
            }
        }
        
        currentWeaponIndex = index;
    }
    
    public void SwitchToNextWeapon()
    {
        int nextIndex = (currentWeaponIndex + 1) % weapons.Length;
        ActivateWeapon(nextIndex);
    }
    
    public void SwitchToPreviousWeapon()
    {
        int prevIndex = (currentWeaponIndex - 1 + weapons.Length) % weapons.Length;
        ActivateWeapon(prevIndex);
    }
    
    public Weapon GetCurrentWeapon()
    {
        if (weapons != null && currentWeaponIndex < weapons.Length)
            return weapons[currentWeaponIndex];
        
        return null;
    }
    
    public void SetBulletPool(BulletPool pool)
    {
        bulletPool = pool;
        InitializeWeapons();
    }
}
