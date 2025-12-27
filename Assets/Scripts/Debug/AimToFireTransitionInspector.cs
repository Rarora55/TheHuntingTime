using UnityEngine;

#if UNITY_EDITOR
using UnityEditor;
using UnityEditor.Animations;

public class AimToFireTransitionInspector : MonoBehaviour
{
    [ContextMenu("Inspect Aim → Fire Transition")]
    public void InspectAimToFire()
    {
        var animator = GetComponent<Animator>();
        if (animator == null || animator.runtimeAnimatorController == null)
        {
            Debug.LogError("<color=red>No Animator or Controller found!</color>");
            return;
        }

        var controller = animator.runtimeAnimatorController as AnimatorController;
        if (controller == null)
        {
            Debug.LogError("<color=red>Not an AnimatorController!</color>");
            return;
        }

        Debug.Log("<color=cyan>========== AIM → FIRE TRANSITION ==========</color>");

        foreach (var layer in controller.layers)
        {
            var stateMachine = layer.stateMachine;
            
            foreach (var state in stateMachine.states)
            {
                if (state.state.name == "Aim")
                {
                    Debug.Log($"<color=yellow>Found Aim State in layer: {layer.name}</color>");
                    Debug.Log($"  Transitions from Aim: {state.state.transitions.Length}");
                    
                    foreach (var transition in state.state.transitions)
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
                            
                            if (transition.destinationState.name == "Fire")
                            {
                                Debug.LogWarning("<color=green>✓ FOUND AIM → FIRE TRANSITION</color>");
                                
                                if (transition.mute)
                                {
                                    Debug.LogError("<color=red>❌ PROBLEM: Mute = TRUE (transition is disabled!)</color>");
                                }
                                
                                if (transition.hasExitTime)
                                {
                                    Debug.LogError("<color=red>❌ PROBLEM: Has Exit Time = TRUE (should be FALSE)</color>");
                                }
                                
                                if (transition.duration > 0.1f)
                                {
                                    Debug.LogWarning($"<color=orange>⚠️ Duration = {transition.duration} (consider making it 0 for instant transition)</color>");
                                }
                                
                                bool hasFireCondition = false;
                                foreach (var condition in transition.conditions)
                                {
                                    if (condition.parameter == "fire" && condition.mode == AnimatorConditionMode.If)
                                    {
                                        hasFireCondition = true;
                                    }
                                }
                                
                                if (!hasFireCondition)
                                {
                                    Debug.LogError("<color=red>❌ PROBLEM: No 'fire = true' condition found!</color>");
                                }
                                else
                                {
                                    Debug.Log("<color=green>✓ Has 'fire = true' condition</color>");
                                }
                            }
                        }
                    }
                }
            }
        }
        
        Debug.Log("<color=cyan>===========================================</color>");
    }
}
#endif
