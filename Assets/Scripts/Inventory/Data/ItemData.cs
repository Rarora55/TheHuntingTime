using UnityEngine;

namespace TheHunt.Inventory
{
    public abstract class ItemData : ScriptableObject
    {
        [Header("Basic Info")]
        [SerializeField] private string itemName;
        [SerializeField] private string itemID;
        [SerializeField] [TextArea(3, 6)] private string description;
        [SerializeField] private Sprite itemIcon;
        [SerializeField] private Sprite itemDetailImage;

        [Header("Item Type")]
        [SerializeField] private ItemType itemType;
        [SerializeField] private bool stackable;

        [Header("Examination")]
        [SerializeField] private bool canBeExamined = true;
        [SerializeField] [TextArea(3, 6)] private string examinationText;
        [SerializeField] private Sprite examinationImage;

        public string ItemName => itemName;
        public string ItemID => itemID;
        public string Description => description;
        public Sprite ItemIcon => itemIcon;
        public Sprite ItemDetailImage => itemDetailImage;
        public ItemType ItemType => itemType;
        public bool IsStackable => stackable;
        public bool CanBeExamined => canBeExamined;
        public string ExaminationText => examinationText;
        public Sprite ExaminationImage => examinationImage;

        public abstract void Use(GameObject user);

        private void OnValidate()
        {
            if (string.IsNullOrEmpty(itemID))
            {
                itemID = System.Guid.NewGuid().ToString();
            }
        }
    }
}
