using UnityEngine;
using UnityEditor;

namespace TheHunt.Inventory
{
    [CustomEditor(typeof(WorldItem))]
    public class WorldItemEditor : Editor
    {
        private SerializedProperty itemDataProp;
        private SerializedProperty quantityProp;
        private SerializedProperty useItemIconProp;
        private SerializedProperty customSpriteProp;
        private SerializedProperty spriteSizeProp;
        private SerializedProperty interactionRadiusProp;
        private SerializedProperty pickupMessageProp;
        private SerializedProperty autoPickupProp;
        private SerializedProperty enableFloatingProp;
        private SerializedProperty floatSpeedProp;
        private SerializedProperty floatHeightProp;
        private SerializedProperty enableRotationProp;
        private SerializedProperty rotationSpeedProp;

        private void OnEnable()
        {
            itemDataProp = serializedObject.FindProperty("itemData");
            quantityProp = serializedObject.FindProperty("quantity");
            useItemIconProp = serializedObject.FindProperty("useItemIcon");
            customSpriteProp = serializedObject.FindProperty("customSprite");
            spriteSizeProp = serializedObject.FindProperty("spriteSize");
            interactionRadiusProp = serializedObject.FindProperty("interactionRadius");
            pickupMessageProp = serializedObject.FindProperty("pickupMessage");
            autoPickupProp = serializedObject.FindProperty("autoPickup");
            enableFloatingProp = serializedObject.FindProperty("enableFloating");
            floatSpeedProp = serializedObject.FindProperty("floatSpeed");
            floatHeightProp = serializedObject.FindProperty("floatHeight");
            enableRotationProp = serializedObject.FindProperty("enableRotation");
            rotationSpeedProp = serializedObject.FindProperty("rotationSpeed");
        }

        public override void OnInspectorGUI()
        {
            serializedObject.Update();

            WorldItem worldItem = (WorldItem)target;

            EditorGUILayout.Space(5);
            DrawHeader("Item Configuration");
            EditorGUILayout.PropertyField(itemDataProp);
            
            if (itemDataProp.objectReferenceValue != null)
            {
                ItemData data = itemDataProp.objectReferenceValue as ItemData;
                EditorGUILayout.HelpBox($"Item: {data.ItemName}\nType: {data.ItemType}", MessageType.Info);
                
                if (data.IsStackable)
                {
                    EditorGUILayout.PropertyField(quantityProp);
                }
                else
                {
                    quantityProp.intValue = 1;
                    GUI.enabled = false;
                    EditorGUILayout.PropertyField(quantityProp);
                    GUI.enabled = true;
                }
            }
            else
            {
                EditorGUILayout.HelpBox("Assign an ItemData to configure this pickup.", MessageType.Warning);
            }

            EditorGUILayout.Space(10);
            DrawHeader("Visual Settings");
            EditorGUILayout.PropertyField(useItemIconProp);
            
            if (!useItemIconProp.boolValue)
            {
                EditorGUILayout.PropertyField(customSpriteProp);
            }
            
            EditorGUILayout.PropertyField(spriteSizeProp);
            
            if (GUILayout.Button("Apply Visual Settings"))
            {
                worldItem.SendMessage("RefreshVisuals", SendMessageOptions.DontRequireReceiver);
            }

            EditorGUILayout.Space(10);
            DrawHeader("Interaction Settings");
            EditorGUILayout.PropertyField(interactionRadiusProp);
            EditorGUILayout.PropertyField(pickupMessageProp);
            EditorGUILayout.PropertyField(autoPickupProp);
            
            if (autoPickupProp.boolValue)
            {
                EditorGUILayout.HelpBox("Player will pick up automatically on contact.", MessageType.Info);
            }

            EditorGUILayout.Space(10);
            DrawHeader("Animation Settings");
            
            EditorGUILayout.PropertyField(enableFloatingProp);
            if (enableFloatingProp.boolValue)
            {
                EditorGUI.indentLevel++;
                EditorGUILayout.PropertyField(floatSpeedProp);
                EditorGUILayout.PropertyField(floatHeightProp);
                EditorGUI.indentLevel--;
            }
            
            EditorGUILayout.PropertyField(enableRotationProp);
            if (enableRotationProp.boolValue)
            {
                EditorGUI.indentLevel++;
                EditorGUILayout.PropertyField(rotationSpeedProp);
                EditorGUI.indentLevel--;
            }

            EditorGUILayout.Space(10);
            DrawQuickSetup();

            serializedObject.ApplyModifiedProperties();
        }

        private void DrawHeader(string title)
        {
            GUIStyle headerStyle = new GUIStyle(EditorStyles.boldLabel);
            headerStyle.fontSize = 12;
            EditorGUILayout.LabelField(title, headerStyle);
            EditorGUILayout.Space(3);
        }

        private void DrawQuickSetup()
        {
            DrawHeader("Quick Setup Presets");
            
            EditorGUILayout.BeginHorizontal();
            
            if (GUILayout.Button("Weapon"))
            {
                spriteSizeProp.vector2Value = new Vector2(0.8f, 0.8f);
                enableFloatingProp.boolValue = true;
                floatSpeedProp.floatValue = 1f;
                floatHeightProp.floatValue = 0.2f;
                enableRotationProp.boolValue = false;
                interactionRadiusProp.floatValue = 2f;
            }
            
            if (GUILayout.Button("Consumable"))
            {
                spriteSizeProp.vector2Value = new Vector2(0.5f, 0.5f);
                enableFloatingProp.boolValue = true;
                floatSpeedProp.floatValue = 1.5f;
                floatHeightProp.floatValue = 0.15f;
                enableRotationProp.boolValue = true;
                rotationSpeedProp.floatValue = 30f;
                interactionRadiusProp.floatValue = 1.5f;
            }
            
            if (GUILayout.Button("Ammo"))
            {
                spriteSizeProp.vector2Value = new Vector2(0.6f, 0.6f);
                enableFloatingProp.boolValue = true;
                floatSpeedProp.floatValue = 2f;
                floatHeightProp.floatValue = 0.1f;
                enableRotationProp.boolValue = false;
                interactionRadiusProp.floatValue = 1.5f;
            }
            
            EditorGUILayout.EndHorizontal();
            
            EditorGUILayout.BeginHorizontal();
            
            if (GUILayout.Button("Key"))
            {
                spriteSizeProp.vector2Value = new Vector2(0.4f, 0.4f);
                enableFloatingProp.boolValue = true;
                floatSpeedProp.floatValue = 0.8f;
                floatHeightProp.floatValue = 0.25f;
                enableRotationProp.boolValue = true;
                rotationSpeedProp.floatValue = 45f;
                interactionRadiusProp.floatValue = 1.5f;
            }
            
            if (GUILayout.Button("No Animation"))
            {
                enableFloatingProp.boolValue = false;
                enableRotationProp.boolValue = false;
            }
            
            EditorGUILayout.EndHorizontal();
        }
    }
}
