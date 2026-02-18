using UnityEngine;
using UnityEngine.UI;
using TMPro;
using TheHunt.Inventory;

namespace TheHunt.Radio.UI
{
    public class RadioSlotUI : MonoBehaviour
    {
        [Header("UI Elements")]
        [SerializeField] private Image slotBackground;
        [SerializeField] private Image radioIcon;
        [SerializeField] private TextMeshProUGUI radioNameText;
        [SerializeField] private TextMeshProUGUI slotLabel;

        [Header("Visual States")]
        [SerializeField] private Color emptySlotColor = new Color(0.2f, 0.2f, 0.2f, 1f);
        [SerializeField] private Color equippedSlotColor = new Color(0.2f, 0.6f, 0.6f, 1f);
        [SerializeField] private Color selectedSlotColor = new Color(1f, 1f, 1f, 1f);
        [SerializeField] private Sprite emptyRadioSprite;

        private RadioItemData equippedRadio;
        private bool isSelected;
        private Outline selectionOutline;

        public RadioItemData EquippedRadio => equippedRadio;
        public bool HasRadio => equippedRadio != null;
        public bool IsSelected => isSelected;

        private void Awake()
        {
            if (slotBackground == null)
            {
                Debug.LogError($"<color=red>[RADIO SLOT UI] slotBackground is NOT assigned in Inspector!</color>");
            }
            
            selectionOutline = slotBackground?.gameObject.GetComponent<Outline>();
            if (selectionOutline == null && slotBackground != null)
            {
                selectionOutline = slotBackground.gameObject.AddComponent<Outline>();
                selectionOutline.effectColor = new Color(0f, 1f, 1f, 1f);
                selectionOutline.effectDistance = new Vector2(8f, 8f);
                selectionOutline.enabled = false;
                
                Debug.Log($"<color=green>[RADIO SLOT UI] Outline component added successfully!</color>");
            }
            
            InitializeSlot();
        }

        private void InitializeSlot()
        {
            if (slotBackground != null && slotBackground.color.a < 0.5f)
            {
                slotBackground.color = emptySlotColor;
            }
            
            ClearSlot();
            UpdateSlotLabel();
        }

        public void EquipRadio(RadioItemData radio)
        {
            if (radio == null)
            {
                ClearSlot();
                return;
            }

            equippedRadio = radio;

            if (radioIcon != null)
            {
                radioIcon.sprite = radio.ItemIcon;
                radioIcon.enabled = true;
                radioIcon.color = Color.white;
            }

            if (radioNameText != null)
            {
                radioNameText.text = radio.ItemName;
                radioNameText.enabled = true;
            }

            UpdateVisualState();
        }

        public void UnequipRadio()
        {
            equippedRadio = null;
            ClearSlot();
        }

        public void SetSelected(bool selected)
        {
            isSelected = selected;
            
            if (selectionOutline != null)
            {
                selectionOutline.enabled = selected;
            }
            
            UpdateVisualState();
            
            if (selected)
            {
                Debug.Log($"<color=orange>[RADIO SLOT UI] Radio slot SELECTED</color>");
            }
        }

        private void ClearSlot()
        {
            equippedRadio = null;

            if (radioIcon != null)
            {
                if (emptyRadioSprite != null)
                {
                    radioIcon.sprite = emptyRadioSprite;
                    radioIcon.enabled = true;
                    radioIcon.color = new Color(1f, 1f, 1f, 0.3f);
                }
                else
                {
                    radioIcon.enabled = false;
                }
            }

            if (radioNameText != null)
            {
                radioNameText.text = "Empty";
                radioNameText.enabled = true;
            }

            UpdateVisualState();
        }

        private void UpdateSlotLabel()
        {
            if (slotLabel == null)
                return;

            slotLabel.text = "Radio";
        }

        private void UpdateVisualState()
        {
            if (slotBackground == null)
            {
                Debug.LogWarning($"<color=red>[RADIO SLOT UI] slotBackground is NULL!</color>");
                return;
            }

            Color targetColor;
            Vector3 targetScale = Vector3.one;
            
            if (isSelected)
            {
                targetColor = selectedSlotColor;
                targetScale = Vector3.one * 1.3f;
            }
            else if (HasRadio)
            {
                targetColor = equippedSlotColor;
            }
            else
            {
                targetColor = emptySlotColor;
            }
            
            slotBackground.color = targetColor;
            transform.localScale = targetScale;
            
            Debug.Log($"<color=cyan>[RADIO SLOT UI] Visual updated - Selected: {isSelected}, HasRadio: {HasRadio}</color>");
        }
    }
}
