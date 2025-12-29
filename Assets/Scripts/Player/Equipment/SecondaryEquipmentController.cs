using UnityEngine;
using TheHunt.Inventory;

namespace TheHunt.Equipment
{
    public class SecondaryEquipmentController : MonoBehaviour
    {
        [Header("References")]
        [SerializeField] private WeaponInventoryManager weaponManager;
        [SerializeField] private FlashlightController flashlight;
        
        private bool isFlashlightEquipped;
        
        public bool IsFlashlightEquipped => isFlashlightEquipped;
        public FlashlightController Flashlight => flashlight;
        
        void Awake()
        {
            if (weaponManager == null)
            {
                weaponManager = GetComponent<WeaponInventoryManager>();
            }
            
            if (flashlight == null)
            {
                flashlight = GetComponentInChildren<FlashlightController>();
            }
            
            if (flashlight != null)
            {
                flashlight.gameObject.SetActive(false);
            }
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
        
        void HandleWeaponEquipped(EquipSlot slot, WeaponItemData weapon)
        {
            if (slot != EquipSlot.Secondary)
                return;
            
            if (IsFlashlightWeapon(weapon))
            {
                EquipFlashlight();
            }
            else
            {
                UnequipFlashlight();
            }
        }
        
        void HandleWeaponUnequipped(EquipSlot slot)
        {
            if (slot == EquipSlot.Secondary)
            {
                UnequipFlashlight();
            }
        }
        
        bool IsFlashlightWeapon(WeaponItemData weapon)
        {
            if (weapon == null)
                return false;
            
            return weapon.WeaponType == WeaponType.Tool && 
                   weapon.ToolType == ToolType.Flashlight;
        }
        
        void EquipFlashlight()
        {
            if (flashlight == null)
            {
                Debug.LogWarning($"<color=yellow>[SECONDARY EQUIPMENT] Flashlight reference missing!</color>");
                return;
            }
            
            isFlashlightEquipped = true;
            flashlight.gameObject.SetActive(true);
            
            Debug.Log($"<color=green>[SECONDARY EQUIPMENT] Flashlight equipped</color>");
        }
        
        void UnequipFlashlight()
        {
            if (flashlight == null)
                return;
            
            isFlashlightEquipped = false;
            flashlight.TurnOff();
            flashlight.gameObject.SetActive(false);
            
            Debug.Log($"<color=yellow>[SECONDARY EQUIPMENT] Flashlight unequipped</color>");
        }
        
        public void ToggleFlashlight()
        {
            if (!isFlashlightEquipped)
            {
                Debug.Log($"<color=yellow>[SECONDARY EQUIPMENT] Flashlight not equipped</color>");
                return;
            }
            
            flashlight.Toggle();
        }
    }
}
