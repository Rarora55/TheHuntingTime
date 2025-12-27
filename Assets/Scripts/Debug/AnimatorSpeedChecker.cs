using UnityEngine;

public class AnimatorSpeedChecker : MonoBehaviour
{
    private Animator animator;

    private void Start()
    {
        animator = GetComponent<Animator>();
        
        if (animator == null)
        {
            Debug.LogError("<color=red>[SPEED CHECKER] No Animator found!</color>");
            return;
        }

        Debug.Log($"<color=cyan>[SPEED CHECKER] Animator Speed: {animator.speed}</color>");
        
        if (animator.speed == 0)
        {
            Debug.LogError("<color=red>[SPEED CHECKER] ⚠️ ANIMATOR SPEED IS ZERO! Animations are frozen!</color>");
        }
    }

    private void Update()
    {
        if (animator == null) return;

        var stateInfo = animator.GetCurrentAnimatorStateInfo(0);
        
        if (stateInfo.IsName("Aim"))
        {
            Debug.Log($"<color=yellow>[AIM ANIMATION] NormalizedTime: {stateInfo.normalizedTime} | Speed: {stateInfo.speed} | Length: {stateInfo.length}</color>");
            
            if (stateInfo.speed == 0)
            {
                Debug.LogError("<color=red>[AIM ANIMATION] ⚠️ AIM STATE SPEED IS ZERO!</color>");
            }
            
            if (stateInfo.length == 0)
            {
                Debug.LogError("<color=red>[AIM ANIMATION] ⚠️ AIM CLIP LENGTH IS ZERO!</color>");
            }
        }
    }

    [ContextMenu("Check Animator Speed Now")]
    public void CheckSpeedNow()
    {
        if (animator == null) animator = GetComponent<Animator>();
        
        Debug.Log($"<color=cyan>========== ANIMATOR SPEED CHECK ==========</color>");
        Debug.Log($"Animator Speed: {animator.speed}");
        Debug.Log($"Animator Enabled: {animator.enabled}");
        Debug.Log($"Animator Update Mode: {animator.updateMode}");
        
        var stateInfo = animator.GetCurrentAnimatorStateInfo(0);
        Debug.Log($"Current State Speed: {stateInfo.speed}");
        Debug.Log($"Current State Length: {stateInfo.length}");
        Debug.Log($"Current State NormalizedTime: {stateInfo.normalizedTime}");
        Debug.Log($"<color=cyan>==========================================</color>");
    }
}
