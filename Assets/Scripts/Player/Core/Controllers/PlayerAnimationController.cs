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
