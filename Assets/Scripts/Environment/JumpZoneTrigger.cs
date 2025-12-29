using UnityEngine;
using System.Collections.Generic;

namespace TheHunt.Environment
{
    [RequireComponent(typeof(Collider2D))]
    public class JumpZoneTrigger : MonoBehaviour, IJumpZone
    {
        [Header("Jump Settings")]
        [SerializeField] private JumpType jumpType = JumpType.Vertical;
        [SerializeField] private List<JumpDirection> allowedDirections = new List<JumpDirection> { JumpDirection.Up };

        [Header("Jump Forces")]
        [SerializeField] private float upForce = 10f;
        [SerializeField] private float downForce = 5f;
        [SerializeField] private float horizontalForce = 8f;
        [SerializeField] private float jumpForceMultiplier = 1f;

        [Header("Target (Optional)")]
        [SerializeField] private Transform targetPosition;

        [Header("Visual Feedback")]
        [SerializeField] private Color gizmoColor = Color.yellow;
        [SerializeField] private bool showGizmos = true;
        [SerializeField] private bool showDirectionArrows = true;

        private Collider2D jumpCollider;
        private global::Player currentPlayer;

        private void Awake()
        {
            jumpCollider = GetComponent<Collider2D>();
            
            if (jumpCollider != null && !jumpCollider.isTrigger)
            {
                Debug.LogWarning($"[JUMPZONE] {name}: Collider should be a trigger. Setting isTrigger = true");
                jumpCollider.isTrigger = true;
            }

            if (allowedDirections.Count == 0)
            {
                Debug.LogWarning($"[JUMPZONE] {name}: No allowed directions set. Adding default Up direction");
                allowedDirections.Add(JumpDirection.Up);
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
                    player.SetCurrentJumpZone(this);
                    Debug.Log($"<color=magenta>[JUMPZONE] Player entró en zona de salto {jumpType}</color>");
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
                    player.ClearCurrentJumpZone(this);
                    currentPlayer = null;
                    Debug.Log($"<color=yellow>[JUMPZONE] Player salió de zona de salto</color>");
                }
            }
        }

        public bool CanJumpInDirection(JumpDirection direction)
        {
            return allowedDirections.Contains(direction);
        }

        public Vector2 GetJumpForce(JumpDirection direction)
        {
            Vector2 force = Vector2.zero;

            switch (direction)
            {
                case JumpDirection.Up:
                    force = new Vector2(0, upForce);
                    break;
                case JumpDirection.Down:
                    force = new Vector2(0, -downForce);
                    break;
                case JumpDirection.Left:
                    force = new Vector2(-horizontalForce, upForce * 0.5f);
                    break;
                case JumpDirection.Right:
                    force = new Vector2(horizontalForce, upForce * 0.5f);
                    break;
            }

            return force * jumpForceMultiplier;
        }

        public JumpType GetJumpType() => jumpType;
        public Transform GetTargetPosition() => targetPosition;
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

            if (showDirectionArrows)
            {
                DrawDirectionArrows();
            }

            if (targetPosition != null)
            {
                Gizmos.color = Color.red;
                Gizmos.DrawLine(transform.position, targetPosition.position);
                Gizmos.DrawWireSphere(targetPosition.position, 0.3f);
            }
        }

        private void DrawDirectionArrows()
        {
            Vector3 center = transform.position;
            float arrowLength = 1f;

            Gizmos.color = Color.green;

            foreach (JumpDirection dir in allowedDirections)
            {
                Vector3 direction = Vector3.zero;
                
                switch (dir)
                {
                    case JumpDirection.Up:
                        direction = Vector3.up;
                        break;
                    case JumpDirection.Down:
                        direction = Vector3.down;
                        break;
                    case JumpDirection.Left:
                        direction = Vector3.left;
                        break;
                    case JumpDirection.Right:
                        direction = Vector3.right;
                        break;
                }

                if (direction != Vector3.zero)
                {
                    Vector3 end = center + direction * arrowLength;
                    Gizmos.DrawLine(center, end);
                    
                    Vector3 arrowHead1 = end - (direction + Vector3.right * 0.3f).normalized * 0.3f;
                    Vector3 arrowHead2 = end - (direction + Vector3.left * 0.3f).normalized * 0.3f;
                    Gizmos.DrawLine(end, arrowHead1);
                    Gizmos.DrawLine(end, arrowHead2);
                }
            }
        }

        private void OnDrawGizmosSelected()
        {
            if (!showGizmos) return;

            Gizmos.color = Color.white;
            Vector3 labelPos = transform.position + Vector3.up * 1.5f;
            
#if UNITY_EDITOR
            string directionsStr = string.Join(", ", allowedDirections);
            UnityEditor.Handles.Label(labelPos, $"{jumpType}\nDirections: {directionsStr}\nMultiplier: {jumpForceMultiplier}");
#endif
        }
    }
}
