using UnityEngine;
using TheHunt.Events;

namespace TheHunt.Environment
{
    [RequireComponent(typeof(Collider2D))]
    public class Ladder : MonoBehaviour
    {
        [Header("Events")]
        [SerializeField] private ScreenFadeEvent screenFadeEvent;

        [Header("Settings")]
        [SerializeField] private bool requireInteractionInput = false;
        [SerializeField] private KeyCode interactionKey = KeyCode.W;
        
        [Header("Teleport Settings")]
        [SerializeField] private Transform topExitPoint;
        [SerializeField] private float fadeDuration = 0.5f;

        private Collider2D ladderCollider;
        private global::Player playerInRange;
        private bool hasUsed = false;
        private BoxCollider2D boxCollider;

        private void Awake()
        {
            ladderCollider = GetComponent<Collider2D>();
            boxCollider = GetComponent<BoxCollider2D>();
            
            if (!ladderCollider.isTrigger)
            {
                ladderCollider.isTrigger = true;
            }

            if (!gameObject.CompareTag("FrontLadder"))
            {
                Debug.LogWarning($"[LADDER] {gameObject.name} should have tag 'FrontLadder'");
            }
            
            if (topExitPoint == null && boxCollider != null)
            {
                GameObject topObj = new GameObject("LadderTopExit");
                topObj.transform.SetParent(transform);
                topExitPoint = topObj.transform;
                
                Vector2 topPosition = (Vector2)transform.position + boxCollider.offset + Vector2.up * (boxCollider.size.y / 2f + 1f);
                topExitPoint.position = topPosition;
            }
        }

        private void Update()
        {
            if (playerInRange != null && !hasUsed && requireInteractionInput)
            {
                if (UnityEngine.Input.GetKeyDown(interactionKey))
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
                    player.SetCurrentLadder(ladderCollider);

                    if (!requireInteractionInput)
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
                global::Player player = other.GetComponent<global::Player>();
                
                if (player != null)
                {
                    player.ClearCurrentLadder(ladderCollider);
                    playerInRange = null;
                }
            }
        }

        private void TriggerClimb()
        {
            if (hasUsed || playerInRange == null || topExitPoint == null)
                return;

            hasUsed = true;

            playerInRange.SetVelocityZero();
            playerInRange.RB.gravityScale = 0f;

            if (screenFadeEvent != null)
            {
                screenFadeEvent.RaiseFadeToBlackAndTeleport(
                    fadeDuration,
                    topExitPoint.position,
                    playerInRange.transform,
                    null
                );
            }
            else
            {
                Debug.LogWarning("[LADDER] ScreenFadeEvent not assigned!");
                playerInRange.transform.position = topExitPoint.position;
            }

            Debug.Log("<color=green>[LADDER] Teleporting player to top exit point</color>");
        }

        public void Reset()
        {
            hasUsed = false;
        }
        
        public bool IsPlayerAtTop(Vector3 playerPosition)
        {
            if (topExitPoint == null || boxCollider == null)
                return false;
            
            float distanceToTop = Mathf.Abs(playerPosition.y - topExitPoint.position.y);
            return distanceToTop <= 1f;
        }
        
        public Vector3 GetTopPosition()
        {
            if (topExitPoint != null)
                return topExitPoint.position;
            
            if (boxCollider != null)
            {
                return (Vector2)transform.position + boxCollider.offset + Vector2.up * (boxCollider.size.y / 2f);
            }
            
            return transform.position;
        }
        
        public Vector3 GetBottomPosition()
        {
            if (boxCollider != null)
            {
                return (Vector2)transform.position + boxCollider.offset - Vector2.up * (boxCollider.size.y / 2f);
            }
            
            return transform.position;
        }

        private void OnDrawGizmosSelected()
        {
            Collider2D col = GetComponent<Collider2D>();
            if (col == null) return;

            Gizmos.color = new Color(0, 1, 1, 0.3f);
            
            if (col is BoxCollider2D boxCol)
            {
                Gizmos.matrix = transform.localToWorldMatrix;
                Gizmos.DrawCube(boxCol.offset, boxCol.size);
                Gizmos.DrawWireCube(boxCol.offset, boxCol.size);
            }
            
            if (topExitPoint != null)
            {
                Gizmos.color = Color.green;
                Gizmos.DrawWireSphere(topExitPoint.position, 0.4f);
                Gizmos.DrawLine(transform.position, topExitPoint.position);
            }

#if UNITY_EDITOR
            Vector3 labelPos = transform.position + Vector3.up * 0.5f;
            UnityEditor.Handles.Label(labelPos, $"LADDER\n{(requireInteractionInput ? $"Press [{interactionKey}]" : "Auto")}\nFade Duration: {fadeDuration}s");
            
            if (topExitPoint != null)
            {
                UnityEditor.Handles.Label(topExitPoint.position + Vector3.up * 0.5f, "EXIT POINT");
            }
#endif
        }
    }
}
