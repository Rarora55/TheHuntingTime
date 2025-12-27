using UnityEngine;
#if UNITY_EDITOR
using UnityEditor.Animations;
#endif

public class AnimatorTransitionInspector : MonoBehaviour
{
    [SerializeField] private Animator animator;

    [ContextMenu("Inspect Aim -> Fire Transition")]
    public void InspectAimToFireTransition()
    {
#if UNITY_EDITOR
        if (animator == null || animator.runtimeAnimatorController == null)
        {
            Debug.LogError("[INSPECTOR] Animator or controller is NULL!");
            return;
        }

        AnimatorController controller = animator.runtimeAnimatorController as AnimatorController;
        
        Debug.Log("<color=cyan>========== INSPECTING AIM → FIRE TRANSITION ==========</color>");
        
        foreach (var layer in controller.layers)
        {
            foreach (var state in layer.stateMachine.states)
            {
                if (state.state.name == "Aim")
                {
                    Debug.Log($"<color=yellow>Found Aim state. Hash: {Animator.StringToHash("Aim")}</color>");
                    
                    foreach (var transition in state.state.transitions)
                    {
                        if (transition.destinationState != null && transition.destinationState.name == "Fire")
                        {
                            Debug.Log("<color=green>Found Aim → Fire transition:</color>");
                            Debug.Log($"  Mute: {transition.mute}");
                            Debug.Log($"  Solo: {transition.solo}");
                            Debug.Log($"  Has Exit Time: {transition.hasExitTime}");
                            Debug.Log($"  Exit Time: {transition.exitTime}");
                            Debug.Log($"  Has Fixed Duration: {transition.hasFixedDuration}");
                            Debug.Log($"  Duration: {transition.duration}");
                            Debug.Log($"  Offset: {transition.offset}");
                            Debug.Log($"  Interruption Source: {transition.interruptionSource}");
                            Debug.Log($"  Ordered Interruption: {transition.orderedInterruption}");
                            Debug.Log($"  Can Transition To Self: {transition.canTransitionToSelf}");
                            
                            Debug.Log($"<color=magenta>  Conditions ({transition.conditions.Length}):</color>");
                            
                            if (transition.conditions.Length == 0)
                            {
                                Debug.LogError("    <color=red>⚠️ NO CONDITIONS! This is the problem!</color>");
                            }
                            
                            foreach (var condition in transition.conditions)
                            {
                                Debug.Log($"    Parameter: '{condition.parameter}'");
                                Debug.Log($"    Mode: {condition.mode}");
                                Debug.Log($"    Threshold: {condition.threshold}");
                                
                                if (condition.mode == AnimatorConditionMode.If)
                                {
                                    Debug.Log("      <color=green>✓ Mode is 'If' (requires true)</color>");
                                }
                                else if (condition.mode == AnimatorConditionMode.IfNot)
                                {
                                    Debug.LogError("      <color=red>✗ Mode is 'IfNot' (requires false) - THIS IS WRONG!</color>");
                                }
                            }
                            
                            if (transition.mute)
                            {
                                Debug.LogError("    <color=red>⚠️ TRANSITION IS MUTED!</color>");
                            }
                        }
                    }
                }
            }
        }
        
        Debug.Log("<color=cyan>======================================================</color>");
#else
        Debug.LogWarning("[INSPECTOR] This only works in Editor!");
#endif
    }

    private void OnValidate()
    {
        if (animator == null)
        {
            animator = GetComponent<Animator>();
        }
    }
}
