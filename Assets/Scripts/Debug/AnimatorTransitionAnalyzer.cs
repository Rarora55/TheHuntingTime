using UnityEngine;

public class AnimatorTransitionAnalyzer : MonoBehaviour
{
    private Animator animator;
    private int previousStateHash = 0;
    private string previousStateName = "";

    private void Start()
    {
        animator = GetComponent<Animator>();
    }

    private void Update()
    {
        if (animator == null) return;

        AnimatorStateInfo currentState = animator.GetCurrentAnimatorStateInfo(0);
        AnimatorClipInfo[] clipInfo = animator.GetCurrentAnimatorClipInfo(0);
        
        string currentStateName = clipInfo.Length > 0 ? clipInfo[0].clip.name : "Unknown";
        
        if (currentState.fullPathHash != previousStateHash)
        {
            Debug.Log($"<color=cyan>[ANIMATOR TRANSITION] {previousStateName} → {currentStateName}</color>");
            Debug.Log($"<color=cyan>  Hash: {previousStateHash} → {currentState.fullPathHash}</color>");
            Debug.Log($"<color=cyan>  Parameters: aim={animator.GetBool("aim")} | fire={animator.GetBool("fire")} | reload={animator.GetBool("reload")}</color>");
            
            previousStateHash = currentState.fullPathHash;
            previousStateName = currentStateName;
        }

        if (animator.IsInTransition(0))
        {
            AnimatorTransitionInfo transInfo = animator.GetAnimatorTransitionInfo(0);
            AnimatorClipInfo[] nextClipInfo = animator.GetNextAnimatorClipInfo(0);
            string nextStateName = nextClipInfo.Length > 0 ? nextClipInfo[0].clip.name : "Unknown";
            
            Debug.Log($"<color=yellow>[TRANSITION] {currentStateName} → {nextStateName} | Progress: {transInfo.normalizedTime:F2}</color>");
        }
    }

    [ContextMenu("Force Log State")]
    public void ForceLogState()
    {
        if (animator == null) return;

        AnimatorStateInfo currentState = animator.GetCurrentAnimatorStateInfo(0);
        AnimatorClipInfo[] clipInfo = animator.GetCurrentAnimatorClipInfo(0);
        string currentStateName = clipInfo.Length > 0 ? clipInfo[0].clip.name : "Unknown";

        Debug.Log("<color=magenta>=== FORCE LOG ===</color>");
        Debug.Log($"Current State: {currentStateName}");
        Debug.Log($"Hash: {currentState.fullPathHash}");
        Debug.Log($"Normalized Time: {currentState.normalizedTime}");
        Debug.Log($"Parameters:");
        Debug.Log($"  aim: {animator.GetBool("aim")}");
        Debug.Log($"  fire: {animator.GetBool("fire")}");
        Debug.Log($"  reload: {animator.GetBool("reload")}");
        Debug.Log($"  inAir: {animator.GetBool("inAir")}");
        Debug.Log($"  isRunning: {animator.GetBool("isRunning")}");
        Debug.Log($"In Transition: {animator.IsInTransition(0)}");
        Debug.Log("<color=magenta>=================</color>");
    }
}
