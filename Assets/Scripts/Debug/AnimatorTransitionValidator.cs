using UnityEngine;
#if UNITY_EDITOR
using UnityEditor.Animations;
#endif

public class AnimatorTransitionValidator : MonoBehaviour
{
    [SerializeField] private Animator animator;

    [ContextMenu("Validate Animator Transitions")]
    public void ValidateTransitions()
    {
        if (animator == null)
        {
            Debug.LogError("[VALIDATOR] Animator is NULL!");
            return;
        }

#if UNITY_EDITOR
        AnimatorController controller = animator.runtimeAnimatorController as AnimatorController;
        
        if (controller == null)
        {
            Debug.LogError("[VALIDATOR] AnimatorController is NULL!");
            return;
        }

        Debug.Log("<color=cyan>========== ANIMATOR TRANSITION VALIDATOR ==========</color>");
        
        foreach (var layer in controller.layers)
        {
            Debug.Log($"<color=yellow>LAYER: {layer.name}</color>");
            
            AnimatorStateMachine stateMachine = layer.stateMachine;
            
            foreach (var state in stateMachine.states)
            {
                string stateName = state.state.name;
                int stateHash = Animator.StringToHash(stateName);
                bool hasMotion = state.state.motion != null;
                string motionName = hasMotion ? state.state.motion.name : "NULL";
                
                Debug.Log($"  <color=cyan>STATE: {stateName}</color> | Hash: {stateHash} | Motion: {motionName}");
                
                var transitions = state.state.transitions;
                
                if (transitions.Length == 0)
                {
                    Debug.LogWarning($"    <color=red>⚠️ NO TRANSITIONS FROM {stateName}!</color>");
                }
                else
                {
                    foreach (var transition in transitions)
                    {
                        string destName = transition.destinationState != null ? transition.destinationState.name : "NULL";
                        bool hasExitTime = transition.hasExitTime;
                        float exitTime = transition.exitTime;
                        float duration = transition.duration;
                        
                        Debug.Log($"    → {destName} | HasExitTime: {hasExitTime} | ExitTime: {exitTime} | Duration: {duration}");
                        
                        foreach (var condition in transition.conditions)
                        {
                            Debug.Log($"      Condition: {condition.parameter} {condition.mode} {condition.threshold}");
                        }
                        
                        if (transition.conditions.Length == 0 && !hasExitTime)
                        {
                            Debug.LogWarning($"      <color=red>⚠️ Transition has NO conditions and NO exit time!</color>");
                        }
                    }
                }
            }
            
            Debug.Log($"  <color=magenta>ANY STATE TRANSITIONS:</color>");
            foreach (var transition in stateMachine.anyStateTransitions)
            {
                string destName = transition.destinationState != null ? transition.destinationState.name : "NULL";
                Debug.Log($"    Any State → {destName}");
                foreach (var condition in transition.conditions)
                {
                    Debug.Log($"      Condition: {condition.parameter} {condition.mode} {condition.threshold}");
                }
            }
        }
        
        Debug.Log("<color=cyan>===================================================</color>");
        
        CheckCriticalTransitions(controller);
#else
        Debug.LogWarning("[VALIDATOR] This script only works in the Editor!");
#endif
    }

#if UNITY_EDITOR
    private void CheckCriticalTransitions(AnimatorController controller)
    {
        Debug.Log("<color=yellow>========== CHECKING CRITICAL TRANSITIONS ==========</color>");
        
        bool foundAimToFire = false;
        bool foundFireToAim = false;
        bool foundAimToReload = false;
        bool foundReloadToAim = false;
        
        foreach (var layer in controller.layers)
        {
            foreach (var state in layer.stateMachine.states)
            {
                if (state.state.name == "Aim")
                {
                    foreach (var transition in state.state.transitions)
                    {
                        if (transition.destinationState != null)
                        {
                            if (transition.destinationState.name == "Fire")
                            {
                                foundAimToFire = true;
                                Debug.Log("<color=green>✓ Found Aim → Fire transition</color>");
                            }
                            if (transition.destinationState.name == "Reload")
                            {
                                foundAimToReload = true;
                                Debug.Log("<color=green>✓ Found Aim → Reload transition</color>");
                            }
                        }
                    }
                }
                
                if (state.state.name == "Fire")
                {
                    foreach (var transition in state.state.transitions)
                    {
                        if (transition.destinationState != null && transition.destinationState.name == "Aim")
                        {
                            foundFireToAim = true;
                            Debug.Log("<color=green>✓ Found Fire → Aim transition</color>");
                        }
                    }
                }
                
                if (state.state.name == "Reload")
                {
                    foreach (var transition in state.state.transitions)
                    {
                        if (transition.destinationState != null && transition.destinationState.name == "Aim")
                        {
                            foundReloadToAim = true;
                            Debug.Log("<color=green>✓ Found Reload → Aim transition</color>");
                        }
                    }
                }
            }
        }
        
        if (!foundAimToFire)
            Debug.LogError("<color=red>❌ MISSING: Aim → Fire transition</color>");
        if (!foundFireToAim)
            Debug.LogError("<color=red>❌ MISSING: Fire → Aim transition</color>");
        if (!foundAimToReload)
            Debug.LogError("<color=red>❌ MISSING: Aim → Reload transition</color>");
        if (!foundReloadToAim)
            Debug.LogError("<color=red>❌ MISSING: Reload → Aim transition</color>");
        
        Debug.Log("<color=yellow>===================================================</color>");
    }
#endif

    private void OnValidate()
    {
        if (animator == null)
        {
            animator = GetComponent<Animator>();
        }
    }
}
