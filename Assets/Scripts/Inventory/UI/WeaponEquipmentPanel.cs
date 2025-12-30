using UnityEngine;
using TheHunt.Utilities;

namespace TheHunt.Inventory
{
    public class WeaponEquipmentPanel : MonoBehaviour
    {
        [Header("References")]
        [SerializeField] private WeaponInventoryManager weaponManager;
        [SerializeField] private InventoryUIController uiController;
        [SerializeField] private PlayerWeaponController playerWeaponController;

        [Header("Weapon Slots")]
        [SerializeField] private WeaponSlotUI primarySlot;
        [SerializeField] private WeaponSlotUI secondarySlot;

        [Header("Settings")]
        [SerializeField] private bool autoHideWhenEmpty = false;
        [SerializeField] private CanvasGroup canvasGroup;
        
        private ComponentAutoAssigner autoAssigner;

        private void Awake()
        {
            autoAssigner = new ComponentAutoAssigner();
            
            weaponManager = autoAssigner.GetOrFindComponent(weaponManager);
            uiController = autoAssigner.GetOrFindComponent(uiController);
            playerWeaponController = autoAssigner.GetOrFindComponent(playerWeaponController);
            canvasGroup = canvasGroup != null ? canvasGroup : GetComponent<CanvasGroup>();

            ValidateSlots();
        }

        private void OnEnable()
        {
            if (weaponManager != null)
            {
                weaponManager.OnWeaponEquipped += OnWeaponEquipped;
                weaponManager.OnWeaponUnequipped += OnWeaponUnequipped;
                weaponManager.OnWeaponsSwapped += OnWeaponsSwapped;
            }
            
            if (playerWeaponController != null)
            {
                playerWeaponController.OnAmmoChanged += OnAmmoChanged;
            }
            else
            {
                Debug.LogWarning("<color=yellow>[WEAPON EQUIPMENT PANEL] PlayerWeaponController is NULL! Cannot subscribe to OnAmmoChanged</color>");
            }

            RefreshAllSlots();
        }

        private void OnDisable()
        {
            if (weaponManager != null)
            {
                weaponManager.OnWeaponEquipped -= OnWeaponEquipped;
                weaponManager.OnWeaponUnequipped -= OnWeaponUnequipped;
                weaponManager.OnWeaponsSwapped -= OnWeaponsSwapped;
            }
            
            if (playerWeaponController != null)
            {
                playerWeaponController.OnAmmoChanged -= OnAmmoChanged;
            }
        }

        private void ValidateSlots()
        {
            if (primarySlot == null)
            {
                Debug.LogError("<color=red>[WEAPON EQUIPMENT PANEL] Primary slot not assigned!</color>");
            }

            if (secondarySlot == null)
            {
                Debug.LogError("<color=red>[WEAPON EQUIPMENT PANEL] Secondary slot not assigned!</color>");
            }

            if (primarySlot != null && primarySlot.SlotType != EquipSlot.Primary)
            {
                Debug.LogWarning("<color=yellow>[WEAPON EQUIPMENT PANEL] Primary slot has wrong slot type!</color>");
            }

            if (secondarySlot != null && secondarySlot.SlotType != EquipSlot.Secondary)
            {
                Debug.LogWarning("<color=yellow>[WEAPON EQUIPMENT PANEL] Secondary slot has wrong slot type!</color>");
            }
        }

        private void OnWeaponEquipped(EquipSlot slot, WeaponItemData weapon)
        {
            WeaponSlotUI slotUI = GetSlotUI(slot);
            
            if (slotUI != null)
            {
                slotUI.EquipWeapon(weapon);
                
                if (playerWeaponController != null && slot == playerWeaponController.ActiveWeaponSlot)
                {
                    slotUI.UpdateAmmoDisplay(playerWeaponController.CurrentMagazineAmmo, playerWeaponController.ReserveAmmo);
                    Debug.Log($"<color=cyan>[WEAPON EQUIPMENT PANEL] Force updated ammo after equipping: {playerWeaponController.CurrentMagazineAmmo}/{playerWeaponController.ReserveAmmo}</color>");
                }
                
                slotUI.Pulse();
            }

            UpdateVisibility();
        }

