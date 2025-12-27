using UnityEngine;

public class AnimatorControllerChecker : MonoBehaviour
{
    private Animator animator;

    private void Start()
    {
        animator = GetComponent<Animator>();
        
        if (animator == null)
        {
            Debug.LogError("<color=red>[ANIMATOR CHECKER] No Animator component found!</color>");
            return;
        }

        if (animator.runtimeAnimatorController == null)
        {
            Debug.LogError("<color=red>[ANIMATOR CHECKER] RuntimeAnimatorController is NULL!</color>");
            return;
        }

        Debug.Log($"<color=cyan>[ANIMATOR CHECKER] Using Controller: {animator.runtimeAnimatorController.name}</color>");
        
        var parameters = animator.parameters;
        Debug.Log($"<color=yellow>[ANIMATOR CHECKER] Parameters Count: {parameters.Length}</color>");
        
        foreach (var param in parameters)
        {
            Debug.Log($"  Parameter: {param.name} | Type: {param.type}");
        }
        
        bool hasAim = animator.HasState(0, Animator.StringToHash("Aim"));
        bool hasFire = animator.HasState(0, Animator.StringToHash("Fire"));
        bool hasReload = animator.HasState(0, Animator.StringToHash("Reload"));
        
        Debug.Log($"<color=magenta>[ANIMATOR CHECKER] States Found:</color>");
        Debug.Log($"  Aim: {hasAim}");
        Debug.Log($"  Fire: {hasFire}");
        Debug.Log($"  Reload: {hasReload}");
        
        if (!hasAim || !hasFire || !hasReload)
        {
            Debug.LogError("<color=red>[ANIMATOR CHECKER] ⚠️ MISSING STATES IN RUNTIME CONTROLLER!</color>");
        }
    }

    [ContextMenu("Check Current Animator State")]
    public void CheckCurrentState()
    {
        if (animator == null) animator = GetComponent<Animator>();
        
        var currentState = animator.GetCurrentAnimatorStateInfo(0);
        Debug.Log($"<color=cyan>[CURRENT STATE] Hash: {currentState.fullPathHash} | NormalizedTime: {currentState.normalizedTime}</color>");
        
        bool isAim = currentState.IsName("Aim");
        bool isFire = currentState.IsName("Fire");
        bool isReload = currentState.IsName("Reload");
        
        Debug.Log($"  Is Aim: {isAim}");
        Debug.Log($"  Is Fire: {isFire}");
        Debug.Log($"  Is Reload: {isReload}");
        
        Debug.Log($"<color=yellow>[PARAMETERS]</color>");
        Debug.Log($"  aim: {animator.GetBool("aim")}");
        Debug.Log($"  fire: {animator.GetBool("fire")}");
        Debug.Log($"  reload: {animator.GetBool("reload")}");
    }
}
