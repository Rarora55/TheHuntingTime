using UnityEngine;

namespace TheHunt.Inventory
{
    public class WeaponEquipmentPanel : MonoBehaviour
    {
        [Header("References")]
        [SerializeField] private WeaponInventoryManager weaponManager;
        [SerializeField] private InventoryUIController uiController;

        [Header("Weapon Slots")]
        [SerializeField] private WeaponSlotUI primarySlot;
        [SerializeField] private WeaponSlotUI secondarySlot;

        [Header("Settings")]
        [SerializeField] private bool autoHideWhenEmpty = false;
        [SerializeField] private CanvasGroup canvasGroup;

        private void Awake()
        {
            if (weaponManager == null)
            {
                weaponManager = FindFirstObjectByType<WeaponInventoryManager>();
            }

            if (uiController == null)
            {
                uiController = FindFirstObjectByType<InventoryUIController>();
            }

            if (canvasGroup == null)
            {
                canvasGroup = GetComponent<CanvasGroup>();
            }

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
