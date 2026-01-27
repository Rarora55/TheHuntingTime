using UnityEngine;

public class DeathClipRuntimeValidator : MonoBehaviour
{
    private Animator animator;
    private bool hasValidated = false;
    
    void Start()
    {
        animator = GetComponent<Animator>();
        ValidateDeathClips();
    }
    
    void Update()
    {
        if (!hasValidated && animator != null)
        {
            AnimatorStateInfo stateInfo = animator.GetCurrentAnimatorStateInfo(0);
            
            if (stateInfo.IsName("death") || stateInfo.IsName("Death") || stateInfo.IsName("deathAnim"))
            {
                AnimationClip currentClip = GetCurrentAnimationClip();
                if (currentClip != null)
                {
                    Debug.Log("<color=yellow>═══════════════════════════════════════</color>");
                    Debug.Log($"<color=yellow>   DEATH CLIP IN USE: {currentClip.name}</color>");
                    Debug.Log("<color=yellow>═══════════════════════════════════════</color>");
                    Debug.Log($"<color=cyan>Length:</color> {currentClip.length}s");
                    Debug.Log($"<color=cyan>Frame Rate:</color> {currentClip.frameRate}");
                    Debug.Log($"<color=cyan>Is Looping:</color> {currentClip.isLooping}");
                    Debug.Log($"<color=cyan>Is Empty:</color> {currentClip.empty}");
                    Debug.Log("<color=yellow>═══════════════════════════════════════</color>");
                    
                    hasValidated = true;
                }
            }
        }
    }
    
    void ValidateDeathClips()
    {
        if (animator == null || animator.runtimeAnimatorController == null)
        {
            Debug.LogError("<color=red>[DEATH CLIP VALIDATOR] No Animator or Controller found!</color>");
            return;
        }
        
        Debug.Log("<color=magenta>═══════════════════════════════════════</color>");
        Debug.Log("<color=magenta>   SEARCHING FOR DEATH ANIMATION CLIPS</color>");
        Debug.Log("<color=magenta>═══════════════════════════════════════</color>");
        
        AnimationClip[] clips = animator.runtimeAnimatorController.animationClips;
        foreach (AnimationClip clip in clips)
        {
            string lowerName = clip.name.ToLower();
            if (lowerName.Contains("death"))
            {
                Debug.Log($"<color=cyan>Found Death Clip:</color> {clip.name}");
                Debug.Log($"  - Length: {clip.length}s");
                Debug.Log($"  - Frame Rate: {clip.frameRate}");
                Debug.Log($"  - Is Looping: {clip.isLooping}");
                Debug.Log($"  - Is Empty: {clip.empty}");
                
                if (clip.empty)
                {
                    Debug.LogError($"<color=red>❌ CLIP '{clip.name}' IS EMPTY!</color>");
                }
            }
        }
        
        Debug.Log("<color=magenta>═══════════════════════════════════════</color>");
    }
    
    AnimationClip GetCurrentAnimationClip()
    {
        if (animator == null || animator.runtimeAnimatorController == null)
            return null;
        
        AnimatorClipInfo[] clipInfo = animator.GetCurrentAnimatorClipInfo(0);
        if (clipInfo.Length > 0)
        {
            return clipInfo[0].clip;
        }
        
        return null;
    }
}
