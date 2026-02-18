using UnityEngine;
using TheHunt.Radio;
using TheHunt.Radio.Events;

namespace TheHunt.Inventory
{
    public class RadioEquipmentManager : MonoBehaviour
    {
        [Header("Data Reference")]
        [SerializeField] private RadioEquipmentData radioEquipmentData;

        [Header("Events")]
        [SerializeField] private RadioEquipEvent onRadioEquipped;
        [SerializeField] private RadioEquipEvent onRadioUnequipped;

        [Header("References")]
        [SerializeField] private InventorySystem inventorySystem;

        private void Awake()
        {
            if (inventorySystem == null)
            {
                inventorySystem = GetComponent<InventorySystem>();
            }
        }

        public bool TryEquipRadio(RadioItemData radio)
        {
            if (radioEquipmentData == null)
            {
                Debug.LogError("[RADIO] RadioEquipmentData reference is missing!");
                return false;
            }

            if (radio == null)
            {
                Debug.LogWarning("[RADIO] Cannot equip null radio");
                return false;
            }

            if (radioEquipmentData.HasRadioEquipped)
            {
                Debug.LogWarning($"[RADIO] Already have {radioEquipmentData.EquippedRadio.ItemName} equipped. Unequip first.");
                return false;
            }

            radioEquipmentData.EquipRadio(radio);
            onRadioEquipped?.Raise(radio);
            
            Debug.Log($"<color=green>[RADIO] Equipped {radio.ItemName}</color>");
            return true;
        }

        public bool UnequipRadio()
        {
            if (radioEquipmentData == null)
            {
                Debug.LogError("[RADIO] RadioEquipmentData reference is missing!");
                return false;
            }

            if (!radioEquipmentData.HasRadioEquipped)
            {
                Debug.LogWarning("[RADIO] No radio equipped to unequip");
                return false;
            }

            RadioItemData unequippedRadio = radioEquipmentData.EquippedRadio;
            radioEquipmentData.UnequipRadio();
            onRadioUnequipped?.Raise(unequippedRadio);

            Debug.Log($"<color=yellow>[RADIO] Unequipped {unequippedRadio.ItemName}</color>");
            return true;
        }

        public bool EquipRadioFromInventory(RadioItemData radioData)
        {
            if (inventorySystem == null)
            {
                Debug.LogError("[RADIO] InventorySystem reference is missing!");
                return false;
            }

            if (!inventorySystem.HasItem(radioData))
            {
                Debug.LogWarning($"[RADIO] {radioData.ItemName} not found in inventory");
                return false;
            }

            return TryEquipRadio(radioData);
        }
    }
}
