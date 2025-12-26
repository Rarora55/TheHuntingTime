using UnityEngine;
using UnityEditor;

public class QuickFixRevolver
{
    [MenuItem("GameObject/Fix Revolver Layer (Selected)", true)]
    private static bool ValidateFixSelected()
    {
        return Selection.activeGameObject != null && Selection.activeGameObject.name == "Revolver";
    }
    
    [MenuItem("GameObject/Fix Revolver Layer (Selected)")]
    private static void FixSelected()
    {
        GameObject revolver = Selection.activeGameObject;
        
        if (revolver == null || revolver.name != "Revolver")
        {
            EditorUtility.DisplayDialog("Error", "Please select the Revolver GameObject first.", "OK");
            return;
        }
        
        int interactableLayer = LayerMask.NameToLayer("Interactable");
        
        if (interactableLayer == -1)
        {
            EditorUtility.DisplayDialog("Error", "Layer 'Interactable' does not exist in project settings.", "OK");
            return;
        }
        
        string oldLayerName = LayerMask.LayerToName(revolver.layer);
        Debug.Log($"<color=yellow>[FIX] Current Layer: {revolver.layer} ({oldLayerName})</color>");
        
        revolver.layer = interactableLayer;
        
        EditorUtility.SetDirty(revolver);
        UnityEditor.SceneManagement.EditorSceneManager.MarkSceneDirty(revolver.scene);
        
        string newLayerName = LayerMask.LayerToName(revolver.layer);
        Debug.Log($"<color=green>[FIX] ✓ New Layer: {revolver.layer} ({newLayerName})</color>");
        
        EditorUtility.DisplayDialog("Success", 
            $"Revolver layer changed!\n\nFrom: {oldLayerName} (index {0})\nTo: {newLayerName} (index {interactableLayer})", 
            "OK");
    }
}
