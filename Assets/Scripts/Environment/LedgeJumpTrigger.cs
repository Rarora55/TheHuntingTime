using UnityEngine;

namespace TheHunt.Environment
{
    [RequireComponent(typeof(Collider2D))]
    public class LedgeJumpTrigger : MonoBehaviour
    {
        [Header("Ledge Position")]
        [SerializeField] private Transform ledgeGrabPoint;
        [SerializeField] private Vector2 ledgeGrabOffset = new Vector2(0f, 0.5f);
        
        [Header("Jump Settings")]
        [SerializeField] private float jumpForceX = 4f;
        [SerializeField] private float jumpForceY = 10f;
        [SerializeField] private bool requiresJumpInput = true;
        [SerializeField] private float jumpCooldown = 0.5f;
        
        [Header("Detection")]
        [SerializeField] private bool showGizmos = true;
        
        private bool playerInZone = false;
        private global::Player playerReference = null;
        private float lastJumpTime = -999f;
        
        private void Awake()
        {
            Collider2D col = GetComponent<Collider2D>();
            if (!col.isTrigger)
            {
                Debug.LogWarning($"[LEDGE JUMP TRIGGER] {gameObject.name} - Collider must be a trigger! Setting isTrigger = true.");
                col.isTrigger = true;
            }
        }
        
        private void Update()
        {
            if (!playerInZone || playerReference == null) return;
            
            if (requiresJumpInput)
            {
                if (playerReference.InputHandler.JumpInput)
                {
                    TriggerLedgeJump();
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
                    playerInZone = true;
                    playerReference = player;
                    
                    Debug.Log($"<color=cyan>[LEDGE JUMP TRIGGER] Player entered zone: {gameObject.name}</color>");
                    
                    if (!requiresJumpInput)
                    {
                        TriggerLedgeJump();
                    }
                }
            }
        }
        
        private void OnTriggerExit2D(Collider2D other)
        {
            if (other.CompareTag("Player"))
            {
                playerInZone = false;
                playerReference = null;
                Debug.Log($"<color=yellow>[LEDGE JUMP TRIGGER] Player exited zone: {gameObject.name}</color>");
            }
        }
        
        private void TriggerLedgeJump()
        {
            if (playerReference == null) return;
            
            if (Time.time < lastJumpTime + jumpCooldown)
            {
                return;
            }
            
            Vector2 targetPosition = GetLedgeGrabPosition();
            Vector2 playerPosition = playerReference.transform.position;
            
            Vector2 direction = (targetPosition - playerPosition).normalized;
            
            playerReference.SetVelocityX(direction.x * jumpForceX);
            playerReference.SetVelocityY(jumpForceY);
            
            int requiredFacingDirection = direction.x > 0 ? 1 : -1;
            if (playerReference.FacingRight != requiredFacingDirection)
            {
                playerReference.Flip();
            }
            
            lastJumpTime = Time.time;
            
            Debug.Log($"<color=green>[LEDGE JUMP TRIGGER] ✓ Jumping to ledge! Force: ({direction.x * jumpForceX:F2}, {jumpForceY:F2})</color>");
        }
        
        private Vector2 GetLedgeGrabPosition()
        {
            if (ledgeGrabPoint != null)
            {
                return ledgeGrabPoint.position;
            }
            
            return (Vector2)transform.position + ledgeGrabOffset;
        }
        
        private void OnDrawGizmos()
        {
            if (!showGizmos) return;
            
            Gizmos.color = Color.cyan;
            Vector2 ledgePos = GetLedgeGrabPosition();
            Gizmos.DrawWireSphere(ledgePos, 0.2f);
            Gizmos.DrawLine(transform.position, ledgePos);
            
            Gizmos.color = new Color(0, 1, 1, 0.3f);
            Gizmos.DrawCube(transform.position, GetComponent<Collider2D>().bounds.size);
        }
        
        private void OnDrawGizmosSelected()
        {
            if (!showGizmos) return;
            
            Gizmos.color = Color.green;
            Vector2 ledgePos = GetLedgeGrabPosition();
            Gizmos.DrawSphere(ledgePos, 0.25f);
        }
    }
}
