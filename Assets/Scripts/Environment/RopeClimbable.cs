using UnityEngine;

[RequireComponent(typeof(Collider2D))]
public class RopeClimbable : MonoBehaviour
{
    [Header("Rope Configuration")]
    [SerializeField] private float ropeLength = 5f;
    [SerializeField] private bool requireInteractionInput = false;
    
    [Header("Visual Settings")]
    [SerializeField] private LineRenderer lineRenderer;
    [SerializeField] private int ropeSegments = 10;
    [SerializeField] private Color ropeColor = new Color(0.6f, 0.4f, 0.2f);
    [SerializeField] private float ropeWidth = 0.1f;
    
    private object anchorReference;
    private Collider2D ropeCollider;
    private global::Player playerInRange;
    private BoxCollider2D boxCollider;
    
    void Awake()
    {
        ropeCollider = GetComponent<Collider2D>();
        ropeCollider.isTrigger = true;
        
        boxCollider = GetComponent<BoxCollider2D>();
        if (boxCollider == null)
        {
            boxCollider = gameObject.AddComponent<BoxCollider2D>();
            boxCollider.isTrigger = true;
        }
        
        if (!CompareTag("FrontLadder"))
        {
            gameObject.tag = "FrontLadder";
        }
    }
    
    void Start()
    {
        if (anchorReference == null)
        {
            SetupCollider();
            SetupVisual();
        }
    }
    
    public void Initialize(object anchor, float length)
    {
        anchorReference = anchor;
        ropeLength = length;
        
        Debug.Log($"<color=cyan>[ROPE CLIMBABLE] Initializing rope with length {length}</color>");
        
        SpriteRenderer spriteRenderer = GetComponent<SpriteRenderer>();
        if (spriteRenderer != null)
        {
            spriteRenderer.enabled = false;
            Debug.Log($"<color=cyan>[ROPE CLIMBABLE] Disabled SpriteRenderer</color>");
        }
        
        SetupCollider();
        SetupVisual();
        
        Debug.Log($"<color=green>[ROPE CLIMBABLE] Rope initialized successfully - Collider offset: {boxCollider.offset}, size: {boxCollider.size}</color>");
    }
    
    void SetupCollider()
    {
        if (boxCollider != null)
        {
            boxCollider.offset = new Vector2(0, -ropeLength / 2f);
            boxCollider.size = new Vector2(0.5f, ropeLength);
            Debug.Log($"<color=cyan>[ROPE CLIMBABLE] Collider configured - offset: {boxCollider.offset}, size: {boxCollider.size}</color>");
        }
    }
    
    void SetupVisual()
    {
        if (lineRenderer == null)
        {
            lineRenderer = gameObject.AddComponent<LineRenderer>();
        }
        
        lineRenderer.positionCount = ropeSegments;
        lineRenderer.startWidth = ropeWidth;
        lineRenderer.endWidth = ropeWidth;
        lineRenderer.material = new Material(Shader.Find("Sprites/Default"));
        lineRenderer.startColor = ropeColor;
        lineRenderer.endColor = ropeColor;
        lineRenderer.sortingOrder = -1;
        lineRenderer.useWorldSpace = false;
        
        for (int i = 0; i < ropeSegments; i++)
        {
            float t = i / (float)(ropeSegments - 1);
            Vector3 pos = Vector3.down * (ropeLength * t);
            
            float sway = Mathf.Sin(t * Mathf.PI) * 0.1f;
            pos.x += sway;
            
            lineRenderer.SetPosition(i, pos);
        }
        
        Debug.Log($"<color=cyan>[ROPE CLIMBABLE] LineRenderer configured with {ropeSegments} segments, length: {ropeLength}</color>");
    }
    
    void OnTriggerEnter2D(Collider2D other)
    {
        if (other.CompareTag("Player"))
        {
            global::Player player = other.GetComponent<global::Player>();
            if (player != null)
            {
                playerInRange = player;
                player.SetCurrentLadder(ropeCollider);
                
                if (!requireInteractionInput)
                {
                    TryStartClimbing(player);
                }
            }
        }
    }
    
    void OnTriggerStay2D(Collider2D other)
    {
        if (other.CompareTag("Player") && requireInteractionInput)
        {
            global::Player player = other.GetComponent<global::Player>();
            if (player != null && player.InputHandler.GrabInput)
            {
                TryStartClimbing(player);
            }
        }
    }
    
    void OnTriggerExit2D(Collider2D other)
    {
        if (other.CompareTag("Player"))
        {
            global::Player player = other.GetComponent<global::Player>();
            if (player != null)
            {
                player.ClearCurrentLadder(ropeCollider);
                playerInRange = null;
            }
        }
    }
    
    void TryStartClimbing(global::Player player)
    {
        if (player.InputHandler.NormInputY != 0)
        {
            player.StateMachine.ChangeState(player.LadderClimbState);
        }
    }
    
    void OnDrawGizmos()
    {
        Gizmos.color = Color.cyan;
        Gizmos.DrawLine(transform.position, transform.position + Vector3.down * ropeLength);
        
        Gizmos.color = new Color(0, 1, 1, 0.3f);
        Gizmos.DrawCube(transform.position + Vector3.down * (ropeLength / 2f), new Vector3(0.5f, ropeLength, 0.1f));
    }
}
