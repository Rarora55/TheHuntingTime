using UnityEngine;
using TheHunt.Inventory;

namespace TheHunt.Items
{
    public class RadioPickup : MonoBehaviour
    {
        [Header("Radio Data")]
        [SerializeField] private RadioItemData radioData;

        [Header("Pickup Settings")]
        [SerializeField] private LayerMask playerLayer;

        private bool hasBeenPickedUp;

        private void OnTriggerEnter2D(Collider2D other)
        {
            if (hasBeenPickedUp)
                return;

            if (((1 << other.gameObject.layer) & playerLayer) == 0)
                return;

            InventorySystem inventory = other.GetComponent<InventorySystem>();
            if (inventory == null)
            {
                Debug.LogWarning("[RADIO PICKUP] Player does not have InventorySystem");
                return;
            }

            if (inventory.TryAddItem(radioData))
            {
                Debug.Log($"<color=green>[RADIO PICKUP] Added {radioData.ItemName} to inventory. Open inventory to equip it in the Radio slot!</color>");
                hasBeenPickedUp = true;
                gameObject.SetActive(false);
            }
        }

        private void OnDrawGizmosSelected()
        {
            BoxCollider2D col = GetComponent<BoxCollider2D>();
            if (col != null)
            {
                Gizmos.color = hasBeenPickedUp ? Color.gray : Color.yellow;
                Vector3 center = (Vector2)transform.position + col.offset;
                Gizmos.DrawWireCube(center, col.size);
            }
        }
    }
}
