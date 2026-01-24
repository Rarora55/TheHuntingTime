using UnityEngine;
using TheHunt.Interaction;
using TheHunt.Environment;

[RequireComponent(typeof(RopeAnchorPoint))]
public class RopeAnchorInteraction : InteractableObject
{
    private RopeAnchorPoint anchorPoint;
    
    void Awake()
    {
        anchorPoint = GetComponent<RopeAnchorPoint>();
        
        if (anchorPoint == null)
        {
            Debug.LogError($"<color=red>[ROPE ANCHOR INTERACTION] RopeAnchorPoint component missing on {gameObject.name}</color>");
        }
        else
        {
            Debug.Log($"<color=green>[ROPE ANCHOR INTERACTION] Initialized on {gameObject.name}</color>");
        }
        
        interactionPrompt = "Deploy Rope";
    }
    
    protected override void OnInteract(GameObject interactor)
    {
        Debug.Log($"<color=magenta>[ROPE ANCHOR INTERACTION] OnInteract called!</color>");
        
        if (anchorPoint == null)
        {
            Debug.LogError($"<color=red>[ROPE ANCHOR INTERACTION] anchorPoint is null!</color>");
            return;
        }
        
        global::Player player = interactor.GetComponent<global::Player>();
        if (player == null)
        {
            Debug.LogWarning($"<color=yellow>[ROPE ANCHOR] Interactor is not a player</color>");
            return;
        }
        
        Debug.Log($"<color=cyan>[ROPE ANCHOR] Player detected, checking if rope is deployed...</color>");
        
        if (anchorPoint.IsRopeDeployed)
        {
            ShowMessage("Rope already deployed here");
            return;
        }
        
        if (!anchorPoint.CanDeployRope())
        {
            ShowMessage("Cannot deploy rope here");
            return;
        }
        
        Debug.Log($"<color=green>[ROPE ANCHOR] Attempting to deploy rope...</color>");
        
        bool deployed = anchorPoint.DeployRope(player);
        
        if (deployed)
        {
            ShowMessage("Rope deployed successfully");
            interactionPrompt = "Rope deployed";
            SetInteractable(false);
        }
        else
        {
            ShowMessage("Need rope in inventory");
        }
    }
    
    void ShowMessage(string message)
    {
        Debug.Log($"<color=cyan>[ROPE ANCHOR] {message}</color>");
    }
}