        private void OnWeaponUnequipped(EquipSlot slot)
        {
            WeaponSlotUI slotUI = GetSlotUI(slot);
            
            if (slotUI != null)
            {
                slotUI.UnequipWeapon();
            }

            UpdateVisibility();
        }

        private void OnWeaponsSwapped()
        {
            RefreshAllSlots();
        }
        
        private void OnAmmoChanged(int magazine, int reserve)
        {
            Debug.Log($"<color=cyan>[WEAPON EQUIPMENT PANEL] OnAmmoChanged called! Magazine: {magazine}, Reserve: {reserve}</color>");
            
            if (playerWeaponController == null)
            {
                Debug.LogWarning("<color=yellow>[WEAPON EQUIPMENT PANEL] playerWeaponController is NULL in OnAmmoChanged!</color>");
                return;
            }
                
            WeaponSlotUI activeSlot = GetSlotUI(playerWeaponController.ActiveWeaponSlot);
            
            if (activeSlot != null)
            {
                Debug.Log($"<color=cyan>[WEAPON EQUIPMENT PANEL] Updating {playerWeaponController.ActiveWeaponSlot} slot UI</color>");
                activeSlot.UpdateAmmoDisplay(magazine, reserve);
            }
            else
            {
                Debug.LogWarning($"<color=yellow>[WEAPON EQUIPMENT PANEL] Active slot UI is NULL for {playerWeaponController.ActiveWeaponSlot}</color>");
            }
        }

        public void RefreshAllSlots()
        {
            if (weaponManager == null)
                return;

            if (primarySlot != null)
            {
                WeaponItemData primaryWeapon = weaponManager.GetEquippedWeapon(EquipSlot.Primary);
                if (primaryWeapon != null)
                {
                    primarySlot.EquipWeapon(primaryWeapon);
                }
                else
                {
                    primarySlot.UnequipWeapon();
                }
            }

            if (secondarySlot != null)
            {
                WeaponItemData secondaryWeapon = weaponManager.GetEquippedWeapon(EquipSlot.Secondary);
                if (secondaryWeapon != null)
                {
                    secondarySlot.EquipWeapon(secondaryWeapon);
                }
                else
                {
                    secondarySlot.UnequipWeapon();
                }
            }

            UpdateVisibility();
        }

        public void SelectSlot(EquipSlot slot)
        {
            if (primarySlot != null)
            {
                primarySlot.SetSelected(slot == EquipSlot.Primary);
            }

            if (secondarySlot != null)
            {
                secondarySlot.SetSelected(slot == EquipSlot.Secondary);
            }
        }

        public void ClearSelection()
        {
            if (primarySlot != null)
            {
                primarySlot.SetSelected(false);
            }

            if (secondarySlot != null)
            {
                secondarySlot.SetSelected(false);
            }
        }

        private WeaponSlotUI GetSlotUI(EquipSlot slot)
        {
            return slot == EquipSlot.Primary ? primarySlot : secondarySlot;
        }

        private void UpdateVisibility()
        {
            if (!autoHideWhenEmpty || canvasGroup == null)
                return;

            bool hasAnyWeapon = (primarySlot != null && primarySlot.HasWeapon) || 
                                (secondarySlot != null && secondarySlot.HasWeapon);

            canvasGroup.alpha = hasAnyWeapon ? 1f : 0.5f;
        }

        public bool HasWeaponInSlot(EquipSlot slot)
        {
            WeaponSlotUI slotUI = GetSlotUI(slot);
            return slotUI != null && slotUI.HasWeapon;
        }

        public WeaponItemData GetWeaponInSlot(EquipSlot slot)
        {
            WeaponSlotUI slotUI = GetSlotUI(slot);
            return slotUI?.EquippedWeapon;
        }
    }
}
