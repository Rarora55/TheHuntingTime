using UnityEngine;
using TheHunt.Interaction;

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
        
        interactionPrompt = "Deploy Rope";
    }
    
    protected override void OnInteract(GameObject interactor)
    {
        if (anchorPoint == null)
            return;
        
        global::Player player = interactor.GetComponent<global::Player>();
        if (player == null)
        {
            Debug.LogWarning($"<color=yellow>[ROPE ANCHOR] Interactor is not a player</color>");
            return;
        }
        
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
        
        bool deployed = anchorPoint.DeployRope(player);
        
        if (deployed)
        {
            ShowMessage("Rope deployed successfully");
            interactionPrompt = "Rope deployed";
            SetInteractable(false);
        }
        else
        {
            ShowMessage("Need rope equipped in secondary slot");
        }
    }
    
    void ShowMessage(string message)
    {
        Debug.Log($"<color=cyan>[ROPE ANCHOR] {message}</color>");
    }
}
