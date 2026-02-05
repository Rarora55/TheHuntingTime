using UnityEngine;

namespace TheHunt.Inventory
{
    [CreateAssetMenu(fileName = "New Weapon", menuName = "Inventory/Weapon Item")]
    public class WeaponItemData : ItemData, IEquippable
    {
        [Header("Weapon Stats")]
        [SerializeField] private float damage = 10f;
        [SerializeField] private float attackRange = 1f;
        [SerializeField] private float attackSpeed = 1f;
        [SerializeField] private WeaponType weaponType;

        [Header("Tool Configuration")]
        [SerializeField] private ToolType toolType = ToolType.None;

        [Header("Ammo Configuration")]
        [SerializeField] private AmmoType requiredAmmo = AmmoType.None;
        [SerializeField] private int magazineSize = 12;
        [SerializeField] private int ammoPerShot = 1;
        
        [Header("Bullet Configuration")]
        [SerializeField] private BulletData bulletData;

        [Header("Equipment")]
        [SerializeField] private GameObject equipPrefab;
        [SerializeField] private Sprite equipIcon;
        [SerializeField] private AnimationClip equipAnimation;

        public float Damage => damage;
        public float AttackRange => attackRange;
        public float AttackSpeed => attackSpeed;
        public WeaponType WeaponType => weaponType;
        public ToolType ToolType => toolType;
        public AmmoType RequiredAmmo => requiredAmmo;
        public int MagazineSize => magazineSize;
        public int AmmoPerShot => ammoPerShot;
        public BulletData BulletData => bulletData;
        public GameObject EquipPrefab => equipPrefab;
        public Sprite EquipIcon => equipIcon;
        public AnimationClip EquipAnimation => equipAnimation;

        public void Equip(GameObject user)
        {
            Debug.Log($"<color=cyan>[WEAPON] Equipped {ItemName}</color>");
        }

        public void Unequip(GameObject user)
        {
            Debug.Log($"<color=cyan>[WEAPON] Unequipped {ItemName}</color>");
        }

        public override void Use(GameObject user)
        {
            Debug.Log($"<color=yellow>[WEAPON] Weapons cannot be used directly. Equip them instead.</color>");
        }
    }
}
