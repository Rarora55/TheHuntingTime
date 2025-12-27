using UnityEngine;
using UnityEditor;
using TheHunt.Inventory;

namespace TheHunt.EditorTools
{
    public class UpdateItemDataMaxStackSize : EditorWindow
    {
        [MenuItem("Tools/Inventory/Update All Items - Set Default MaxStackSize")]
        public static void UpdateAllItems()
        {
            string[] guids = AssetDatabase.FindAssets("t:ItemData");
            int updatedCount = 0;

            foreach (string guid in guids)
            {
                string path = AssetDatabase.GUIDToAssetPath(guid);
                ItemData itemData = AssetDatabase.LoadAssetAtPath<ItemData>(path);

                if (itemData != null)
                {
                    SerializedObject serializedItem = new SerializedObject(itemData);
                    SerializedProperty maxStackSizeProp = serializedItem.FindProperty("maxStackSize");

                    if (maxStackSizeProp != null && maxStackSizeProp.intValue == 0)
                    {
                        maxStackSizeProp.intValue = 6;
                        serializedItem.ApplyModifiedProperties();
                        updatedCount++;
                        Debug.Log($"<color=green>[UPDATE] Set MaxStackSize to 6 for: {itemData.ItemName}</color>");
                    }
                }
            }

            AssetDatabase.SaveAssets();
            Debug.Log($"<color=cyan>[UPDATE] Updated {updatedCount} items with default MaxStackSize = 6</color>");
        }
    }
}
