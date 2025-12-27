using UnityEngine;
using UnityEngine.InputSystem;

public class InputBindingInspector : MonoBehaviour
{
    private PlayerInput playerInput;

    private void Start()
    {
        playerInput = GetComponent<PlayerInput>();
        
        if (playerInput == null)
        {
            Debug.LogError("<color=red>[BINDING INSPECTOR] No PlayerInput component found!</color>");
            return;
        }

        InspectBindings();
    }

    [ContextMenu("Inspect Input Bindings")]
    public void InspectBindings()
    {
        if (playerInput == null)
            playerInput = GetComponent<PlayerInput>();

        if (playerInput == null || playerInput.actions == null)
        {
            Debug.LogError("<color=red>[BINDING INSPECTOR] PlayerInput or actions not found!</color>");
            return;
        }

        Debug.Log("<color=cyan>========== INPUT BINDINGS INSPECTION ==========</color>");

        foreach (var actionMap in playerInput.actions.actionMaps)
        {
            Debug.Log($"<color=yellow>Action Map: {actionMap.name}</color>");
            
            foreach (var action in actionMap.actions)
            {
                Debug.Log($"  <color=white>Action: {action.name} | Type: {action.type}</color>");
                
                foreach (var binding in action.bindings)
                {
                    if (!string.IsNullOrEmpty(binding.effectivePath))
                    {
                        Debug.Log($"    → Path: {binding.effectivePath} | Interactions: {binding.interactions} | Groups: {binding.groups}");
                    }
                }
            }
        }

        Debug.Log("<color=cyan>==============================================</color>");
        
        InspectSpecificActions();
    }

    private void InspectSpecificActions()
    {
        Debug.Log("<color=magenta>========== SPECIFIC ACTIONS ==========</color>");
        
        var aimAction = playerInput.actions.FindAction("Aim");
        var fireAction = playerInput.actions.FindAction("Fire");
        var reloadAction = playerInput.actions.FindAction("Reload");

        if (aimAction != null)
        {
            Debug.Log($"<color=green>AIM ACTION:</color>");
            Debug.Log($"  Type: {aimAction.type}");
            Debug.Log($"  Bindings: {aimAction.bindings.Count}");
            foreach (var binding in aimAction.bindings)
            {
                Debug.Log($"    Path: {binding.effectivePath} | Interactions: '{binding.interactions}'");
            }
        }

        if (fireAction != null)
        {
            Debug.Log($"<color=green>FIRE ACTION:</color>");
            Debug.Log($"  Type: {fireAction.type}");
            Debug.Log($"  Bindings: {fireAction.bindings.Count}");
            foreach (var binding in fireAction.bindings)
            {
                Debug.Log($"    Path: {binding.effectivePath} | Interactions: '{binding.interactions}'");
            }
        }

        if (reloadAction != null)
        {
            Debug.Log($"<color=green>RELOAD ACTION:</color>");
            Debug.Log($"  Type: {reloadAction.type}");
            Debug.Log($"  Bindings: {reloadAction.bindings.Count}");
            foreach (var binding in reloadAction.bindings)
            {
                Debug.Log($"    Path: {binding.effectivePath} | Interactions: '{binding.interactions}'");
            }
        }

        Debug.Log("<color=magenta>======================================</color>");
    }
}
