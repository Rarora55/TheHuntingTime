using UnityEngine;

#if UNITY_EDITOR
using UnityEditor;
#endif

/// <summary>
/// Helper para añadir LedgeMarkers automáticamente a plataformas.
/// </summary>
public class LedgeMarkerHelper : MonoBehaviour
{
    [Header("Settings")]
    [Tooltip("Offset desde los bordes del collider para colocar los corners")]
    [SerializeField] private float cornerOffset = 0.1f;
    
    [Header("Info (Read Only)")]
    [SerializeField] private bool hasLedgeMarker;
    
    private void OnValidate()
    {
        hasLedgeMarker = GetComponent<LedgeMarker>() != null;
    }
    
#if UNITY_EDITOR
    [ContextMenu("Add LedgeMarker Automatically")]
    public void AddLedgeMarkerAuto()
    {
        // Verificar si ya tiene LedgeMarker
        LedgeMarker existing = GetComponent<LedgeMarker>();
        if (existing != null)
        {
            Debug.LogWarning($"<color=yellow>[LEDGE HELPER] {gameObject.name} ya tiene un LedgeMarker!</color>");
            return;
        }
        
        // Buscar el collider principal
        BoxCollider2D boxCollider = GetComponent<BoxCollider2D>();
        if (boxCollider == null)
        {
            Debug.LogError($"<color=red>[LEDGE HELPER] {gameObject.name} no tiene BoxCollider2D!</color>");
            return;
        }
        
        // Calcular los corners basado en el bounds del collider
        Bounds bounds = boxCollider.bounds;
        
        Vector2 leftCorner = new Vector2(bounds.min.x + cornerOffset, bounds.max.y);
        Vector2 rightCorner = new Vector2(bounds.max.x - cornerOffset, bounds.max.y);
        
        // Añadir LedgeMarker
        LedgeMarker marker = gameObject.AddComponent<LedgeMarker>();
        
        // Usar SerializedObject para asignar los corners privados
        SerializedObject so = new SerializedObject(marker);
        so.FindProperty("leftCorner").vector2Value = leftCorner;
        so.FindProperty("rightCorner").vector2Value = rightCorner;
        so.ApplyModifiedProperties();
        
        Debug.Log($"<color=green>[LEDGE HELPER] ✅ LedgeMarker añadido a {gameObject.name}!</color>");
        Debug.Log($"<color=cyan>  Left Corner: {leftCorner} | Right Corner: {rightCorner}</color>");
        
        EditorUtility.SetDirty(gameObject);
    }
    
    [ContextMenu("Remove LedgeMarker")]
    public void RemoveLedgeMarker()
    {
        LedgeMarker marker = GetComponent<LedgeMarker>();
        if (marker != null)
        {
            DestroyImmediate(marker);
            Debug.Log($"<color=yellow>[LEDGE HELPER] LedgeMarker removido de {gameObject.name}</color>");
        }
    }
#endif
}
