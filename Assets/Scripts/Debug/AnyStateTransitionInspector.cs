using UnityEngine;

#if UNITY_EDITOR
using UnityEditor;
using UnityEditor.Animations;

public class AnyStateTransitionInspector : MonoBehaviour
{
    [ContextMenu("Inspect Any State → Aim Transition")]
    public void InspectAnyStateToAim()
    {
        var animator = GetComponent<Animator>();
        if (animator == null || animator.runtimeAnimatorController == null)
        {
            Debug.LogError("<color=red>[ANY STATE INSPECTOR] No Animator or Controller found!</color>");
            return;
        }

        var controller = animator.runtimeAnimatorController as AnimatorController;
        if (controller == null)
        {
            Debug.LogError("<color=red>[ANY STATE INSPECTOR] Not an AnimatorController!</color>");
            return;
        }

        Debug.Log("<color=cyan>========== ANY STATE TRANSITIONS ==========</color>");

        foreach (var layer in controller.layers)
        {
            Debug.Log($"<color=yellow>Layer: {layer.name}</color>");
            
            var stateMachine = layer.stateMachine;
            var anyStateTransitions = stateMachine.anyStateTransitions;
            
            Debug.Log($"  Any State Transitions Count: {anyStateTransitions.Length}");
            
            foreach (var transition in anyStateTransitions)
            {
                if (transition.destinationState != null)
                {
                    Debug.Log($"\n<color=magenta>  → {transition.destinationState.name}</color>");
                    Debug.Log($"    Mute: {transition.mute}");
                    Debug.Log($"    Solo: {transition.solo}");
                    Debug.Log($"    Has Exit Time: {transition.hasExitTime}");
                    Debug.Log($"    Exit Time: {transition.exitTime}");
                    Debug.Log($"    Has Fixed Duration: {transition.hasFixedDuration}");
                    Debug.Log($"    Duration: {transition.duration}");
                    Debug.Log($"    Offset: {transition.offset}");
                    Debug.Log($"    Interruption Source: {transition.interruptionSource}");
                    Debug.Log($"    Can Transition To Self: {transition.canTransitionToSelf}");
                    
                    Debug.Log($"    Conditions ({transition.conditions.Length}):");
                    foreach (var condition in transition.conditions)
                    {
                        Debug.Log($"      '{condition.parameter}' {condition.mode} {condition.threshold}");
                    }
                    
                    if (transition.destinationState.name == "Aim")
                    {
                        Debug.LogWarning("<color=red>⚠️ FOUND ANY STATE → AIM TRANSITION</color>");
                        
                        if (transition.hasExitTime)
                        {
                            Debug.LogError("<color=red>❌ PROBLEM: Has Exit Time = TRUE (should be FALSE)</color>");
                        }
                        
                        if (transition.duration > 0.1f)
                        {
                            Debug.LogError($"<color=red>❌ PROBLEM: Duration = {transition.duration} (should be 0 or very small)</color>");
                        }
                        
                        if (transition.interruptionSource == TransitionInterruptionSource.None)
                        {
                            Debug.LogError("<color=red>❌ PROBLEM: Interruption Source = None (should be CurrentState or NextState)</color>");
                        }
                    }
                }
            }
        }
        
        Debug.Log("<color=cyan>===========================================</color>");
    }
}
#endif
