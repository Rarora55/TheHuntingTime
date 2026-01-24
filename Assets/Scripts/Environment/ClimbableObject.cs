using UnityEngine;
using TheHunt.Inventory;

namespace TheHunt.Environment
{
    [RequireComponent(typeof(Collider2D))]
    public class ClimbableWithTeleport : MonoBehaviour
    {
        [Header("Climb Settings")]
        [SerializeField] private Transform exitPoint;
        [SerializeField] private float fadeDuration = 0.5f;
        [SerializeField] private bool autoClimb = true;
        [SerializeField] private KeyCode climbKey = KeyCode.W;

        [Header("Tipo de Objeto")]
        [SerializeField] private ClimbableType climbType = ClimbableType.Ladder;

        private Collider2D triggerCollider;
        private global::Player playerInRange;
        private bool hasUsed = false;

        private void Awake()
        {
            triggerCollider = GetComponent<Collider2D>();
            triggerCollider.isTrigger = true;

            if (exitPoint == null)
            {
                GameObject exitGO = new GameObject("ExitPoint");
                exitGO.transform.SetParent(transform);
                exitPoint = exitGO.transform;
                
                BoxCollider2D boxCol = GetComponent<BoxCollider2D>();
                if (boxCol != null)
                {
                    Vector2 topPosition = (Vector2)transform.position + boxCol.offset + Vector2.up * (boxCol.size.y / 2f + 1f);
                    exitPoint.position = topPosition;
                }
                else
                {
                    exitPoint.localPosition = Vector3.up * 3f;
                }
            }
        }

        private void Update()
        {
            if (playerInRange != null && !hasUsed && !autoClimb)
            {
                if (UnityEngine.Input.GetKeyDown(climbKey))
                {
                    TriggerClimb();
                }
            }
        }

        private void OnTriggerEnter2D(Collider2D other)
        {
            if (other.CompareTag("Player"))
            {
                global::Player player = other.GetComponent<global::Player>();
                if (player != null)
                {
                    playerInRange = player;

                    if (autoClimb)
                    {
                        TriggerClimb();
                    }
                }
            }
        }

        private void OnTriggerExit2D(Collider2D other)
        {
            if (other.CompareTag("Player"))
            {
                playerInRange = null;
            }
        }

        private void TriggerClimb()
        {
            if (hasUsed || playerInRange == null || exitPoint == null)
                return;

            hasUsed = true;

            playerInRange.SetVelocityZero();
            playerInRange.RB.gravityScale = 0f;

            ScreenFadeManager.Instance.FadeToBlackAndTeleport(
                exitPoint.position,
                playerInRange.gameObject,
                fadeDuration
            );
        }

        public void Reset()
        {
            hasUsed = false;
        }

        private void OnDrawGizmosSelected()
        {
            if (exitPoint != null)
            {
                Gizmos.color = Color.green;
                Gizmos.DrawWireSphere(exitPoint.position, 0.3f);
                Gizmos.DrawLine(transform.position, exitPoint.position);

                Gizmos.color = Color.cyan;
                Vector3 labelPos = exitPoint.position + Vector3.up * 0.5f;
                
#if UNITY_EDITOR
                UnityEditor.Handles.Label(labelPos, "EXIT POINT");
#endif
            }

            Collider2D col = GetComponent<Collider2D>();
            if (col != null && col is BoxCollider2D boxCol)
            {
                Gizmos.color = new Color(1, 0.5f, 0, 0.3f);
                Gizmos.matrix = transform.localToWorldMatrix;
                Gizmos.DrawCube(boxCol.offset, boxCol.size);
            }
        }
    }

    public enum ClimbableType
    {
        Ladder,
        Rope,
        Vine
    }
}
