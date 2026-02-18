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
        [SerializeField] private Color emptySlotColor = new Color(0.2f, 0.2f, 0.2f, 1f);
        [SerializeField] private Color equippedSlotColor = new Color(0.2f, 0.6f, 0.2f, 1f);
        [SerializeField] private Color selectedSlotColor = new Color(1f, 1f, 1f, 1f);
        [SerializeField] private Sprite emptyWeaponSprite;

        private WeaponItemData equippedWeapon;
        private bool isSelected;
        private int magazineAmmo;
        private int reserveAmmo;
        private Outline selectionOutline;

        public EquipSlot SlotType => slotType;
        public WeaponItemData EquippedWeapon => equippedWeapon;
        public bool HasWeapon => equippedWeapon != null;
        public bool IsSelected => isSelected;

        private void Awake()
        {
            // Asegurar que slotBackground existe
            if (slotBackground == null)
            {
                Debug.LogError($"<color=red>[WEAPON SLOT UI] {slotType} - slotBackground is NOT assigned in Inspector!</color>");
            }
            
            // Añadir Outline component para highlight de selección
            selectionOutline = slotBackground?.gameObject.GetComponent<Outline>();
            if (selectionOutline == null && slotBackground != null)
            {
                selectionOutline = slotBackground.gameObject.AddComponent<Outline>();
                selectionOutline.effectColor = new Color(1f, 1f, 1f, 1f); // BLANCO brillante
                selectionOutline.effectDistance = new Vector2(8f, 8f); // Outline MUY grueso
                selectionOutline.enabled = false; // Desactivado por defecto
                
                Debug.Log($"<color=green>[WEAPON SLOT UI] {slotType} - Outline component added successfully!</color>");
            }
            
            InitializeSlot();
        }

        private void InitializeSlot()
        {
            // Asegurar que el background tenga un color visible desde el inicio
            if (slotBackground != null && slotBackground.color.a < 0.5f)
            {
                slotBackground.color = emptySlotColor;
            }
            
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
        }
        
        public void UpdateAmmoDisplay(int magazine, int reserve)
        {
            magazineAmmo = magazine;
            reserveAmmo = reserve;
            
            if (ammoText != null)
            {
                if (equippedWeapon == null)
                {
                    ammoText.text = "";
                    ammoText.enabled = false;
                }
                else if (equippedWeapon.RequiredAmmo == AmmoType.None)
                {
                    ammoText.text = "∞";
                    ammoText.enabled = true;
                }
                else
                {
                    ammoText.text = $"{magazineAmmo}/{reserveAmmo}";
                    ammoText.enabled = true;
                }
            }
            else
            {
                Debug.LogWarning($"<color=red>[WEAPON SLOT UI] ammoText is NULL! Cannot update ammo display!</color>");
            }
        }

        public void UnequipWeapon()
        {
            equippedWeapon = null;
            ClearSlot();
        }

        public void SetSelected(bool selected)
        {
            isSelected = selected;
            
            // Activar/desactivar outline para mejor visibilidad
            if (selectionOutline != null)
            {
                selectionOutline.enabled = selected;
            }
            
            UpdateVisualState();
            
            if (selected)
            {
                Debug.Log($"<color=orange>[WEAPON SLOT UI] {slotType} slot SELECTED - Color: {selectedSlotColor}, Outline: {selectionOutline != null && selectionOutline.enabled}</color>");
            }
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
                ammoText.text = "";
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
            {
                Debug.LogWarning($"<color=red>[WEAPON SLOT UI] {slotType} - slotBackground is NULL!</color>");
                return;
            }

            Color targetColor;
            Vector3 targetScale = Vector3.one;
            
            if (isSelected)
            {
                targetColor = selectedSlotColor;
                targetScale = Vector3.one * 1.3f; // Mucho más grande - 30% más grande
            }
            else if (HasWeapon)
            {
                targetColor = equippedSlotColor;
            }
            else
            {
                targetColor = emptySlotColor;
            }
            
            slotBackground.color = targetColor;
            transform.localScale = targetScale;
            
            Debug.Log($"<color=cyan>[WEAPON SLOT UI] {slotType} visual updated - Selected: {isSelected}, HasWeapon: {HasWeapon}, Color: {targetColor}, Scale: {targetScale}</color>");
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
