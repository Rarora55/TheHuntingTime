using UnityEngine;

public class AnimatorParameterMonitor : MonoBehaviour
{
    [SerializeField] private Animator animator;
    
    private bool lastDeathValue;
    private bool lastIdleValue;
    private bool initialized;
    
    void Start()
    {
        if (animator == null)
            animator = GetComponent<Animator>();
            
        if (animator != null)
        {
            lastDeathValue = animator.GetBool("death");
            lastIdleValue = animator.GetBool("idle");
            initialized = true;
            
            Debug.Log($"<color=cyan>[PARAM MONITOR] Initialized - death: {lastDeathValue}, idle: {lastIdleValue}</color>");
        }
    }
    
    void Update()
    {
        if (!initialized || animator == null)
            return;
            
        bool currentDeath = animator.GetBool("death");
        bool currentIdle = animator.GetBool("idle");
        
        if (currentDeath != lastDeathValue)
        {
            Debug.Log($"<color=yellow>[PARAM MONITOR] 'death' changed: {lastDeathValue} → {currentDeath}</color>");
            lastDeathValue = currentDeath;
        }
        
        if (currentIdle != lastIdleValue)
        {
            Debug.Log($"<color=yellow>[PARAM MONITOR] 'idle' changed: {lastIdleValue} → {currentIdle}</color>");
            lastIdleValue = currentIdle;
        }
        
        AnimatorStateInfo stateInfo = animator.GetCurrentAnimatorStateInfo(0);
        
        if (stateInfo.IsName("death"))
        {
            if (currentIdle)
            {
                Debug.LogWarning($"<color=red>[PARAM MONITOR] ⚠️ IDLE IS TRUE DURING DEATH STATE! Time: {stateInfo.normalizedTime:F2}</color>");
            }
        }
    }
}
