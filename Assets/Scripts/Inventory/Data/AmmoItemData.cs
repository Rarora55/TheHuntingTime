using UnityEngine;

namespace TheHunt.Inventory
{
    [CreateAssetMenu(fileName = "New Ammo", menuName = "Inventory/Ammo Item")]
    public class AmmoItemData : ItemData
    {
        [Header("Ammo Configuration")]
        [SerializeField] private AmmoType ammoType;
        [SerializeField] private int ammoAmount = 12;

        public AmmoType AmmoType => ammoType;
        public int AmmoAmount => ammoAmount;

        public override void Use(GameObject user)
        {
            Debug.Log($"<color=yellow>[AMMO] Ammo is added automatically when picked up</color>");
        }
    }
}
