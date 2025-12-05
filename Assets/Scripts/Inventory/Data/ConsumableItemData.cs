using UnityEngine;

namespace TheHunt.Inventory
{
    [CreateAssetMenu(fileName = "New Consumable", menuName = "Inventory/Consumable Item")]
    public class ConsumableItemData : ItemData, IUsable
    {
        [Header("Consumable Settings")]
        [SerializeField] private float healAmount = 50f;
        [SerializeField] private bool removeOnUse = true;
        [SerializeField] private AudioClip useSound;
        [SerializeField] private GameObject useEffect;

        public float HealAmount => healAmount;
        public bool RemoveOnUse => removeOnUse;
        public AudioClip UseSound => useSound;
        public GameObject UseEffect => useEffect;

        public bool CanUse(GameObject user)
        {
            HealthController health = user.GetComponent<HealthController>();
            if (health == null)
                return false;

            return health.CurrentHealth < health.MaxHealth;
        }

        public override void Use(GameObject user)
        {
            if (!CanUse(user))
            {
                Debug.Log($"<color=yellow>[INVENTORY] Cannot use {ItemName} - Health is full</color>");
                return;
            }

            HealthController health = user.GetComponent<HealthController>();
            if (health != null)
            {
                health.Heal(healAmount);
                Debug.Log($"<color=green>[INVENTORY] Used {ItemName} - Healed {healAmount} HP</color>");

                if (useSound != null)
                {
                    AudioSource.PlayClipAtPoint(useSound, user.transform.position);
                }

                if (useEffect != null)
                {
                    Instantiate(useEffect, user.transform.position, Quaternion.identity);
                }
            }
        }
    }
}
