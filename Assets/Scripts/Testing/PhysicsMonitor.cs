using UnityEngine;

public class PhysicsMonitor : MonoBehaviour
{
    [SerializeField] private Rigidbody2D rb;
    [SerializeField] private bool logEveryFrame = false;
    [SerializeField] private float logInterval = 0.5f;
    
    private float lastLogTime;
    private float previousGravityScale;
    private float previousDrag;
    
    void Start()
    {
        if (rb == null)
        {
            rb = GetComponent<Rigidbody2D>();
        }
        
        if (rb != null)
        {
            previousGravityScale = rb.gravityScale;
            previousDrag = rb.linearDamping;
            LogPhysicsState("INITIAL STATE");
        }
    }
    
    void Update()
    {
        if (rb == null) return;
        
        bool gravityChanged = Mathf.Abs(rb.gravityScale - previousGravityScale) > 0.01f;
        bool dragChanged = Mathf.Abs(rb.linearDamping - previousDrag) > 0.01f;
        
        if (gravityChanged || dragChanged)
        {
            LogPhysicsState("PHYSICS CHANGED");
            previousGravityScale = rb.gravityScale;
            previousDrag = rb.linearDamping;
        }
        
        if (logEveryFrame || Time.time - lastLogTime >= logInterval)
        {
            LogPhysicsState("PERIODIC UPDATE");
            lastLogTime = Time.time;
        }
    }
    
    void LogPhysicsState(string context)
    {
        Debug.Log($"<color=orange>[PHYSICS MONITOR] {context}</color>\n" +
                  $"GravityScale: {rb.gravityScale:F2}\n" +
                  $"Drag: {rb.linearDamping:F2}\n" +
                  $"Velocity: {rb.linearVelocity}\n" +
                  $"Mass: {rb.mass:F2}");
    }
}
