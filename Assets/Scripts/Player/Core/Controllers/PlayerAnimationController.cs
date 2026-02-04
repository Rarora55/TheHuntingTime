using UnityEngine;

public class PlayerAnimationController : IPlayerAnimation
{
    private readonly Animator animator;
    private readonly PlayerStateMachine stateMachine;
    private readonly PlayerEvents events;
    
    public PlayerAnimationController(Animator anim, PlayerStateMachine machine, PlayerEvents playerEvents)
    {
        animator = anim;
        stateMachine = machine;
        events = playerEvents;
    }
    
    public void SetBool(string parameterName, bool value)
    {
        if (animator == null) return;
        
        if (parameterName == "fire" || parameterName == "reload" || parameterName == "aim" || 
            parameterName == "push" || parameterName == "pull")
        {
            try
            {
                var currentValue = animator.GetBool(parameterName);
                if (currentValue != value)
                {
                    Debug.Log($"<color=orange>━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━</color>");
                    Debug.Log($"<color=orange>[ANIMATOR BOOL CHANGE] {parameterName}: {currentValue} → {value}</color>");
                    Debug.Log($"<color=orange>━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━</color>");
                }
            }
            catch (System.Exception)
            {
                Debug.LogError($"<color=red>⚠️ ANIMATOR PARAMETER '{parameterName}' DOES NOT EXIST!</color>");
                Debug.LogError($"<color=yellow>→ Open 'Player.controller' and add a Bool parameter named '{parameterName}'</color>");
            }
        }
        
        animator.SetBool(parameterName, value);
    }
    
    public void SetFloat(string parameterName, float value)
    {
        if (animator == null) return;
        animator.SetFloat(parameterName, value);
    }
    
    public void SetTrigger(string parameterName)
    {
        if (animator == null) return;
        animator.SetTrigger(parameterName);
    }
    
    public void AnimationTrigger()
    {
        stateMachine?.CurrentState?.AnimationTrigger();
        
        events?.InvokeAnimationTrigger(new PlayerAnimationEventData
        {
            TriggerName = "AnimationTrigger",
            StateName = stateMachine?.CurrentState?.GetType().Name
        });
    }
    
    public void AnimationFinishTrigger()
    {
        stateMachine?.CurrentState?.AnimationFinishTrigger();
        
        events?.InvokeAnimationTrigger(new PlayerAnimationEventData
        {
            TriggerName = "AnimationFinishTrigger",
            StateName = stateMachine?.CurrentState?.GetType().Name
        });
    }
}
