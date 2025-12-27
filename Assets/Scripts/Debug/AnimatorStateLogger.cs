using UnityEngine;

public class AnimatorStateLogger : MonoBehaviour
{
    private Animator animator;
    private int lastStateHash = 0;
    private string lastStateName = "";
    
    private void Awake()
    {
        animator = GetComponent<Animator>();
    }
    
    private void Update()
    {
        if (animator == null || !animator.isInitialized) return;
        
        var currentState = animator.GetCurrentAnimatorStateInfo(0);
        
        if (currentState.shortNameHash != lastStateHash)
        {
            lastStateHash = currentState.shortNameHash;
            lastStateName = GetStateNameFromHash(currentState.shortNameHash);
            
            bool aim = animator.GetBool("aim");
            bool fire = animator.GetBool("fire");
            bool reload = animator.GetBool("reload");
            
            Debug.Log($"<color=yellow>╔════════════════════════════════════════╗</color>");
            Debug.Log($"<color=yellow>║ ANIMATOR STATE CHANGED                 ║</color>");
            Debug.Log($"<color=yellow>╠════════════════════════════════════════╣</color>");
            Debug.Log($"<color=cyan>║ New State: {lastStateName,-28}║</color>");
            Debug.Log($"<color=cyan>║ Hash: {currentState.shortNameHash,-33}║</color>");
            Debug.Log($"<color=green>║ aim={aim,-35}║</color>");
            Debug.Log($"<color=green>║ fire={fire,-34}║</color>");
            Debug.Log($"<color=green>║ reload={reload,-32}║</color>");
            Debug.Log($"<color=yellow>╚════════════════════════════════════════╝</color>");
        }
        
        bool currentAim = animator.GetBool("aim");
        bool currentFire = animator.GetBool("fire");
        bool currentReload = animator.GetBool("reload");
        
        if (currentFire || currentReload)
        {
            if (animator.IsInTransition(0))
            {
                var nextState = animator.GetNextAnimatorStateInfo(0);
                Debug.Log($"<color=magenta>[TRANSITION] From {lastStateName} → Next Hash: {nextState.shortNameHash}</color>");
            }
        }
    }
    
    private string GetStateNameFromHash(int hash)
    {
        if (hash == Animator.StringToHash("Idle")) return "Idle";
        if (hash == Animator.StringToHash("Move")) return "Move";
        if (hash == Animator.StringToHash("Aim")) return "Aim";
        if (hash == Animator.StringToHash("Fire")) return "Fire";
        if (hash == Animator.StringToHash("Reload")) return "Reload";
        if (hash == Animator.StringToHash("InAir")) return "InAir";
        if (hash == Animator.StringToHash("Crouch")) return "Crouch";
        
        return $"Unknown ({hash})";
    }
}
