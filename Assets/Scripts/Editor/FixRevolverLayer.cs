using UnityEngine;
using UnityEditor;
using UnityEngine.SceneManagement;
using UnityEditor.SceneManagement;

public class FixRevolverLayer : EditorWindow
{
    [MenuItem("Tools/Fix Revolver Layer")]
    public static void Fix()
    {
        bool foundRevolver = false;
        
        for (int i = 0; i < SceneManager.sceneCount; i++)
        {
            Scene scene = SceneManager.GetSceneAt(i);
            
            GameObject[] allObjects = Object.FindObjectsByType<GameObject>(FindObjectsSortMode.None);
            
            foreach (GameObject go in allObjects)
            {
                if (go.name == "Revolver" && go.layer == LayerMask.NameToLayer("Default"))
                {
                    Debug.Log($"<color=yellow>[FIX] Found Revolver with incorrect layer: {LayerMask.LayerToName(go.layer)}</color>");
                    
                    int interactableLayer = LayerMask.NameToLayer("Interactable");
                    
                    if (interactableLayer == -1)
                    {
                        Debug.LogError("[FIX] Layer 'Interactable' does not exist!");
                        EditorUtility.DisplayDialog("Error", "Layer 'Interactable' not found in project settings.", "OK");
                        return;
                    }
                    
                    go.layer = interactableLayer;
                    
                    EditorUtility.SetDirty(go);
                    EditorSceneManager.MarkSceneDirty(scene);
                    
                    Debug.Log($"<color=green>[FIX] ✓ Revolver layer changed to: {LayerMask.LayerToName(interactableLayer)}</color>");
                    foundRevolver = true;
                }
            }
        }
        
        if (foundRevolver)
        {
            EditorUtility.DisplayDialog("Success", "Revolver layer has been fixed!\nIt's now on the 'Interactable' layer.", "OK");
        }
        else
        {
            EditorUtility.DisplayDialog("Info", "No Revolver found or it was already on the correct layer.", "OK");
        }
    }
}
