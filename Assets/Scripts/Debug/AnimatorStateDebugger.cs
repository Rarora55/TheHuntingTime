using UnityEngine;

public class AnimatorStateDebugger : MonoBehaviour
{
    [SerializeField] private Animator animator;
    [SerializeField] private bool logOnUpdate = true;
    
    private int lastStateHash = 0;
    private float lastNormalizedTime = 0f;
    
    private void Awake()
    {
        if (animator == null)
            animator = GetComponent<Animator>();
    }
    
    private void Update()
    {
        if (!logOnUpdate || animator == null)
            return;
        
        AnimatorStateInfo stateInfo = animator.GetCurrentAnimatorStateInfo(0);
        
        if (stateInfo.fullPathHash != lastStateHash)
        {
            Debug.Log($"<color=cyan>[ANIMATOR STATE] Changed to: {GetStateName(stateInfo)} | Hash: {stateInfo.fullPathHash}</color>");
            lastStateHash = stateInfo.fullPathHash;
        }
        
        if (Mathf.Abs(stateInfo.normalizedTime - lastNormalizedTime) > 0.1f)
        {
            Debug.Log($"<color=yellow>[ANIMATOR STATE] {GetStateName(stateInfo)} | NormalizedTime: {stateInfo.normalizedTime:F2} | Speed: {stateInfo.speed} | AnimSpeed: {animator.speed}</color>");
            lastNormalizedTime = stateInfo.normalizedTime;
        }
    }
    
    [ContextMenu("Log Current State Info")]
    public void LogCurrentState()
    {
        if (animator == null)
        {
            Debug.LogError("[ANIMATOR STATE] Animator is null!");
            return;
        }
        
        AnimatorStateInfo stateInfo = animator.GetCurrentAnimatorStateInfo(0);
        
        Debug.Log($"<color=magenta>========== ANIMATOR STATE INFO ==========</color>");
        Debug.Log($"Current State: {GetStateName(stateInfo)}");
        Debug.Log($"Full Path Hash: {stateInfo.fullPathHash}");
        Debug.Log($"Normalized Time: {stateInfo.normalizedTime}");
        Debug.Log($"Length: {stateInfo.length}");
        Debug.Log($"Speed: {stateInfo.speed}");
        Debug.Log($"Speed Multiplier: {stateInfo.speedMultiplier}");
        Debug.Log($"Animator Speed: {animator.speed}");
        Debug.Log($"Is In Transition: {animator.IsInTransition(0)}");
        
        if (animator.IsInTransition(0))
        {
            AnimatorTransitionInfo transitionInfo = animator.GetAnimatorTransitionInfo(0);
            Debug.Log($"<color=cyan>--- TRANSITION INFO ---</color>");
            Debug.Log($"Transition Normalized Time: {transitionInfo.normalizedTime}");
            Debug.Log($"Transition Duration: {transitionInfo.duration}");
        }
        
        Debug.Log($"<color=cyan>--- PARAMETERS ---</color>");
        Debug.Log($"aim: {animator.GetBool("aim")}");
        Debug.Log($"fire: {animator.GetBool("fire")}");
        Debug.Log($"reload: {animator.GetBool("reload")}");
        Debug.Log($"<color=magenta>=====================================</color>");
    }
    
    private string GetStateName(AnimatorStateInfo stateInfo)
    {
        if (stateInfo.IsName("Idle")) return "Idle";
        if (stateInfo.IsName("Move")) return "Move";
        if (stateInfo.IsName("Jump")) return "Jump";
        if (stateInfo.IsName("Air")) return "Air";
        if (stateInfo.IsName("Land")) return "Land";
        if (stateInfo.IsName("Aim")) return "Aim";
        if (stateInfo.IsName("Fire")) return "Fire";
        if (stateInfo.IsName("Reload")) return "Reload";
        if (stateInfo.IsName("LedgeClimb")) return "LedgeClimb";
        if (stateInfo.IsName("WallSlide")) return "WallSlide";
        if (stateInfo.IsName("WallGrab")) return "WallGrab";
        
        return $"Unknown ({stateInfo.fullPathHash})";
    }
}
