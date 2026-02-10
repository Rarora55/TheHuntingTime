using UnityEngine;

public class LedgeMarker : MonoBehaviour
{
    [Header("Configuración del Ledge")]
    [Tooltip("Esquina izquierda del ledge (child GameObject)")]
    public Transform leftCorner;
    
    [Tooltip("Esquina derecha del ledge (child GameObject)")]
    public Transform rightCorner;
    
    [Tooltip("Layer del objeto (debe ser Ground para que se detecte)")]
    public LayerMask ledgeLayer;
    
    [Header("Debug")]
    [Tooltip("Mostrar gizmos de debug en el editor")]
    public bool showDebug = true;
    
    private void OnValidate()
    {
        if (leftCorner == null || rightCorner == null)
        {
            Debug.LogWarning($"[LedgeMarker] {gameObject.name} necesita asignar leftCorner y rightCorner.", this);
        }
    }
    
    public Vector2 GetCornerPosition(int facingDirection)
    {
        Transform selectedCorner = null;
        string cornerName = "";
        
        if (facingDirection > 0 && rightCorner != null)
        {
            selectedCorner = rightCorner;
            cornerName = "RIGHT";
        }
        else if (facingDirection < 0 && leftCorner != null)
        {
            selectedCorner = leftCorner;
            cornerName = "LEFT";
        }
        
        if (selectedCorner != null)
        {
            Vector2 worldPos = selectedCorner.position;
            Debug.Log($"<color=lime>[LedgeMarker.GetCornerPosition] {gameObject.name} | Corner: {cornerName} | WorldPos: {worldPos} | FacingDir: {facingDirection}</color>");
            return worldPos;
        }
        
        Debug.LogWarning($"[LedgeMarker] No se encontró corner para dirección {facingDirection} en {gameObject.name}", this);
        return transform.position;
    }
    
    public bool HasValidCorners()
    {
        return leftCorner != null && rightCorner != null;
    }
    
    private void OnDrawGizmos()
    {
        if (!showDebug) return;
        
        if (leftCorner != null)
        {
            Gizmos.color = Color.cyan;
            Gizmos.DrawWireSphere(leftCorner.position, 0.1f);
            Gizmos.DrawLine(transform.position, leftCorner.position);
        }
        
        if (rightCorner != null)
        {
            Gizmos.color = Color.magenta;
            Gizmos.DrawWireSphere(rightCorner.position, 0.1f);
            Gizmos.DrawLine(transform.position, rightCorner.position);
        }
        
        if (leftCorner != null && rightCorner != null)
        {
            Gizmos.color = Color.green;
            Gizmos.DrawLine(leftCorner.position, rightCorner.position);
        }
    }
}
