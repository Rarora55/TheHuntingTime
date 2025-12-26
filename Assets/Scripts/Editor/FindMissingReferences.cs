using UnityEngine;
using UnityEditor;
using System.Collections.Generic;

public class FindMissingReferences : EditorWindow
{
    private static List<string> foundIssues = new List<string>();
    
    [MenuItem("Tools/Find Missing References")]
    public static void ShowWindow()
    {
        GetWindow<FindMissingReferences>("Find Missing References");
    }
    
    private void OnGUI()
    {
        GUILayout.Label("Find Missing References in Project", EditorStyles.boldLabel);
        GUILayout.Space(10);
        
        if (GUILayout.Button("Scan All Prefabs", GUILayout.Height(40)))
        {
            ScanAllPrefabs();
        }
        
        GUILayout.Space(10);
        
        if (GUILayout.Button("Scan Current Scene", GUILayout.Height(40)))
        {
            ScanCurrentScene();
        }
        
        GUILayout.Space(20);
        
        if (foundIssues.Count > 0)
        {
            GUILayout.Label($"Found {foundIssues.Count} issues:", EditorStyles.boldLabel);
            
            foreach (string issue in foundIssues)
            {
                EditorGUILayout.HelpBox(issue, MessageType.Warning);
            }
        }
        else
        {
            EditorGUILayout.HelpBox("No issues found or scan not run yet.", MessageType.Info);
        }
    }
    
    private static void ScanAllPrefabs()
    {
        foundIssues.Clear();
        
        string[] guids = AssetDatabase.FindAssets("t:Prefab");
        int count = 0;
        
        foreach (string guid in guids)
        {
            string path = AssetDatabase.GUIDToAssetPath(guid);
            GameObject prefab = AssetDatabase.LoadAssetAtPath<GameObject>(path);
            
            if (prefab != null)
            {
                CheckGameObject(prefab, path);
                count++;
            }
            
            EditorUtility.DisplayProgressBar("Scanning Prefabs", path, (float)count / guids.Length);
        }
        
        EditorUtility.ClearProgressBar();
        
        if (foundIssues.Count > 0)
        {
            Debug.LogWarning($"<color=yellow>[SCAN] Found {foundIssues.Count} issues in prefabs</color>");
        }
        else
        {
            Debug.Log("<color=green>[SCAN] ✓ No issues found in prefabs</color>");
        }
    }
    
    private static void ScanCurrentScene()
    {
        foundIssues.Clear();
        
        GameObject[] allObjects = Object.FindObjectsByType<GameObject>(FindObjectsSortMode.None);
        
        foreach (GameObject go in allObjects)
        {
            CheckGameObject(go, go.name);
        }
        
        if (foundIssues.Count > 0)
        {
            Debug.LogWarning($"<color=yellow>[SCAN] Found {foundIssues.Count} issues in scene</color>");
        }
        else
        {
            Debug.Log("<color=green>[SCAN] ✓ No issues found in scene</color>");
        }
    }
    
    private static void CheckGameObject(GameObject go, string context)
    {
        Component[] components = go.GetComponents<Component>();
        
        for (int i = 0; i < components.Length; i++)
        {
            if (components[i] == null)
            {
                string issue = $"Missing Script on '{go.name}' in {context}";
                foundIssues.Add(issue);
                Debug.LogWarning($"<color=red>[MISSING] {issue}</color>", go);
            }
            else
            {
                SerializedObject so = new SerializedObject(components[i]);
                SerializedProperty sp = so.GetIterator();
                
                while (sp.NextVisible(true))
                {
                    if (sp.propertyType == SerializedPropertyType.ObjectReference)
                    {
                        if (sp.objectReferenceValue == null && sp.objectReferenceInstanceIDValue != 0)
                        {
                            string issue = $"Missing Reference in '{go.name}.{components[i].GetType().Name}.{sp.name}' ({context})";
                            foundIssues.Add(issue);
                            Debug.LogWarning($"<color=orange>[NULL REF] {issue}</color>", go);
                        }
                    }
                }
            }
        }
        
        foreach (Transform child in go.transform)
        {
            CheckGameObject(child.gameObject, context);
        }
    }
}
