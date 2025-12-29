using UnityEngine;

namespace TheHunt.Environment
{
    [RequireComponent(typeof(Collider2D))]
    public class SlideableWall : MonoBehaviour, ISlideable
    {
        [Header("Slide Settings")]
        [SerializeField] private float slideSpeed = 2f;
        [SerializeField] private float surfaceAngle = 90f;
        [SerializeField] private float maxSlideAngle = 90f;
        [SerializeField] private bool requiresSpecialGear = false;

        [Header("Visual Feedback")]
        [SerializeField] private Color gizmoColor = Color.blue;
        [SerializeField] private bool showGizmos = true;

        private Collider2D slideCollider;
        private global::Player currentPlayer;

        private void Awake()
        {
            slideCollider = GetComponent<Collider2D>();
            
            if (slideCollider != null && !slideCollider.isTrigger)
            {
                Debug.LogWarning($"[SLIDEABLE] {name}: Collider should be a trigger. Setting isTrigger = true");
                slideCollider.isTrigger = true;
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
                    player.SetCurrentSlideable(this);
                    Debug.Log($"<color=cyan>[SLIDEABLE] Player entró en superficie deslizable (ángulo: {surfaceAngle}°)</color>");
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
                    player.ClearCurrentSlideable(this);
                    currentPlayer = null;
                    Debug.Log($"<color=yellow>[SLIDEABLE] Player salió de superficie deslizable</color>");
                }
            }
        }

        public bool CanSlide(global::Player player)
        {
            if (requiresSpecialGear)
            {
                return false;
            }

            return surfaceAngle <= maxSlideAngle;
        }

        public float GetSlideSpeed() => slideSpeed;
        public float GetSurfaceAngle() => surfaceAngle;
        public Transform GetTransform() => transform;

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
        }

        private void OnDrawGizmosSelected()
        {
            if (!showGizmos) return;

            Gizmos.color = Color.white;
            Vector3 labelPos = transform.position + Vector3.up * 0.5f;
            
#if UNITY_EDITOR
            UnityEditor.Handles.Label(labelPos, $"Slideable Wall\nSpeed: {slideSpeed}\nAngle: {surfaceAngle}°");
#endif
        }
    }
}
