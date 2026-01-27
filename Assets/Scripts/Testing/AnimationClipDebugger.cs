using UnityEngine;

public class AnimationClipDebugger : MonoBehaviour
{
    [Header("References")]
    [SerializeField] private Animator animator;
    [SerializeField] private SpriteRenderer spriteRenderer;
    
    private Sprite lastSprite;
    private string lastClipName;
    
    void Start()
    {
        if (animator == null)
            animator = GetComponent<Animator>();
            
        if (spriteRenderer == null)
            spriteRenderer = GetComponent<SpriteRenderer>();
    }
    
    void Update()
    {
        if (spriteRenderer != null && spriteRenderer.sprite != lastSprite)
        {
            lastSprite = spriteRenderer.sprite;
            Debug.Log($"<color=cyan>[SPRITE CHANGE] New sprite: {lastSprite?.name ?? "NULL"}</color>");
        }
        
        if (animator != null)
        {
            AnimatorClipInfo[] clipInfo = animator.GetCurrentAnimatorClipInfo(0);
            if (clipInfo.Length > 0)
            {
                string currentClipName = clipInfo[0].clip.name;
                if (currentClipName != lastClipName)
                {
                    lastClipName = currentClipName;
                    Debug.Log($"<color=yellow>[CLIP CHANGE] Now playing: {currentClipName}</color>");
                }
            }
        }
    }

    void OnGUI()
    {
        GUILayout.BeginArea(new Rect(470, 10, 400, 400));
        GUILayout.Box("🔍 ANIMATION CLIP DEBUGGER");
        
        if (animator != null)
        {
            AnimatorClipInfo[] clipInfo = animator.GetCurrentAnimatorClipInfo(0);
            
            GUILayout.Label($"Current Clips: {clipInfo.Length}");
            
            foreach (var info in clipInfo)
            {
                AnimationClip clip = info.clip;
                GUI.color = Color.cyan;
                GUILayout.Label($"• {clip.name}");
                GUI.color = Color.white;
                GUILayout.Label($"  Length: {clip.length:F2}s");
                GUILayout.Label($"  Frame Rate: {clip.frameRate} fps");
                GUILayout.Label($"  Total Frames: {(int)(clip.length * clip.frameRate)}");
                GUILayout.Label($"  Is Looping: {clip.isLooping}");
                GUILayout.Label($"  Empty: {clip.empty}");
                
                if (clip.empty)
                {
                    GUI.color = Color.red;
                    GUILayout.Label("  ⚠️ CLIP IS EMPTY!");
                    GUI.color = Color.white;
                }
            }
            
            AnimatorStateInfo stateInfo = animator.GetCurrentAnimatorStateInfo(0);
            GUILayout.Space(10);
            GUILayout.Label("Animator State:");
            GUILayout.Label($"  Normalized Time: {stateInfo.normalizedTime:F2}");
            GUILayout.Label($"  Length: {stateInfo.length:F2}s");
            GUILayout.Label($"  Speed: {stateInfo.speed:F2}");
            GUILayout.Label($"  Speed Multiplier: {stateInfo.speedMultiplier:F2}");
        }
        
        if (spriteRenderer != null)
        {
            GUILayout.Space(10);
            GUILayout.Label("Current Sprite:");
            if (spriteRenderer.sprite != null)
            {
                GUI.color = Color.green;
                GUILayout.Label($"  {spriteRenderer.sprite.name}");
                GUI.color = Color.white;
            }
            else
            {
                GUI.color = Color.red;
                GUILayout.Label("  NULL");
                GUI.color = Color.white;
            }
        }
        
        GUILayout.EndArea();
    }
}
