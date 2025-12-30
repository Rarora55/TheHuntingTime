using UnityEngine;
using TheHunt.Inventory;

namespace TheHunt.Environment
{
    [RequireComponent(typeof(Collider2D))]
    public class ClimbableObject : MonoBehaviour, IClimbable
    {
        [Header("Climb Settings")]
        [SerializeField] private ClimbType climbType = ClimbType.Rope;
        [SerializeField] private float climbSpeed = 3f;
        [SerializeField] private bool allowWallSlide = false;

        [Header("Pickup Settings")]
        [SerializeField] private bool canBePickedUp = false;
        [SerializeField] private WeaponItemData itemData;

        [Header("Visual Feedback")]
        [SerializeField] private Color gizmoColor = Color.green;
        [SerializeField] private bool showGizmos = true;

        private Collider2D climbCollider;
        private global::Player currentPlayer;

        private void Awake()
        {
            climbCollider = GetComponent<Collider2D>();
            
            if (climbCollider != null && !climbCollider.isTrigger)
            {
                climbCollider.isTrigger = true;
            }
        }

        private void OnTriggerEnter2D(Collider2D collision)
        {
            if (collision.CompareTag("Player"))
            {
                global::Player player = collision.GetComponent<global::Player>();
                if (player != null)
                {
                    currentPlayer = player;
                    player.SetCurrentClimbable(this);
                }
            }
        }

        private void OnTriggerExit2D(Collider2D collision)
        {
            if (collision.CompareTag("Player"))
            {
                global::Player player = collision.GetComponent<global::Player>();
                if (player != null && player == currentPlayer)
                {
                    player.ClearCurrentClimbable(this);
                    currentPlayer = null;
                }
            }
        }

        public ClimbType GetClimbType() => climbType;
        public float GetClimbSpeed() => climbSpeed;
        public bool AllowsWallSlide() => allowWallSlide;
        public bool CanBePickedUp() => canBePickedUp;
        public Transform GetTransform() => transform;

        public WeaponItemData GetItemData() => itemData;

        public void PickUp(global::Player player)
        {
            if (!canBePickedUp || itemData == null)
            {
                return;
            }
            
            if (currentPlayer == player)
            {
                player.ClearCurrentClimbable(this);
                currentPlayer = null;
            }

            gameObject.SetActive(false);
        }

        private void OnDrawGizmos()
        {
            if (!showGizmos) return;

            Collider2D col = GetComponent<Collider2D>();
            if (col == null) return;

            Gizmos.color = gizmoColor;
            
            if (col is BoxCollider2D boxCollider)
            {
                Vector3 center = transform.TransformPoint(boxCollider.offset);
                Vector3 size = new Vector3(
                    boxCollider.size.x * transform.lossyScale.x,
                    boxCollider.size.y * transform.lossyScale.y,
                    0.1f
                );
                Gizmos.DrawWireCube(center, size);
            }
            else if (col is CircleCollider2D circleCollider)
            {
                Vector3 center = transform.TransformPoint(circleCollider.offset);
                float radius = circleCollider.radius * Mathf.Max(transform.lossyScale.x, transform.lossyScale.y);
                Gizmos.DrawWireSphere(center, radius);
            }
        }

        private void OnDrawGizmosSelected()
        {
            if (!showGizmos) return;

            Gizmos.color = Color.white;
            Vector3 labelPos = transform.position + Vector3.up * 0.5f;
            
#if UNITY_EDITOR
            UnityEditor.Handles.Label(labelPos, $"{climbType}\nSpeed: {climbSpeed}\nPickup: {canBePickedUp}");
#endif
        }
    }
}
