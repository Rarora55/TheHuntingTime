using UnityEngine;
using UnityEngine.UI;
using TMPro;

namespace TheHunt.Inventory
{
    public class InventorySlotUI : MonoBehaviour
    {
        [Header("UI References")]
        [SerializeField] private Image iconImage;
        [SerializeField] private TextMeshProUGUI quantityText;
        [SerializeField] private Image highlightImage;
        [SerializeField] private Image backgroundImage;

        [Header("Visual Settings")]
        [SerializeField] private Color normalColor = new Color(0.2f, 0.2f, 0.2f, 0.8f);
        [SerializeField] private Color highlightColor = new Color(1f, 1f, 0f, 1f);
        [SerializeField] private Color emptyIconColor = new Color(1f, 1f, 1f, 0.3f);
        [SerializeField] private Color fullIconColor = new Color(1f, 1f, 1f, 1f);
        [SerializeField] private Color disabledColor = new Color(0.3f, 0.3f, 0.3f, 0.3f);

        private int slotIndex;
        private bool isHighlighted = false;
        private bool isDisabled = false;
        private WeaponInventoryManager weaponManager;
        private CanvasGroup canvasGroup;

        public int SlotIndex => slotIndex;
        public bool IsDisabled => isDisabled;

        private void Awake()
        {
            GameObject player = GameObject.FindGameObjectWithTag("Player");
            if (player != null)
            {
                weaponManager = player.GetComponent<WeaponInventoryManager>();
            }

            canvasGroup = GetComponent<CanvasGroup>();
            if (canvasGroup == null)
            {
                canvasGroup = gameObject.AddComponent<CanvasGroup>();
            }
        }

        public void Initialize(int index)
        {
            slotIndex = index;
            ClearSlot();
            Unhighlight();
        }

        public void UpdateSlot(ItemInstance item)
        {
            if (item == null || item.itemData == null)
            {
                ClearSlot();
                EnableSlot();
                return;
            }

            bool isWeapon = item.itemData is WeaponItemData;
            bool isEquipped = false;

            if (isWeapon && weaponManager != null)
            {
                isEquipped = weaponManager.IsWeaponEquipped(item.itemData as WeaponItemData);
            }

            if (isEquipped)
            {
                ClearSlot();
                DisableSlot();
                return;
            }

            EnableSlot();

            if (iconImage != null)
            {
                iconImage.gameObject.SetActive(true);
                iconImage.sprite = item.itemData.ItemIcon;
                iconImage.color = fullIconColor;
                iconImage.enabled = true;
            }

            if (quantityText != null)
            {
                bool isAmmo = item.itemData is AmmoItemData;
                
                if (item.quantity > 1 || isAmmo)
                {
                    quantityText.gameObject.SetActive(true);
                    quantityText.text = $"x{item.quantity}";
                    quantityText.enabled = true;
                }
                else
                {
                    quantityText.gameObject.SetActive(false);
                    quantityText.enabled = false;
                }
            }
        }

        public void ClearSlot()
        {
            if (iconImage != null)
            {
                iconImage.gameObject.SetActive(false);
                iconImage.sprite = null;
                iconImage.color = emptyIconColor;
                iconImage.enabled = false;
            }

            if (quantityText != null)
            {
                quantityText.gameObject.SetActive(false);
                quantityText.enabled = false;
            }
        }

        public void Highlight()
        {
            if (isDisabled)
                return;

            isHighlighted = true;
            
            if (highlightImage != null)
            {
                highlightImage.enabled = true;
                highlightImage.color = highlightColor;
            }

            if (backgroundImage != null)
            {
                backgroundImage.color = highlightColor * 0.3f;
            }
        }

        public void Unhighlight()
        {
            isHighlighted = false;
            
            if (highlightImage != null)
            {
                highlightImage.enabled = false;
            }

            if (backgroundImage != null)
            {
                backgroundImage.color = normalColor;
            }
        }

        private void DisableSlot()
        {
            isDisabled = true;
            
            if (canvasGroup != null)
            {
                canvasGroup.alpha = 0.3f;
                canvasGroup.interactable = false;
                canvasGroup.blocksRaycasts = false;
            }

            if (backgroundImage != null)
            {
                backgroundImage.color = disabledColor;
            }
        }

        private void EnableSlot()
        {
            isDisabled = false;
            
            if (canvasGroup != null)
            {
                canvasGroup.alpha = 1f;
                canvasGroup.interactable = true;
                canvasGroup.blocksRaycasts = true;
            }

            if (backgroundImage != null && !isHighlighted)
            {
                backgroundImage.color = normalColor;
            }
        }
    }
}
