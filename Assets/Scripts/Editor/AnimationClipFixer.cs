using UnityEngine;
using UnityEditor;

public class AnimationClipFixer : EditorWindow
{
    private AnimationClip clipToFix;
    private Sprite[] spritesToUse;
    private float animationDuration = 1.0f;
    
    [MenuItem("Tools/Fix Death Animation Clip")]
    static void ShowWindow()
    {
        GetWindow<AnimationClipFixer>("Animation Clip Fixer");
    }
    
    void OnGUI()
    {
        GUILayout.Label("Fix Empty Animation Clip", EditorStyles.boldLabel);
        
        EditorGUILayout.Space();
        
        clipToFix = (AnimationClip)EditorGUILayout.ObjectField("Clip to Fix", clipToFix, typeof(AnimationClip), false);
        
        EditorGUILayout.Space();
        
        GUILayout.Label("Sprites to Add (in order):", EditorStyles.label);
        
        SerializedObject so = new SerializedObject(this);
        SerializedProperty spritesProperty = so.FindProperty("spritesToUse");
        EditorGUILayout.PropertyField(spritesProperty, true);
        so.ApplyModifiedProperties();
        
        EditorGUILayout.Space();
        
        animationDuration = EditorGUILayout.FloatField("Animation Duration (seconds)", animationDuration);
        
        EditorGUILayout.Space();
        
        if (GUILayout.Button("Validate Clip"))
        {
            ValidateClip();
        }
        
        EditorGUILayout.Space();
        
        if (GUILayout.Button("Fix Clip (Add Keyframes)"))
        {
            FixClip();
        }
    }
    
    void ValidateClip()
    {
        if (clipToFix == null)
        {
            Debug.LogError("[CLIP FIXER] No clip assigned!");
            return;
        }
        
        Debug.Log($"<color=yellow>═══════════════════════════════════════</color>");
        Debug.Log($"<color=yellow>   VALIDATING: {clipToFix.name}</color>");
        Debug.Log($"<color=yellow>═══════════════════════════════════════</color>");
        
        Debug.Log($"<color=cyan>Length:</color> {clipToFix.length} seconds");
        Debug.Log($"<color=cyan>Frame Rate:</color> {clipToFix.frameRate}");
        Debug.Log($"<color=cyan>Is Legacy:</color> {clipToFix.legacy}");
        
        EditorCurveBinding[] objectBindings = AnimationUtility.GetObjectReferenceCurveBindings(clipToFix);
        Debug.Log($"<color=green>Object Reference Bindings:</color> {objectBindings.Length}");
        
        foreach (var binding in objectBindings)
        {
            ObjectReferenceKeyframe[] keyframes = AnimationUtility.GetObjectReferenceCurve(clipToFix, binding);
            Debug.Log($"  - Property: {binding.propertyName} | Keyframes: {keyframes.Length}");
        }
        
        if (objectBindings.Length == 0)
        {
            Debug.LogError($"<color=red>❌ CLIP IS EMPTY!</color>");
        }
        else
        {
            Debug.Log($"<color=green>✅ Clip has {objectBindings.Length} binding(s)</color>");
        }
        
        Debug.Log($"<color=yellow>═══════════════════════════════════════</color>");
    }
    
    void FixClip()
    {
        if (clipToFix == null)
        {
            Debug.LogError("[CLIP FIXER] No clip assigned!");
            return;
        }
        
        if (spritesToUse == null || spritesToUse.Length == 0)
        {
            Debug.LogError("[CLIP FIXER] No sprites assigned! Please add sprites to the array.");
            return;
        }
        
        if (animationDuration <= 0)
        {
            Debug.LogError("[CLIP FIXER] Animation duration must be greater than 0!");
            return;
        }
        
        Debug.Log($"<color=yellow>Fixing clip '{clipToFix.name}' with {spritesToUse.Length} sprites over {animationDuration}s...</color>");
        
        ObjectReferenceKeyframe[] keyframes = new ObjectReferenceKeyframe[spritesToUse.Length];
        
        for (int i = 0; i < spritesToUse.Length; i++)
        {
            if (spritesToUse[i] == null)
            {
                Debug.LogWarning($"[CLIP FIXER] Sprite at index {i} is null! Skipping...");
                continue;
            }
            
            float time = (animationDuration / (spritesToUse.Length - 1)) * i;
            if (spritesToUse.Length == 1)
                time = 0f;
            
            keyframes[i] = new ObjectReferenceKeyframe
            {
                time = time,
                value = spritesToUse[i]
            };
            
            Debug.Log($"  <color=cyan>Keyframe {i}:</color> Time={time:F3}s, Sprite={spritesToUse[i].name}");
        }
        
        EditorCurveBinding spriteBinding = new EditorCurveBinding
        {
            type = typeof(SpriteRenderer),
            path = "",
            propertyName = "m_Sprite"
        };
        
        AnimationUtility.SetObjectReferenceCurve(clipToFix, spriteBinding, keyframes);
        
        EditorUtility.SetDirty(clipToFix);
        AssetDatabase.SaveAssets();
        
        Debug.Log($"<color=green>✅ Clip fixed! {keyframes.Length} keyframes added.</color>");
        Debug.Log($"<color=yellow>Please test the animation in Play Mode.</color>");
    }
}
