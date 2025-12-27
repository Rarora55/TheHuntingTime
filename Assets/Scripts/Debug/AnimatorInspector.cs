using UnityEngine;

#if UNITY_EDITOR
using UnityEditor;
using UnityEditor.Animations;
#endif

public class AnimatorInspector : MonoBehaviour
{
    private Animator animator;
    private bool hasLogged = false;
    
    private void Awake()
    {
        animator = GetComponent<Animator>();
    }
    
    private void Start()
    {
#if UNITY_EDITOR
        LogAnimatorInfo();
#endif
    }
    
#if UNITY_EDITOR
    private void LogAnimatorInfo()
    {
        if (hasLogged) return;
        hasLogged = true;
        
        if (animator == null || animator.runtimeAnimatorController == null)
        {
            Debug.LogError("[ANIMATOR] No animator or controller found!");
            return;
        }
        
        var controller = animator.runtimeAnimatorController as AnimatorController;
        if (controller == null)
        {
            Debug.LogError("[ANIMATOR] Controller is not an AnimatorController!");
            return;
        }
        
        Debug.Log("<color=cyan>========== ANIMATOR INFO ==========</color>");
        
        foreach (var layer in controller.layers)
        {
            Debug.Log($"<color=yellow>[LAYER] {layer.name}</color>");
            
            foreach (var state in layer.stateMachine.states)
            {
                if (state.state.name == "Aim" || state.state.name == "Fire" || state.state.name == "Reload")
                {
                    Debug.Log($"  <color=green>[STATE] {state.state.name}</color>");
                    Debug.Log($"    Motion: {(state.state.motion != null ? state.state.motion.name : "NULL")}");
                    Debug.Log($"    Transitions: {state.state.transitions.Length}");
                    
                    foreach (var transition in state.state.transitions)
                    {
                        string destName = transition.destinationState != null ? transition.destinationState.name : "NULL";
                        Debug.Log($"      → {destName} | HasExitTime: {transition.hasExitTime} | Duration: {transition.duration}");
                        
                        foreach (var condition in transition.conditions)
                        {
                            Debug.Log($"        Condition: {condition.parameter} {condition.mode} {condition.threshold}");
                        }
                    }
                }
            }
            
            Debug.Log($"  <color=magenta>[ANY STATE] Transitions: {layer.stateMachine.anyStateTransitions.Length}</color>");
            foreach (var transition in layer.stateMachine.anyStateTransitions)
            {
                string destName = transition.destinationState != null ? transition.destinationState.name : "NULL";
                if (destName == "Aim" || destName == "Fire" || destName == "Reload")
                {
                    Debug.Log($"    → {destName} | HasExitTime: {transition.hasExitTime}");
                    
                    foreach (var condition in transition.conditions)
                    {
                        Debug.Log($"      Condition: {condition.parameter} {condition.mode} {condition.threshold}");
                    }
                }
            }
        }
        
        Debug.Log("<color=cyan>===================================</color>");
    }
#endif
}
