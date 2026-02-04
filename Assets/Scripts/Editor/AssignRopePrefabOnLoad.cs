using UnityEngine;
using UnityEditor;
using UnityEditor.SceneManagement;
using TheHunt.Interaction;
using System;

namespace TheHuntEditor
{
    [InitializeOnLoad]
    public static class AssignRopePrefabOnLoad
    {
        private const string ROPE_PREFAB_PATH = "Assets/Prefabs/ObjectsForTests/RopeClimbable.prefab";
        private const string SCENE_PATH = "Assets/Scenes/Character.unity";
        
        private const string AUTO_ASSIGN_ENABLED_KEY = "RopeAutoAssign_Enabled";
        private const string LAST_RUN_KEY = "RopeAutoAssign_LastRun";
        private const string MENU_PATH = "Tools/TheHunt/Auto-Assign Rope Prefabs on Scene Open";
        
        private const double COOLDOWN_HOURS = 1.0;
        
        static AssignRopePrefabOnLoad()
        {
            EditorSceneManager.sceneOpened += OnSceneOpened;
            EditorApplication.delayCall += () => Menu.SetChecked(MENU_PATH, IsAutoAssignEnabled());
        }
        
        [MenuItem(MENU_PATH)]
        private static void ToggleAutoAssign()
        {
            bool currentValue = IsAutoAssignEnabled();
            SetAutoAssignEnabled(!currentValue);
            Menu.SetChecked(MENU_PATH, !currentValue);
            
            string status = !currentValue ? "ENABLED" : "DISABLED";
            Debug.Log($"<color=cyan>[ROPE AUTO-ASSIGN] Auto-assignment {status}</color>");
        }
        
        private static bool IsAutoAssignEnabled()
        {
            return EditorPrefs.GetBool(AUTO_ASSIGN_ENABLED_KEY, false);
        }
        
        private static void SetAutoAssignEnabled(bool enabled)
        {
            EditorPrefs.SetBool(AUTO_ASSIGN_ENABLED_KEY, enabled);
        }
        
        private static void OnSceneOpened(UnityEngine.SceneManagement.Scene scene, OpenSceneMode mode)
        {
            if (!IsAutoAssignEnabled())
                return;
                
            if (scene.path != SCENE_PATH)
                return;
            
            if (!ShouldRunAutoAssign())
                return;
            
            AssignRopePrefab();
            UpdateLastRunTimestamp();
        }
        
        private static bool ShouldRunAutoAssign()
        {
            string lastRunStr = EditorPrefs.GetString(LAST_RUN_KEY, string.Empty);
            
            if (string.IsNullOrEmpty(lastRunStr))
                return true;
            
            if (DateTime.TryParse(lastRunStr, out DateTime lastRun))
            {
                double hoursSinceLastRun = (DateTime.Now - lastRun).TotalHours;
                
                if (hoursSinceLastRun < COOLDOWN_HOURS)
                {
                    return false;
                }
            }
            
            return true;
        }
        
        private static void UpdateLastRunTimestamp()
        {
            EditorPrefs.SetString(LAST_RUN_KEY, DateTime.Now.ToString("o"));
        }
        
        [MenuItem("Tools/TheHunt/Assign Rope Prefab to RopeAnchors (Manual)")]
        private static void AssignRopePrefabMenu()
        {
            AssignRopePrefab();
        }
        
        [MenuItem("Tools/TheHunt/Reset Rope Auto-Assign Cooldown")]
        private static void ResetCooldown()
        {
            EditorPrefs.DeleteKey(LAST_RUN_KEY);
            Debug.Log("<color=cyan>[ROPE AUTO-ASSIGN] Cooldown reset. Auto-assign will run next time the scene opens.</color>");
        }
        
        private static void AssignRopePrefab()
        {
            GameObject ropePrefab = AssetDatabase.LoadAssetAtPath<GameObject>(ROPE_PREFAB_PATH);
            
            if (ropePrefab == null)
            {
                Debug.LogError($"[ROPE ANCHOR FIX] Could not load rope prefab from: {ROPE_PREFAB_PATH}");
                return;
            }
            
            RopeAnchorPassiveItem[] anchors = UnityEngine.Object.FindObjectsByType<RopeAnchorPassiveItem>(FindObjectsSortMode.None);
            
            if (anchors.Length == 0)
            {
                return;
            }
            
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
                }
            }
            
            if (fixedCount > 0)
            {
                EditorSceneManager.MarkSceneDirty(EditorSceneManager.GetActiveScene());
                Debug.Log($"<color=green>[ROPE ANCHOR FIX] ✓ Auto-assigned RopeClimbable to {fixedCount} anchor(s)</color>");
            }
        }
    }
}
