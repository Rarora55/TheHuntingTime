using UnityEngine;
using UnityEngine.UI;
using TMPro;
using System.Collections;

namespace TheHunt.Inventory
{
    public class WeaponSlotUI : MonoBehaviour
    {
        [Header("Slot Configuration")]
        [SerializeField] private EquipSlot slotType;

        [Header("UI Elements")]
        [SerializeField] private Image slotBackground;
        [SerializeField] private Image weaponIcon;
        [SerializeField] private TextMeshProUGUI weaponNameText;
        [SerializeField] private TextMeshProUGUI slotLabel;
        [SerializeField] private TextMeshProUGUI ammoText;

        [Header("Visual States")]
        [SerializeField] private Color emptySlotColor = new Color(0.3f, 0.3f, 0.3f, 0.5f);
        [SerializeField] private Color equippedSlotColor = new Color(0.2f, 0.6f, 0.2f, 0.8f);
        [SerializeField] private Color selectedSlotColor = new Color(0.8f, 0.8f, 0.2f, 1f);
        [SerializeField] private Sprite emptyWeaponSprite;

        private WeaponItemData equippedWeapon;
        private bool isSelected;
        private int magazineAmmo;
        private int reserveAmmo;

        public EquipSlot SlotType => slotType;
        public WeaponItemData EquippedWeapon => equippedWeapon;
        public bool HasWeapon => equippedWeapon != null;
        public bool IsSelected => isSelected;

        private void Awake()
        {
            InitializeSlot();
        }

        private void InitializeSlot()
        {
            ClearSlot();
            UpdateSlotLabel();
        }

        public void EquipWeapon(WeaponItemData weapon)
        {
            if (weapon == null)
            {
                ClearSlot();
                return;
            }

            equippedWeapon = weapon;

            if (weaponIcon != null)
            {
                weaponIcon.sprite = weapon.EquipIcon != null ? weapon.EquipIcon : weapon.ItemIcon;
                weaponIcon.enabled = true;
                weaponIcon.color = Color.white;
            }

            if (weaponNameText != null)
            {
                weaponNameText.text = weapon.ItemName;
                weaponNameText.enabled = true;
            }

            UpdateVisualState();

            UpdateAmmoDisplay(0, 0);
            
            Debug.Log($"<color=green>[WEAPON SLOT UI] {slotType} slot equipped with {weapon.ItemName}</color>");
        }
        
        public void UpdateAmmoDisplay(int magazine, int reserve)
        {
            magazineAmmo = magazine;
            reserveAmmo = reserve;
            
            if (ammoText != null && equippedWeapon != null)
            {
                if (equippedWeapon.RequiredAmmo == AmmoType.None)
                {
                    ammoText.text = "∞";
                }
                else
                {
                    ammoText.text = $"{magazineAmmo}/{reserveAmmo}";
                }
                ammoText.enabled = true;
            }
        }

        public void UnequipWeapon()
        {
            equippedWeapon = null;
            ClearSlot();
            Debug.Log($"<color=orange>[WEAPON SLOT UI] {slotType} slot cleared</color>");
        }

        public void SetSelected(bool selected)
        {
            isSelected = selected;
            UpdateVisualState();
        }

        private void ClearSlot()
        {
            equippedWeapon = null;

            if (weaponIcon != null)
            {
                if (emptyWeaponSprite != null)
                {
                    weaponIcon.sprite = emptyWeaponSprite;
                    weaponIcon.enabled = true;
                    weaponIcon.color = new Color(1f, 1f, 1f, 0.3f);
                }
                else
                {
                    weaponIcon.enabled = false;
                }
            }

            if (weaponNameText != null)
            {
                weaponNameText.text = "Empty";
                weaponNameText.enabled = true;
            }
            
            if (ammoText != null)
            {
                ammoText.enabled = false;
            }

            UpdateVisualState();
        }

        private void UpdateSlotLabel()
        {
            if (slotLabel == null)
                return;

            slotLabel.text = slotType == EquipSlot.Primary ? "Primary" : "Secondary";
        }

        private void UpdateVisualState()
        {
            if (slotBackground == null)
                return;

            if (isSelected)
            {
                slotBackground.color = selectedSlotColor;
            }
            else if (HasWeapon)
            {
                slotBackground.color = equippedSlotColor;
            }
            else
            {
                slotBackground.color = emptySlotColor;
            }
        }

        public void Pulse()
        {
            if (slotBackground != null)
            {
                StopAllCoroutines();
                StartCoroutine(PulseAnimation());
            }
        }

        private IEnumerator PulseAnimation()
        {
            Vector3 originalScale = slotBackground.transform.localScale;
            Vector3 targetScale = originalScale * 1.1f;
            
            float duration = 0.15f;
            float elapsed = 0f;

            while (elapsed < duration)
            {
                elapsed += Time.deltaTime;
                float t = elapsed / duration;
                slotBackground.transform.localScale = Vector3.Lerp(originalScale, targetScale, t);
                yield return null;
            }

            elapsed = 0f;
            while (elapsed < duration)
            {
                elapsed += Time.deltaTime;
                float t = elapsed / duration;
                slotBackground.transform.localScale = Vector3.Lerp(targetScale, originalScale, t);
                yield return null;
            }

            slotBackground.transform.localScale = originalScale;
        }
    }
}
