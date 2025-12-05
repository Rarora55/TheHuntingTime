using UnityEngine;

namespace TheHunt.Inventory
{
    [CreateAssetMenu(fileName = "New Key Item", menuName = "Inventory/Key Item")]
    public class KeyItemData : ItemData
    {
        [Header("Key Item Settings")]
        [SerializeField] private string[] unlocks;
        [SerializeField] private bool isQuestItem;
        [SerializeField] private bool canBeDiscarded = false;

        public string[] Unlocks => unlocks;
        public bool IsQuestItem => isQuestItem;
        public bool CanBeDiscarded => canBeDiscarded;

        public override void Use(GameObject user)
        {
            Debug.Log($"<color=yellow>[KEY ITEM] {ItemName} cannot be used directly</color>");
        }
    }
}
