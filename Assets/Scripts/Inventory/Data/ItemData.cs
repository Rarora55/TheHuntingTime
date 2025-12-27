using UnityEngine;
using System.Collections.Generic;

namespace TheHunt.Inventory
{
    public abstract class ItemData : ScriptableObject, ICombinable
    {
        [Header("Basic Info")]
        [SerializeField] private string itemName;
        [SerializeField] private string itemID;
        [SerializeField] [TextArea(3, 6)] private string description;
        [SerializeField] private Sprite itemIcon;
        [SerializeField] private Sprite itemDetailImage;
        [SerializeField] private GameObject pickupPrefab;

        [Header("Item Type")]
        [SerializeField] private ItemType itemType;
        [SerializeField] private bool stackable;
        [SerializeField] private int maxStackSize = 6;

        [Header("Examination")]
        [SerializeField] private bool canBeExamined = true;
        [SerializeField] [TextArea(3, 6)] private string examinationText;
        [SerializeField] private Sprite examinationImage;

        [Header("Combination")]
        [SerializeField] private bool canBeCombined = false;
        [SerializeField] [TextArea(2, 3)] private string combinationHint;

        public string ItemName => itemName;
        public string ItemID => itemID;
        public string Description => description;
        public Sprite ItemIcon => itemIcon;
        public Sprite ItemDetailImage => itemDetailImage;
        public GameObject PickupPrefab => pickupPrefab;
        public ItemType ItemType => itemType;
        public bool IsStackable => stackable;
        public int MaxStackSize => stackable ? Mathf.Max(1, maxStackSize) : 1;
        public bool CanBeExamined => canBeExamined;
        public string ExaminationText => examinationText;
        public Sprite ExaminationImage => examinationImage;
        public bool CanBeCombined => canBeCombined;

        public abstract void Use(GameObject user);

        public virtual bool CanCombineWith(ItemData otherItem)
        {
            return canBeCombined && otherItem != null && otherItem.CanBeCombined && this != otherItem;
        }

        public virtual string GetCombinationHint(ItemData otherItem)
        {
            if (!string.IsNullOrEmpty(combinationHint))
                return combinationHint;

            return canBeCombined ? "Can be combined with other items." : string.Empty;
        }

        private void OnValidate()
        {
            if (string.IsNullOrEmpty(itemID))
            {
                itemID = System.Guid.NewGuid().ToString();
            }
        }
    }
}
