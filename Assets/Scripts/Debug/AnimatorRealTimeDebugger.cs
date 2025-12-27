using UnityEngine;

public class AnimatorRealTimeDebugger : MonoBehaviour
{
    private Animator animator;
    private PlayerInputHandler inputHandler;
    private int lastStateHash = -1;
    private float lastNormalizedTime = -1f;

    private void Start()
    {
        animator = GetComponent<Animator>();
        inputHandler = GetComponent<PlayerInputHandler>();
    }

    private void Update()
    {
        if (animator == null || inputHandler == null)
            return;

        var stateInfo = animator.GetCurrentAnimatorStateInfo(0);
        int currentHash = stateInfo.fullPathHash;
        float currentNormalizedTime = stateInfo.normalizedTime;

        if (currentHash != lastStateHash)
        {
            Debug.Log($"<color=cyan>[ANIMATOR RT] State changed to: {currentHash} | IsName(Aim): {stateInfo.IsName("Aim")} | IsName(Fire): {stateInfo.IsName("Fire")}</color>");
            lastStateHash = currentHash;
        }

        if (stateInfo.IsName("Aim"))
        {
            bool aimParam = animator.GetBool("aim");
            bool fireParam = animator.GetBool("fire");
            bool reloadParam = animator.GetBool("reload");
            
            if (Mathf.Abs(currentNormalizedTime - lastNormalizedTime) > 0.01f)
            {
                Debug.Log($"<color=yellow>[AIM RT] NormalizedTime: {currentNormalizedTime:F3} | aim={aimParam} fire={fireParam} reload={reloadParam} | Input: Aim={inputHandler.AimInput} Fire={inputHandler.FireInput}</color>");
                lastNormalizedTime = currentNormalizedTime;
            }
            
            if (animator.IsInTransition(0))
            {
                var transitionInfo = animator.GetAnimatorTransitionInfo(0);
                Debug.Log($"<color=magenta>[AIM RT] IN TRANSITION! Duration: {transitionInfo.duration} | NormalizedTime: {transitionInfo.normalizedTime}</color>");
            }
            else if (fireParam || reloadParam)
            {
                Debug.LogWarning($"<color=red>[AIM RT] ⚠️ fire={fireParam} or reload={reloadParam} but NOT transitioning!</color>");
            }
        }
        
        if (stateInfo.IsName("Fire"))
        {
            Debug.Log($"<color=green>[FIRE RT] In Fire state! NormalizedTime: {stateInfo.normalizedTime:F3}</color>");
        }
    }
}
