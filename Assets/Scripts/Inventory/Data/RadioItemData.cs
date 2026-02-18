using UnityEngine;

namespace TheHunt.Inventory
{
    [CreateAssetMenu(fileName = "New Radio", menuName = "Inventory/Radio Item")]
    public class RadioItemData : KeyItemData
    {
        [Header("Radio Settings")]
        [SerializeField] private bool canReceiveSignals = true;

        public bool CanReceiveSignals => canReceiveSignals;

        public override void Use(GameObject user)
        {
            Debug.Log($"<color=cyan>[RADIO] {ItemName} equipped and ready to receive signals</color>");
        }
    }
}
