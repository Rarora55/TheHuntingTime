using UnityEngine;

public class AnimatorTransitionDebugger : MonoBehaviour
{
    private Animator animator;
    
    private void Awake()
    {
        animator = GetComponent<Animator>();
    }
    
    private void Update()
    {
        if (animator == null) return;
        
        if (!animator.isInitialized)
        {
            Debug.LogWarning("[ANIMATOR DEBUG] Animator not initialized!");
            return;
        }
        
        var currentState = animator.GetCurrentAnimatorStateInfo(0);
        
        bool aim = animator.GetBool("aim");
        bool fire = animator.GetBool("fire");
        bool reload = animator.GetBool("reload");
        
        if (fire || reload)
        {
            Debug.Log($"<color=yellow>[ANIMATOR STATE] Current: {currentState.shortNameHash} | aim={aim}, fire={fire}, reload={reload} | NormalizedTime: {currentState.normalizedTime}</color>");
            
            if (animator.IsInTransition(0))
            {
                var nextState = animator.GetNextAnimatorStateInfo(0);
                Debug.Log($"<color=green>[ANIMATOR] In transition! Next state: {nextState.shortNameHash}</color>");
            }
            else
            {
                Debug.Log($"<color=red>[ANIMATOR] NOT in transition despite fire={fire} or reload={reload}</color>");
            }
        }
    }
}
