using UnityEngine;

public class WeaponDebugger : MonoBehaviour
{
    [Header("Debug Keys")]
    [SerializeField] private KeyCode reloadKey = KeyCode.R;
    [SerializeField] private KeyCode giveAmmoKey = KeyCode.Alpha9;
    [SerializeField] private int ammoToGive = 30;
    
    [Header("Debug Info")]
    [SerializeField] private bool showDebugInfo = true;
    
    private PlayerWeaponController weaponController;
    
    void Awake()
    {
        weaponController = GetComponent<PlayerWeaponController>();
    }
    
    void Update()
    {
        if (weaponController == null)
            return;
        
        if (Input.GetKeyDown(giveAmmoKey))
        {
            GiveAmmo();
        }
        
        if (showDebugInfo && weaponController.ActiveWeapon != null)
        {
            string debugText = $"[WEAPON DEBUG] {weaponController.ActiveWeapon.ItemName} | " +
                             $"Mag: {weaponController.CurrentMagazineAmmo}/{weaponController.ActiveWeapon.MagazineSize} | " +
                             $"Reserve: {weaponController.ReserveAmmo} | " +
                             $"Can Shoot: {weaponController.CanShoot()}";
            
            Debug.Log(debugText);
        }
    }
    
    void GiveAmmo()
    {
        if (weaponController.ActiveWeapon == null)
        {
            Debug.LogWarning("[WEAPON DEBUGGER] No active weapon!");
            return;
        }
        
        weaponController.Reload();
        
        Debug.Log($"<color=green>[WEAPON DEBUGGER] Reloaded! Mag: {weaponController.CurrentMagazineAmmo}</color>");
    }
    
    void OnGUI()
    {
        if (!showDebugInfo || weaponController == null || weaponController.ActiveWeapon == null)
            return;
        
        GUIStyle style = new GUIStyle(GUI.skin.label);
        style.fontSize = 14;
        style.normal.textColor = Color.white;
        style.alignment = TextAnchor.UpperLeft;
        
        string info = $"Weapon: {weaponController.ActiveWeapon.ItemName}\n" +
                     $"Magazine: {weaponController.CurrentMagazineAmmo}/{weaponController.ActiveWeapon.MagazineSize}\n" +
                     $"Reserve: {weaponController.ReserveAmmo}\n" +
                     $"Can Shoot: {weaponController.CanShoot()}\n" +
                     $"\nPress [{giveAmmoKey}] to reload";
        
        GUI.Label(new Rect(10, 10, 400, 150), info, style);
    }
}
