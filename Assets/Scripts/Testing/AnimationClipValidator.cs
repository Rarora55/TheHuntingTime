using UnityEngine;
using UnityEditor;

public class AnimationClipValidator : MonoBehaviour
{
    [Header("Animation Clip to Validate")]
    [SerializeField] private AnimationClip clipToValidate;
    
    [ContextMenu("Validate Animation Clip")]
    void ValidateClip()
    {
        if (clipToValidate == null)
        {
            Debug.LogError("[CLIP VALIDATOR] No clip assigned!");
            return;
        }
        
        Debug.Log($"<color=yellow>═══════════════════════════════════════</color>");
        Debug.Log($"<color=yellow>   VALIDATING CLIP: {clipToValidate.name}</color>");
        Debug.Log($"<color=yellow>═══════════════════════════════════════</color>");
        
        Debug.Log($"<color=cyan>Clip Name:</color> {clipToValidate.name}");
        Debug.Log($"<color=cyan>Length:</color> {clipToValidate.length} seconds");
        Debug.Log($"<color=cyan>Frame Rate:</color> {clipToValidate.frameRate}");
        Debug.Log($"<color=cyan>Is Looping:</color> {clipToValidate.isLooping}");
        Debug.Log($"<color=cyan>Is Legacy:</color> {clipToValidate.legacy}");
        
        EditorCurveBinding[] bindings = AnimationUtility.GetCurveBindings(clipToValidate);
        EditorCurveBinding[] objectBindings = AnimationUtility.GetObjectReferenceCurveBindings(clipToValidate);
        
        Debug.Log($"<color=green>Float Curve Bindings:</color> {bindings.Length}");
        foreach (var binding in bindings)
        {
            AnimationCurve curve = AnimationUtility.GetEditorCurve(clipToValidate, binding);
            Debug.Log($"  - Path: {binding.path} | Property: {binding.propertyName} | Keyframes: {curve.keys.Length}");
        }
        
        Debug.Log($"<color=green>Object Reference Bindings:</color> {objectBindings.Length}");
        foreach (var binding in objectBindings)
        {
            ObjectReferenceKeyframe[] keyframes = AnimationUtility.GetObjectReferenceCurve(clipToValidate, binding);
            Debug.Log($"  - Path: {binding.path} | Property: {binding.propertyName} | Keyframes: {keyframes.Length}");
            
            if (keyframes.Length > 0)
            {
                Debug.Log($"    <color=cyan>First Keyframe Time:</color> {keyframes[0].time}s");
                Debug.Log($"    <color=cyan>Last Keyframe Time:</color> {keyframes[keyframes.Length - 1].time}s");
            }
        }
        
        if (bindings.Length == 0 && objectBindings.Length == 0)
        {
            Debug.LogError($"<color=red>❌ CLIP IS EMPTY! No bindings found!</color>");
        }
        else if (objectBindings.Length == 0)
        {
            Debug.LogWarning($"<color=orange>⚠️ No sprite keyframes found! This clip won't animate sprites.</color>");
        }
        else
        {
            Debug.Log($"<color=green>✅ Clip has valid data!</color>");
        }
        
        Debug.Log($"<color=yellow>═══════════════════════════════════════</color>");
    }
}
