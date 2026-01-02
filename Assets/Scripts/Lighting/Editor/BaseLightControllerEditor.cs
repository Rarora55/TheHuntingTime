using UnityEngine;
using UnityEditor;
using UnityEngine.Rendering.Universal;

namespace TheHunt.Lighting.Editor
{
    [CustomEditor(typeof(BaseLightController), true)]
    public class BaseLightControllerEditor : UnityEditor.Editor
    {
        private SerializedProperty light2DProp;
        private SerializedProperty startEnabledProp;
        private SerializedProperty baseIntensityProp;
        private SerializedProperty behaviorProp;
        private SerializedProperty flickerSpeedProp;
        private SerializedProperty flickerAmountProp;
        private SerializedProperty castsShadowsProp;
        private SerializedProperty shadowQualityProp;

        private BaseLightController lightController;
        private Light2D light2D;

        private void OnEnable()
        {
            light2DProp = serializedObject.FindProperty("light2D");
            startEnabledProp = serializedObject.FindProperty("startEnabled");
            baseIntensityProp = serializedObject.FindProperty("baseIntensity");
            behaviorProp = serializedObject.FindProperty("behavior");
            flickerSpeedProp = serializedObject.FindProperty("flickerSpeed");
            flickerAmountProp = serializedObject.FindProperty("flickerAmount");
            castsShadowsProp = serializedObject.FindProperty("castsShadows");
            shadowQualityProp = serializedObject.FindProperty("shadowQuality");

            lightController = (BaseLightController)target;
            light2D = lightController.GetComponent<Light2D>();
        }

        public override void OnInspectorGUI()
        {
            serializedObject.Update();

            DrawHeader();
            EditorGUILayout.Space(5);

            DrawReferencesSection();
            EditorGUILayout.Space(5);

            DrawBaseSettingsSection();
            EditorGUILayout.Space(5);

            DrawBehaviorSection();
            EditorGUILayout.Space(5);

            DrawPerformanceSection();
            EditorGUILayout.Space(5);

            DrawQuickActions();

            serializedObject.ApplyModifiedProperties();
        }

        private void DrawHeader()
        {
            EditorGUILayout.BeginVertical(EditorStyles.helpBox);
            
            GUIStyle headerStyle = new GUIStyle(EditorStyles.boldLabel)
            {
                fontSize = 14,
                alignment = TextAnchor.MiddleCenter
            };

            EditorGUILayout.LabelField("💡 Light Controller", headerStyle);

            if (light2D != null)
            {
                GUIStyle infoStyle = new GUIStyle(EditorStyles.miniLabel)
                {
                    alignment = TextAnchor.MiddleCenter,
                    normal = { textColor = Color.gray }
                };

                string info = $"Type: {light2D.lightType} | Intensity: {light2D.intensity:F2}";
                EditorGUILayout.LabelField(info, infoStyle);
            }

            EditorGUILayout.EndVertical();
        }

        private void DrawReferencesSection()
        {
            EditorGUILayout.BeginVertical(EditorStyles.helpBox);
            EditorGUILayout.LabelField("References", EditorStyles.boldLabel);

            EditorGUILayout.PropertyField(light2DProp);

            if (light2D == null)
            {
                EditorGUILayout.HelpBox("Light2D component is required!", MessageType.Error);
                
                if (GUILayout.Button("Add Light2D Component"))
                {
                    light2D = lightController.gameObject.AddComponent<Light2D>();
                    light2DProp.objectReferenceValue = light2D;
                }
            }

            EditorGUILayout.EndVertical();
        }

        private void DrawBaseSettingsSection()
        {
            EditorGUILayout.BeginVertical(EditorStyles.helpBox);
            EditorGUILayout.LabelField("Base Settings", EditorStyles.boldLabel);

            EditorGUILayout.PropertyField(startEnabledProp);
            EditorGUILayout.PropertyField(baseIntensityProp);

            if (light2D != null && GUILayout.Button("Copy Intensity from Light2D"))
            {
                baseIntensityProp.floatValue = light2D.intensity;
            }

            EditorGUILayout.EndVertical();
        }

        private void DrawBehaviorSection()
        {
            EditorGUILayout.BeginVertical(EditorStyles.helpBox);
            EditorGUILayout.LabelField("Behavior", EditorStyles.boldLabel);

            EditorGUILayout.PropertyField(behaviorProp);

            LightBehavior currentBehavior = (LightBehavior)behaviorProp.enumValueIndex;

            if (currentBehavior != LightBehavior.Static)
            {
                EditorGUI.indentLevel++;
                EditorGUILayout.PropertyField(flickerSpeedProp);
                EditorGUILayout.PropertyField(flickerAmountProp);
                EditorGUI.indentLevel--;

                DrawBehaviorHelpBox(currentBehavior);
            }
            else
            {
                EditorGUILayout.HelpBox("Static lights do not animate.", MessageType.Info);
            }

            EditorGUILayout.EndVertical();
        }

        private void DrawBehaviorHelpBox(LightBehavior behavior)
        {
            string message = behavior switch
            {
                LightBehavior.Flickering => "Smooth flickering using Perlin Noise. Good for torches.",
                LightBehavior.Pulsating => "Rhythmic pulsation using sine wave. Good for candles.",
                LightBehavior.Random => "Random intensity changes. Good for damaged lights.",
                _ => ""
            };

            if (!string.IsNullOrEmpty(message))
            {
                EditorGUILayout.HelpBox(message, MessageType.Info);
            }
        }

        private void DrawPerformanceSection()
        {
            EditorGUILayout.BeginVertical(EditorStyles.helpBox);
            EditorGUILayout.LabelField("Performance", EditorStyles.boldLabel);

            EditorGUILayout.PropertyField(castsShadowsProp);

            if (castsShadowsProp.boolValue)
            {
                EditorGUI.indentLevel++;
                EditorGUILayout.PropertyField(shadowQualityProp);
                EditorGUI.indentLevel--;

                EditorGUILayout.HelpBox("Shadows are expensive. Consider disabling for small lights.", MessageType.Warning);
            }

            EditorGUILayout.EndVertical();
        }

        private void DrawQuickActions()
        {
            EditorGUILayout.BeginVertical(EditorStyles.helpBox);
            EditorGUILayout.LabelField("Quick Actions", EditorStyles.boldLabel);

            EditorGUILayout.BeginHorizontal();

            GUI.enabled = Application.isPlaying;

            if (GUILayout.Button("Turn ON"))
            {
                lightController.TurnOn();
            }

            if (GUILayout.Button("Turn OFF"))
            {
                lightController.TurnOff();
            }

            GUI.enabled = true;

            EditorGUILayout.EndHorizontal();

            if (!Application.isPlaying)
            {
                EditorGUILayout.HelpBox("Quick Actions available in Play Mode only.", MessageType.Info);
            }

            EditorGUILayout.EndVertical();
        }
    }
}
