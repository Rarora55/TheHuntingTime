using UnityEngine;

namespace TheHunt.Environment
{
    [RequireComponent(typeof(Collider2D))]
    public class Ladder : MonoBehaviour
    {
        [Header("Settings")]
        [SerializeField] private bool requireInteractionInput = false;
        [SerializeField] private KeyCode interactionKey = KeyCode.W;
        
        [Header("Top Entry Detection")]
        [SerializeField] private Transform topPoint;
        [SerializeField] private float topEntryDistance = 1f;

        private Collider2D ladderCollider;
        private global::Player playerInRange;
        private bool playerIsClimbing;
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
            
            if (topPoint == null && boxCollider != null)
            {
                GameObject topObj = new GameObject("LadderTop");
                topObj.transform.SetParent(transform);
                topPoint = topObj.transform;
                
                Vector2 topPosition = (Vector2)transform.position + boxCollider.offset + Vector2.up * (boxCollider.size.y / 2f);
                topPoint.position = topPosition;
            }
        }

        private void Update()
        {
            if (playerInRange != null && !playerIsClimbing && requireInteractionInput)
            {
                if (UnityEngine.Input.GetKeyDown(interactionKey))
                {
                    StartClimbing();
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
                        TryStartClimbing(player);
                    }
                }
            }
        }

        private void OnTriggerStay2D(Collider2D other)
        {
            if (other.CompareTag("Player") && !requireInteractionInput)
            {
                global::Player player = other.GetComponent<global::Player>();
                
                if (player != null && !playerIsClimbing)
                {
                    TryStartClimbing(player);
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
                    playerIsClimbing = false;
                }
            }
        }

        private void TryStartClimbing(global::Player player)
        {
            int yInput = player.InputHandler.NormInputY;
            bool grabInput = player.InputHandler.GrabInput;

            if (grabInput && (yInput == 1 || yInput == -1))
            {
                StartClimbing();
            }
        }

        private void StartClimbing()
        {
            if (playerInRange != null && playerInRange.LadderClimbState != null)
            {
                playerIsClimbing = true;
                playerInRange.StateMachine.ChangeState(playerInRange.LadderClimbState);
            }
        }
        
        public bool IsPlayerAtTop(Vector3 playerPosition)
        {
            if (topPoint == null || boxCollider == null)
                return false;
            
            float distanceToTop = Mathf.Abs(playerPosition.y - topPoint.position.y);
            return distanceToTop <= topEntryDistance;
        }
        
        public Vector3 GetTopPosition()
        {
            if (topPoint != null)
                return topPoint.position;
            
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
            
            if (topPoint != null)
            {
                Gizmos.color = Color.green;
                Gizmos.DrawWireSphere(topPoint.position, topEntryDistance);
            }

#if UNITY_EDITOR
            Vector3 labelPos = transform.position + Vector3.up * 0.5f;
            UnityEditor.Handles.Label(labelPos, $"LADDER\n{(requireInteractionInput ? $"Press [{interactionKey}]" : "Auto")}");
#endif
        }
    }
}
