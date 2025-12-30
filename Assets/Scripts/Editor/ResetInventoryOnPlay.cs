using UnityEditor;
using UnityEngine;
using TheHunt.Inventory;

namespace TheHuntEditor
{
    [InitializeOnLoad]
    public static class ResetInventoryOnPlay
    {
        private const string MENU_NAME = "Tools/Auto Reset Inventory on Play";
        private const string PREF_KEY = "AutoResetInventoryOnPlay";
        
        private static bool IsEnabled
        {
            get => EditorPrefs.GetBool(PREF_KEY, true);
            set => EditorPrefs.SetBool(PREF_KEY, value);
        }
        
        static ResetInventoryOnPlay()
        {
            EditorApplication.playModeStateChanged += OnPlayModeStateChanged;
            EditorApplication.delayCall += () => Menu.SetChecked(MENU_NAME, IsEnabled);
        }
        
        [MenuItem(MENU_NAME)]
        private static void ToggleAutoReset()
        {
            IsEnabled = !IsEnabled;
            Menu.SetChecked(MENU_NAME, IsEnabled);
        }
        
        private static void OnPlayModeStateChanged(PlayModeStateChange state)
        {
            if (state == PlayModeStateChange.ExitingEditMode && IsEnabled)
            {
                ResetAllInventoryData();
            }
        }
        
        [MenuItem("Tools/Reset Inventory Now")]
        private static void ManualReset()
        {
            ResetAllInventoryData();
            Debug.Log("<color=green>[MANUAL RESET] Inventory data cleared!</color>");
        }
        
        private static void ResetAllInventoryData()
        {
            string[] guids = AssetDatabase.FindAssets("t:InventoryDataSO");
            
            if (guids.Length == 0)
            {
                Debug.LogWarning("<color=yellow>[RESET] No InventoryDataSO found in project</color>");
                return;
            }
            
            int resetCount = 0;
            foreach (string guid in guids)
            {
                string path = AssetDatabase.GUIDToAssetPath(guid);
                InventoryDataSO data = AssetDatabase.LoadAssetAtPath<InventoryDataSO>(path);
                
                if (data != null)
                {
                    data.ResetInventory();
                    EditorUtility.SetDirty(data);
                    resetCount++;
                }
            }
            
            AssetDatabase.SaveAssets();
        }
    }
}
