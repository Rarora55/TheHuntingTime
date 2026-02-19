using UnityEngine;
using TheHunt.Inventory;
using TheHunt.Radio.Events;
using TheHunt.Utilities;

namespace TheHunt.Radio.UI
{
    public class RadioEquipmentPanel : MonoBehaviour
    {
        [Header("Data Reference")]
        [SerializeField] private RadioEquipmentData radioEquipmentData;

        [Header("Events")]
        [SerializeField] private RadioEquipEvent onRadioEquipped;
        [SerializeField] private RadioEquipEvent onRadioUnequipped;

        [Header("References")]
        [SerializeField] private RadioEquipmentManager radioManager;
        [SerializeField] private InventoryUIController uiController;

        [Header("Radio Slot")]
        [SerializeField] private RadioSlotUI radioSlot;

        [Header("Settings")]
        [SerializeField] private bool autoHideWhenEmpty = false;
        [SerializeField] private CanvasGroup canvasGroup;
        
        private ComponentAutoAssigner autoAssigner;
        private bool isRadioSlotSelectionMode = false;
        
        public bool IsInRadioSlotSelectionMode => isRadioSlotSelectionMode;

        private void Awake()
        {
            autoAssigner = new ComponentAutoAssigner();
            
            radioManager = autoAssigner.GetOrFindComponent(radioManager);
            uiController = autoAssigner.GetOrFindComponent(uiController);
            canvasGroup = canvasGroup != null ? canvasGroup : GetComponent<CanvasGroup>();

            ValidateSlot();
        }

        private void OnEnable()
        {
            Debug.Log("<color=cyan>[RADIO EQUIPMENT PANEL] OnEnable</color>");
            
            if (onRadioEquipped != null)
            {
                onRadioEquipped.AddListener(OnRadioEquipped);
                Debug.Log("<color=green>[RADIO EQUIPMENT PANEL] Subscribed to onRadioEquipped event</color>");
            }
            else
            {
                Debug.LogWarning("<color=yellow>[RADIO EQUIPMENT PANEL] onRadioEquipped event is NULL!</color>");
            }
            
            if (onRadioUnequipped != null)
            {
                onRadioUnequipped.AddListener(OnRadioUnequipped);
                Debug.Log("<color=green>[RADIO EQUIPMENT PANEL] Subscribed to onRadioUnequipped event</color>");
            }
            else
            {
                Debug.LogWarning("<color=yellow>[RADIO EQUIPMENT PANEL] onRadioUnequipped event is NULL!</color>");
            }

            RefreshSlot();
        }

        private void OnDisable()
        {
            if (onRadioEquipped != null)
            {
                onRadioEquipped.RemoveListener(OnRadioEquipped);
            }
            
            if (onRadioUnequipped != null)
            {
                onRadioUnequipped.RemoveListener(OnRadioUnequipped);
            }
        }

        private void ValidateSlot()
        {
            if (radioSlot == null)
            {
                Debug.LogError("<color=red>[RADIO EQUIPMENT PANEL] Radio slot not assigned!</color>");
            }
        }

        private void OnRadioEquipped(RadioItemData radio)
        {
            Debug.Log($"<color=cyan>[RADIO EQUIPMENT PANEL] OnRadioEquipped EVENT received! Radio: {(radio != null ? radio.ItemName : "NULL")}</color>");
            
            if (radioSlot != null)
            {
                Debug.Log("<color=green>[RADIO EQUIPMENT PANEL] Calling radioSlot.EquipRadio...</color>");
                radioSlot.EquipRadio(radio);
            }
            else
            {
                Debug.LogError("<color=red>[RADIO EQUIPMENT PANEL] radioSlot is NULL!</color>");
            }

            UpdateVisibility();
        }

        private void OnRadioUnequipped(RadioItemData radio)
        {
            if (radioSlot != null)
            {
                radioSlot.UnequipRadio();
            }

            UpdateVisibility();
        }

        public void RefreshSlot()
        {
            Debug.Log("<color=cyan>[RADIO EQUIPMENT PANEL] RefreshSlot called</color>");
            
            if (radioEquipmentData == null)
            {
                Debug.LogError("<color=red>[RADIO EQUIPMENT PANEL] radioEquipmentData is NULL!</color>");
                return;
            }
            
            if (radioSlot == null)
            {
                Debug.LogError("<color=red>[RADIO EQUIPMENT PANEL] radioSlot is NULL!</color>");
                return;
            }

            bool hasRadio = radioEquipmentData.HasRadioEquipped;
            Debug.Log($"<color=yellow>[RADIO EQUIPMENT PANEL] HasRadioEquipped: {hasRadio}</color>");
            
            if (hasRadio)
            {
                RadioItemData equippedRadio = radioEquipmentData.EquippedRadio;
                Debug.Log($"<color=green>[RADIO EQUIPMENT PANEL] Equipped radio: {(equippedRadio != null ? equippedRadio.ItemName : "NULL")}</color>");
                radioSlot.EquipRadio(equippedRadio);
            }
            else
            {
                Debug.Log("<color=yellow>[RADIO EQUIPMENT PANEL] No radio equipped, unequipping slot</color>");
                radioSlot.UnequipRadio();
            }

            UpdateVisibility();
        }

        public void SelectSlot(bool selected)
        {
            if (radioSlot != null)
            {
                radioSlot.SetSelected(selected);
            }
        }

        public void ClearSelection()
        {
            if (radioSlot != null)
            {
                radioSlot.SetSelected(false);
            }
        }

        private void UpdateVisibility()
        {
            if (!autoHideWhenEmpty || canvasGroup == null)
                return;

            bool hasRadio = radioSlot != null && radioSlot.HasRadio;
            canvasGroup.alpha = hasRadio ? 1f : 0.5f;
        }

        public bool HasRadio => radioSlot != null && radioSlot.HasRadio;
        
        public RadioItemData GetEquippedRadio()
        {
            return radioSlot?.EquippedRadio;
        }
        
        public void EnterRadioSlotSelectionMode()
        {
            isRadioSlotSelectionMode = true;
            SelectSlot(true);
            Debug.Log("<color=green>[RADIO EQUIPMENT PANEL] ✓ ENTERED radio slot selection mode</color>");
        }
        
        public void ExitRadioSlotSelectionMode()
        {
            isRadioSlotSelectionMode = false;
            ClearSelection();
            Debug.Log("<color=yellow>[RADIO EQUIPMENT PANEL] ✗ EXITED radio slot selection mode</color>");
        }
        
        public void UnequipCurrentRadio()
        {
            if (!isRadioSlotSelectionMode || radioManager == null)
                return;
            
            RadioItemData radioInSlot = GetEquippedRadio();
            
            if (radioInSlot == null)
            {
                Debug.Log($"<color=yellow>[RADIO EQUIPMENT PANEL] No radio in slot to unequip</color>");
                return;
            }
            
            Debug.Log($"<color=green>[RADIO EQUIPMENT PANEL] Unequipping {radioInSlot.ItemName} from radio slot</color>");
            radioManager.UnequipRadio();
            
            ExitRadioSlotSelectionMode();
        }
    }
}
