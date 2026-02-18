using UnityEngine;
using TheHunt.Inventory;

namespace TheHunt.Radio
{
    [CreateAssetMenu(fileName = "RadioEquipmentData", menuName = "TheHunt/Data/Radio Equipment Data")]
    public class RadioEquipmentData : ScriptableObject
    {
        [Header("Runtime State")]
        [SerializeField] private RadioItemData equippedRadio;

        public RadioItemData EquippedRadio => equippedRadio;
        public bool HasRadioEquipped => equippedRadio != null;

        public void EquipRadio(RadioItemData radio)
        {
            equippedRadio = radio;
        }

        public void UnequipRadio()
        {
            equippedRadio = null;
        }

        private void OnEnable()
        {
            equippedRadio = null;
        }
    }
}
