using UnityEngine;
using UnityEditor;
using UnityEngine.SceneManagement;
using UnityEditor.SceneManagement;
using TheHunt.UI;

public class FixDialogServiceMode : EditorWindow
{
    [MenuItem("Tools/Fix Dialog Service Mode")]
    public static void Fix()
    {
        bool foundIssue = false;
        
        for (int i = 0; i < SceneManager.sceneCount; i++)
        {
            Scene scene = SceneManager.GetSceneAt(i);
            
            DialogService[] services = Object.FindObjectsByType<DialogService>(FindObjectsSortMode.None);
            
            foreach (DialogService service in services)
            {
                SerializedObject so = new SerializedObject(service);
                SerializedProperty usePrefabProp = so.FindProperty("usePrefab");
                SerializedProperty simpleDialogProp = so.FindProperty("simpleDialog");
                SerializedProperty confirmPrefabProp = so.FindProperty("confirmationDialogPrefab");
                
                if (!usePrefabProp.boolValue && simpleDialogProp.objectReferenceValue == null && confirmPrefabProp.objectReferenceValue != null)
                {
                    Debug.Log($"<color=yellow>[FIX] Found DialogService in CODE mode but with NULL simpleDialog</color>");
                    Debug.Log($"<color=cyan>[FIX] Switching to PREFAB mode...</color>");
                    
                    usePrefabProp.boolValue = true;
                    so.ApplyModifiedProperties();
                    
                    EditorUtility.SetDirty(service);
                    EditorSceneManager.MarkSceneDirty(scene);
                    
                    Debug.Log($"<color=green>[FIX] ✓ DialogService switched to PREFAB mode on '{service.gameObject.name}'</color>");
                    foundIssue = true;
                }
            }
        }
        
        if (foundIssue)
        {
            EditorUtility.DisplayDialog("Success", "DialogService has been corrected! It's now using PREFAB mode.", "OK");
        }
        else
        {
            EditorUtility.DisplayDialog("Info", "No DialogService needed fixing.", "OK");
        }
    }
}

