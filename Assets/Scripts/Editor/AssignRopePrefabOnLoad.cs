using UnityEngine;
using UnityEditor;
using UnityEditor.SceneManagement;
using TheHunt.Interaction;

namespace TheHuntEditor
{
    [InitializeOnLoad]
    public static class AssignRopePrefabOnLoad
    {
        private const string ROPE_PREFAB_PATH = "Assets/Prefabs/ObjectsForTests/RopeClimbable.prefab";
        private const string SCENE_PATH = "Assets/Scenes/Character.unity";
        
        static AssignRopePrefabOnLoad()
        {
            EditorSceneManager.sceneOpened += OnSceneOpened;
        }
        
        private static void OnSceneOpened(UnityEngine.SceneManagement.Scene scene, OpenSceneMode mode)
        {
            if (scene.path != SCENE_PATH)
                return;
            
            AssignRopePrefab();
        }
        
        [MenuItem("Tools/TheHunt/Assign Rope Prefab to RopeAnchors")]
        private static void AssignRopePrefabMenu()
        {
            AssignRopePrefab();
        }
        
        private static void AssignRopePrefab()
        {
            GameObject ropePrefab = AssetDatabase.LoadAssetAtPath<GameObject>(ROPE_PREFAB_PATH);
            
            if (ropePrefab == null)
            {
                Debug.LogError($"[ROPE ANCHOR FIX] Could not load rope prefab from: {ROPE_PREFAB_PATH}");
                return;
            }
            
            RopeAnchorPassiveItem[] anchors = Object.FindObjectsByType<RopeAnchorPassiveItem>(FindObjectsSortMode.None);
            
            int fixedCount = 0;
            foreach (RopeAnchorPassiveItem anchor in anchors)
            {
                var serializedObject = new SerializedObject(anchor);
                var ropePrefabProp = serializedObject.FindProperty("ropePrefab");
                
                if (ropePrefabProp.objectReferenceValue == null)
                {
                    ropePrefabProp.objectReferenceValue = ropePrefab;
                    serializedObject.ApplyModifiedProperties();
                    
                    fixedCount++;
                    Debug.Log($"<color=green>[ROPE ANCHOR FIX] ✓ Assigned RopeClimbable prefab to: {anchor.gameObject.name}</color>");
                }
            }
            
            if (fixedCount > 0)
            {
                EditorSceneManager.MarkSceneDirty(EditorSceneManager.GetActiveScene());
                Debug.Log($"<color=green>[ROPE ANCHOR FIX] ✓ Fixed {fixedCount} RopeAnchor(s)!</color>");
            }
            else
            {
                Debug.Log("<color=yellow>[ROPE ANCHOR FIX] All RopeAnchors already have ropePrefab assigned</color>");
            }
        }
    }
}
