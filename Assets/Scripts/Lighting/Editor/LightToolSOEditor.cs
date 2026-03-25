using System.Collections.Generic;
using UnityEngine;
using UnityEditor;

namespace TheHunt.Lighting.Editor
{
    [CustomEditor(typeof(LightToolSO))]
    public class LightToolSOEditor : UnityEditor.Editor
    {
        // ── Serialized Properties ─────────────────────────────────────────────
        private SerializedProperty levelSectionDataProp;
        private SerializedProperty depthLayersProp;
        private SerializedProperty selectedLayerIndexProp;
        private SerializedProperty spacingTypeProp;
        private SerializedProperty automaticSpacingProp;
        private SerializedProperty globalLightColorProp;
        private SerializedProperty opacityMultiplierProp;
        private SerializedProperty depthPreviewActiveProp;
        private SerializedProperty depthPreviewOpacityProp;
        private SerializedProperty midgroundSortingLayerProp;
        private SerializedProperty sortingOrderStartProp;
        private SerializedProperty sortingOrderStepProp;

        // ── Foldout states ────────────────────────────────────────────────────
        private bool sectionLevelData  = true;
        private bool sectionLightData  = true;
        private bool sectionSettings   = true;
        private bool sectionPreview    = true;
        private bool sectionSorting    = true;

        // ── Styles ────────────────────────────────────────────────────────────
        private GUIStyle headerStyle;

        // ── Colors ────────────────────────────────────────────────────────────
        private static readonly Color HeaderBg       = new Color(0.15f, 0.15f, 0.15f, 1f);
        private static readonly Color ActiveLayerBg  = new Color(0.18f, 0.36f, 0.52f, 1f);
        private static readonly Color ScanResultBg   = new Color(0.13f, 0.22f, 0.13f, 1f);
        private static readonly Color DirtyBg        = new Color(0.30f, 0.15f, 0.10f, 1f);

        // ── Sorting Assigner state (not serialized – editor-only) ─────────────
        private List<SpriteSortingEntry> scanResults = new List<SpriteSortingEntry>();
        private bool scanDirty = false;     // true when params changed after last scan
        private Vector2 scanScrollPos;

        // ─────────────────────────────────────────────────────────────────────

        private void OnEnable()
        {
            levelSectionDataProp     = serializedObject.FindProperty("levelSectionData");
            depthLayersProp          = serializedObject.FindProperty("depthLayers");
            selectedLayerIndexProp   = serializedObject.FindProperty("selectedLayerIndex");
            spacingTypeProp          = serializedObject.FindProperty("spacingType");
            automaticSpacingProp     = serializedObject.FindProperty("automaticSpacing");
            globalLightColorProp     = serializedObject.FindProperty("globalLightColor");
            opacityMultiplierProp    = serializedObject.FindProperty("opacityMultiplier");
            depthPreviewActiveProp   = serializedObject.FindProperty("depthPreviewActive");
            depthPreviewOpacityProp  = serializedObject.FindProperty("depthPreviewOpacity");
            midgroundSortingLayerProp = serializedObject.FindProperty("midgroundSortingLayer");
            sortingOrderStartProp    = serializedObject.FindProperty("sortingOrderStart");
            sortingOrderStepProp     = serializedObject.FindProperty("sortingOrderStep");

            SceneView.duringSceneGui += OnSceneGUI;
        }

        private void OnDisable()
        {
            SceneView.duringSceneGui -= OnSceneGUI;
            scanResults.Clear();
        }

        public override void OnInspectorGUI()
        {
            InitStyles();
            serializedObject.Update();

            DrawToolHeader();
            EditorGUILayout.Space(6f);

            DrawLevelSectionData();
            EditorGUILayout.Space(4f);

            DrawLightData();
            EditorGUILayout.Space(4f);

            DrawLightSettings();
            EditorGUILayout.Space(4f);

            DrawDepthPreview();
            EditorGUILayout.Space(4f);

            DrawSortingAssigner();

            serializedObject.ApplyModifiedProperties();
        }

        // ─────────────────────────────────────────────────────────────────────
        // Header
        // ─────────────────────────────────────────────────────────────────────

        private void DrawToolHeader()
        {
            Rect headerRect = EditorGUILayout.BeginVertical();
            EditorGUI.DrawRect(headerRect, HeaderBg);

            GUIStyle title = new GUIStyle(EditorStyles.boldLabel)
            {
                fontSize  = 15,
                alignment = TextAnchor.MiddleCenter,
                normal    = { textColor = Color.white }
            };

            EditorGUILayout.Space(6f);
            EditorGUILayout.LabelField("Depth Light Placement Tool", title);
            EditorGUILayout.Space(4f);

            GUIStyle sub = new GUIStyle(EditorStyles.miniLabel)
            {
                alignment = TextAnchor.MiddleCenter,
                normal    = { textColor = new Color(0.75f, 0.75f, 0.75f) }
            };
            EditorGUILayout.LabelField("Level design helper — depth layer management & sorting", sub);
            EditorGUILayout.Space(6f);
            EditorGUILayout.EndVertical();
        }

        // ─────────────────────────────────────────────────────────────────────
        // Section 1 — Level Section Data
        // ─────────────────────────────────────────────────────────────────────

        private void DrawLevelSectionData()
        {
            sectionLevelData = DrawFoldoutSection("Level Section Data", sectionLevelData, () =>
            {
                EditorGUILayout.PropertyField(
                    levelSectionDataProp,
                    new GUIContent("Level SO", "ScriptableObject representing the level section."));

                if (levelSectionDataProp.objectReferenceValue == null)
                {
                    EditorGUILayout.HelpBox("Assign the level section ScriptableObject.", MessageType.Info);
                }
                else
                {
                    EditorGUILayout.HelpBox(
                        $"Level SO: {levelSectionDataProp.objectReferenceValue.name}",
                        MessageType.None);
                }
            });
        }

        // ─────────────────────────────────────────────────────────────────────
        // Section 2 — Light Data
        // ─────────────────────────────────────────────────────────────────────

        private void DrawLightData()
        {
            sectionLightData = DrawFoldoutSection("Light Data", sectionLightData, () =>
            {
                int layerCount = depthLayersProp.arraySize;

                if (layerCount > 0)
                {
                    EditorGUILayout.LabelField("Depth Layer", EditorStyles.boldLabel);
                    DrawLayerToolbar(layerCount);
                    EditorGUILayout.Space(4f);
                    DrawSelectedLayerDetails();
                    EditorGUILayout.Space(6f);
                }
                else
                {
                    EditorGUILayout.HelpBox("No depth layers defined. Add one below.", MessageType.Info);
                    EditorGUILayout.Space(4f);
                }

                EditorGUILayout.BeginHorizontal();
                if (GUILayout.Button("+ Add Layer", GUILayout.Height(24f)))
                {
                    AddLayer();
                }
                using (new EditorGUI.DisabledGroupScope(layerCount == 0))
                {
                    if (GUILayout.Button("- Remove Selected", GUILayout.Height(24f)))
                    {
                        RemoveSelectedLayer();
                    }
                }
                EditorGUILayout.EndHorizontal();

                EditorGUILayout.Space(6f);
                DrawSeparator();
                EditorGUILayout.Space(4f);

                EditorGUILayout.LabelField("Spacing Type", EditorStyles.boldLabel);
                EditorGUILayout.PropertyField(spacingTypeProp, new GUIContent("Spacing"));

                DepthSpacingType currentSpacing = (DepthSpacingType)spacingTypeProp.enumValueIndex;

                if (currentSpacing == DepthSpacingType.Automatic)
                {
                    EditorGUI.indentLevel++;
                    EditorGUILayout.PropertyField(automaticSpacingProp,
                        new GUIContent("Z Spacing", "World-space Z distance between consecutive layers."));
                    EditorGUI.indentLevel--;
                    EditorGUILayout.HelpBox(
                        "Automatic: each layer Z is calculated as index × spacing.",
                        MessageType.Info);
                }
                else
                {
                    EditorGUILayout.HelpBox(
                        "Manual: set the Z position per layer in the layer details above.",
                        MessageType.Info);
                }
            });
        }

        private void DrawLayerToolbar(int layerCount)
        {
            float buttonWidth = Mathf.Max(60f, (EditorGUIUtility.currentViewWidth - 32f) / Mathf.Min(layerCount, 6));

            EditorGUILayout.BeginHorizontal();

            for (int i = 0; i < layerCount; i++)
            {
                SerializedProperty layerProp = depthLayersProp.GetArrayElementAtIndex(i);
                SerializedProperty nameProp  = layerProp.FindPropertyRelative("layerName");

                bool isSelected = (i == selectedLayerIndexProp.intValue);

                Color prevBg = GUI.backgroundColor;
                GUI.backgroundColor = isSelected ? new Color(0.3f, 0.6f, 1f) : Color.white;

                if (GUILayout.Button(nameProp.stringValue, GUILayout.Width(buttonWidth), GUILayout.Height(28f)))
                {
                    selectedLayerIndexProp.intValue = i;
                }

                GUI.backgroundColor = prevBg;
            }

            EditorGUILayout.EndHorizontal();
        }

        private void DrawSelectedLayerDetails()
        {
            int idx = selectedLayerIndexProp.intValue;
            if (idx < 0 || idx >= depthLayersProp.arraySize) return;

            SerializedProperty layerProp    = depthLayersProp.GetArrayElementAtIndex(idx);
            SerializedProperty nameProp     = layerProp.FindPropertyRelative("layerName");
            SerializedProperty categoryProp = layerProp.FindPropertyRelative("category");
            SerializedProperty zPosProp     = layerProp.FindPropertyRelative("zPosition");
            SerializedProperty colorProp    = layerProp.FindPropertyRelative("layerColor");
            SerializedProperty opacityProp  = layerProp.FindPropertyRelative("opacity");

            EditorGUILayout.BeginVertical(EditorStyles.helpBox);

            Color prevBg = GUI.backgroundColor;
            GUI.backgroundColor = ActiveLayerBg;

            EditorGUI.indentLevel++;

            EditorGUILayout.PropertyField(nameProp,     new GUIContent("Name"));
            EditorGUILayout.PropertyField(categoryProp, new GUIContent("Category",
                "Broad depth category — Foreground / Midground / Background."));

            bool isManual = (DepthSpacingType)spacingTypeProp.enumValueIndex == DepthSpacingType.Manual;
            using (new EditorGUI.DisabledGroupScope(!isManual))
            {
                EditorGUILayout.PropertyField(zPosProp,
                    new GUIContent("Z Position", isManual
                        ? "World-space Z for this layer."
                        : "Calculated automatically — switch to Manual to edit."));
            }

            if (!isManual)
            {
                float autoZ = idx * automaticSpacingProp.floatValue;
                EditorGUILayout.LabelField("  Computed Z", $"{autoZ:F2}", EditorStyles.miniLabel);
            }

            EditorGUILayout.PropertyField(colorProp,   new GUIContent("Layer Tint"));
            EditorGUILayout.PropertyField(opacityProp, new GUIContent("Opacity"));

            EditorGUI.indentLevel--;
            GUI.backgroundColor = prevBg;

            EditorGUILayout.EndVertical();
        }

        private void AddLayer()
        {
            depthLayersProp.arraySize++;
            int newIdx = depthLayersProp.arraySize - 1;
            SerializedProperty newLayer = depthLayersProp.GetArrayElementAtIndex(newIdx);
            newLayer.FindPropertyRelative("layerName").stringValue  = $"Layer {newIdx}";
            newLayer.FindPropertyRelative("category").enumValueIndex = (int)DepthCategory.Midground;
            newLayer.FindPropertyRelative("zPosition").floatValue   = newIdx;
            newLayer.FindPropertyRelative("layerColor").colorValue  = Color.white;
            newLayer.FindPropertyRelative("opacity").floatValue     = 1f;
            selectedLayerIndexProp.intValue = newIdx;
        }

        private void RemoveSelectedLayer()
        {
            int idx = selectedLayerIndexProp.intValue;
            depthLayersProp.DeleteArrayElementAtIndex(idx);
            int newCount = depthLayersProp.arraySize;

            if (newCount == 0)
                selectedLayerIndexProp.intValue = -1;
            else
                selectedLayerIndexProp.intValue = Mathf.Clamp(idx, 0, newCount - 1);
        }

        // ─────────────────────────────────────────────────────────────────────
        // Section 3 — Light Settings
        // ─────────────────────────────────────────────────────────────────────

        private void DrawLightSettings()
        {
            sectionSettings = DrawFoldoutSection("Light Settings", sectionSettings, () =>
            {
                EditorGUILayout.PropertyField(globalLightColorProp,
                    new GUIContent("Global Light Color",
                        "Base color that fades across all depth layers."));

                EditorGUILayout.PropertyField(opacityMultiplierProp,
                    new GUIContent("Opacity Multiplier",
                        "Multiplied by each layer's individual opacity."));

                EditorGUILayout.Space(4f);

                int layerCount = depthLayersProp.arraySize;
                if (layerCount > 0)
                {
                    EditorGUILayout.LabelField("Effective Colors per Layer", EditorStyles.boldLabel);
                    LightToolSO so = (LightToolSO)target;

                    for (int i = 0; i < layerCount; i++)
                    {
                        Color effective = so.GetEffectiveLayerColor(i);
                        EditorGUILayout.BeginHorizontal();

                        string name = depthLayersProp.GetArrayElementAtIndex(i)
                            .FindPropertyRelative("layerName").stringValue;

                        EditorGUILayout.LabelField(name, GUILayout.Width(100f));
                        Rect colorRect = EditorGUILayout.GetControlRect(false, 18f);
                        EditorGUI.DrawRect(colorRect, new Color(effective.r, effective.g, effective.b, 1f));

                        EditorGUILayout.LabelField(
                            $"α {effective.a:F2}",
                            EditorStyles.miniLabel,
                            GUILayout.Width(50f));

                        EditorGUILayout.EndHorizontal();
                    }
                }
            });
        }

        // ─────────────────────────────────────────────────────────────────────
        // Section 4 — Depth Preview
        // ─────────────────────────────────────────────────────────────────────

        private void DrawDepthPreview()
        {
            sectionPreview = DrawFoldoutSection("Depth Preview", sectionPreview, () =>
            {
                int layerCount = depthLayersProp.arraySize;

                if (layerCount == 0)
                {
                    EditorGUILayout.HelpBox("Add at least one depth layer to use preview.", MessageType.Info);
                    return;
                }

                EditorGUILayout.PropertyField(depthPreviewActiveProp,
                    new GUIContent("Enable Preview",
                        "Draws a magenta overlay over the active depth layer in the Scene View."));

                if (depthPreviewActiveProp.boolValue)
                {
                    EditorGUI.indentLevel++;
                    EditorGUILayout.PropertyField(depthPreviewOpacityProp,
                        new GUIContent("Overlay Opacity"));
                    EditorGUI.indentLevel--;

                    int selIdx = selectedLayerIndexProp.intValue;
                    if (selIdx >= 0 && selIdx < layerCount)
                    {
                        string selName = depthLayersProp.GetArrayElementAtIndex(selIdx)
                            .FindPropertyRelative("layerName").stringValue;

                        EditorGUILayout.HelpBox(
                            $"Previewing: \"{selName}\" (index {selIdx})\n" +
                            "Magenta overlay visible in Scene View — requires Orthographic camera.",
                            MessageType.None);
                    }

                    SceneView.RepaintAll();
                }
                else
                {
                    EditorGUILayout.HelpBox(
                        "Enable to display a magenta overlay in Scene View for the selected layer.",
                        MessageType.None);
                }
            });
        }

        // ─────────────────────────────────────────────────────────────────────
        // Section 5 — Sorting Assigner
        // ─────────────────────────────────────────────────────────────────────

        private void DrawSortingAssigner()
        {
            sectionSorting = DrawFoldoutSection("Sorting Assigner — Mid-ground", sectionSorting, () =>
            {
                EditorGUILayout.HelpBox(
                    "Collects every SpriteRenderer whose Sorting Layer matches the field below, " +
                    "sorts them by world Z (ascending = closest first) and computes a linear " +
                    "Sorting Order based on their position. Press Scan to preview, then Apply to commit.",
                    MessageType.Info);

                EditorGUILayout.Space(4f);

                // ── Config ────────────────────────────────────────────────────
                EditorGUILayout.LabelField("Configuration", EditorStyles.boldLabel);

                EditorGUI.BeginChangeCheck();

                DrawSortingLayerPopup();

                EditorGUILayout.PropertyField(sortingOrderStartProp,
                    new GUIContent("Start Order", "Sorting Order assigned to the sprite with the lowest Z."));
                EditorGUILayout.PropertyField(sortingOrderStepProp,
                    new GUIContent("Step per Unit Z",
                        "Sorting Order increment per world-unit of Z depth. Use 1 for dense scenes, higher values for scenes with few sprites."));

                if (EditorGUI.EndChangeCheck())
                {
                    // Parameters changed — mark scan as dirty so the user re-scans
                    if (scanResults.Count > 0)
                        scanDirty = true;
                }

                EditorGUILayout.Space(6f);

                // ── Action buttons ────────────────────────────────────────────
                EditorGUILayout.BeginHorizontal();

                Color prevBg = GUI.backgroundColor;
                GUI.backgroundColor = new Color(0.2f, 0.55f, 0.9f);
                if (GUILayout.Button("Scan Scene", GUILayout.Height(28f)))
                {
                    RunScan();
                }
                GUI.backgroundColor = prevBg;

                using (new EditorGUI.DisabledGroupScope(scanResults.Count == 0 || scanDirty))
                {
                    GUI.backgroundColor = new Color(0.2f, 0.75f, 0.35f);
                    if (GUILayout.Button("Apply to Scene", GUILayout.Height(28f)))
                    {
                        ApplySortingOrders();
                    }
                    GUI.backgroundColor = prevBg;
                }

                if (scanResults.Count > 0)
                {
                    GUI.backgroundColor = new Color(0.75f, 0.25f, 0.25f);
                    if (GUILayout.Button("Clear", GUILayout.Height(28f), GUILayout.Width(56f)))
                    {
                        scanResults.Clear();
                        scanDirty = false;
                    }
                    GUI.backgroundColor = prevBg;
                }

                EditorGUILayout.EndHorizontal();

                if (scanDirty)
                {
                    EditorGUILayout.HelpBox("Parameters changed since last scan. Run Scan again before applying.", MessageType.Warning);
                }

                // ── Scan results table ────────────────────────────────────────
                if (scanResults.Count > 0)
                {
                    EditorGUILayout.Space(6f);
                    EditorGUILayout.LabelField($"Results — {scanResults.Count} sprite(s) found", EditorStyles.boldLabel);

                    DrawSeparator();
                    EditorGUILayout.Space(2f);

                    // Column headers
                    EditorGUILayout.BeginHorizontal();
                    EditorGUILayout.LabelField("GameObject",          EditorStyles.miniLabel, GUILayout.Width(160f));
                    EditorGUILayout.LabelField("World Z",             EditorStyles.miniLabel, GUILayout.Width(60f));
                    EditorGUILayout.LabelField("Current Order",       EditorStyles.miniLabel, GUILayout.Width(80f));
                    EditorGUILayout.LabelField("New Order",           EditorStyles.miniLabel, GUILayout.Width(80f));
                    EditorGUILayout.EndHorizontal();

                    DrawSeparator();

                    const float RowHeight  = 20f;
                    const float MaxVisible = 8f;
                    float scrollHeight = Mathf.Min(scanResults.Count, MaxVisible) * RowHeight;

                    scanScrollPos = EditorGUILayout.BeginScrollView(scanScrollPos,
                        GUILayout.Height(scrollHeight + 4f));

                    foreach (SpriteSortingEntry entry in scanResults)
                    {
                        if (entry.renderer == null) continue;

                        bool changed = entry.pendingSortingOrder != entry.originalSortingOrder;

                        Color rowBg = changed ? ScanResultBg : new Color(0.2f, 0.2f, 0.2f, 0f);
                        Rect rowRect = EditorGUILayout.BeginHorizontal();
                        EditorGUI.DrawRect(rowRect, rowBg);

                        // Object ping
                        GUIStyle linkStyle = new GUIStyle(EditorStyles.miniLabel)
                        {
                            normal = { textColor = changed ? new Color(0.5f, 1f, 0.5f) : Color.gray }
                        };
                        if (GUILayout.Button(entry.renderer.name, linkStyle, GUILayout.Width(160f)))
                        {
                            EditorGUIUtility.PingObject(entry.renderer.gameObject);
                            Selection.activeGameObject = entry.renderer.gameObject;
                        }

                        EditorGUILayout.LabelField($"{entry.worldZ:F2}", EditorStyles.miniLabel, GUILayout.Width(60f));
                        EditorGUILayout.LabelField($"{entry.originalSortingOrder}", EditorStyles.miniLabel, GUILayout.Width(80f));

                        GUIStyle newOrderStyle = new GUIStyle(EditorStyles.miniLabel)
                        {
                            fontStyle = changed ? FontStyle.Bold : FontStyle.Normal,
                            normal    = { textColor = changed ? new Color(0.4f, 1f, 0.4f) : Color.gray }
                        };
                        EditorGUILayout.LabelField($"{entry.pendingSortingOrder}", newOrderStyle, GUILayout.Width(80f));

                        EditorGUILayout.EndHorizontal();
                    }

                    EditorGUILayout.EndScrollView();

                    DrawSeparator();

                    int changedCount = 0;
                    foreach (var e in scanResults) if (e.pendingSortingOrder != e.originalSortingOrder) changedCount++;
                    EditorGUILayout.LabelField($"{changedCount} sprite(s) will change Sorting Order.",
                        EditorStyles.miniLabel);
                }
            });
        }

        /// <summary>Draws a popup listing all valid Sorting Layer names from the project.</summary>
        private void DrawSortingLayerPopup()
        {
            string[] layerNames = GetSortingLayerNames();
            string currentName  = midgroundSortingLayerProp.stringValue;
            int currentIndex    = System.Array.IndexOf(layerNames, currentName);

            if (currentIndex < 0) currentIndex = 0;

            int newIndex = EditorGUILayout.Popup(
                new GUIContent("Sorting Layer", "Only SpriteRenderers on this Sorting Layer will be collected."),
                currentIndex, layerNames);

            if (newIndex != currentIndex || string.IsNullOrEmpty(currentName))
                midgroundSortingLayerProp.stringValue = layerNames[newIndex];
        }

        // ─────────────────────────────────────────────────────────────────────
        // Sorting Assigner logic
        // ─────────────────────────────────────────────────────────────────────

        /// <summary>
        /// Collects all SpriteRenderers in the open scenes that match the configured
        /// Sorting Layer, sorts them by world-space Z ascending, then computes the
        /// pending linear Sorting Order for each. Does NOT write to the scene yet.
        /// </summary>
        private void RunScan()
        {
            serializedObject.ApplyModifiedProperties();

            LightToolSO so = (LightToolSO)target;
            scanResults.Clear();
            scanDirty = false;

            SpriteRenderer[] allRenderers =
                Object.FindObjectsByType<SpriteRenderer>(FindObjectsSortMode.None);

            string targetLayer = so.midgroundSortingLayer;

            // 1. Filter by sorting layer
            var filtered = new List<SpriteRenderer>();
            foreach (SpriteRenderer sr in allRenderers)
            {
                if (sr.sortingLayerName == targetLayer)
                    filtered.Add(sr);
            }

            if (filtered.Count == 0)
            {
                EditorUtility.DisplayDialog(
                    "Sorting Assigner",
                    $"No SpriteRenderers found on Sorting Layer \"{targetLayer}\".\n\nMake sure your Mid-ground sprites use this Sorting Layer.",
                    "OK");
                return;
            }

            // 2. Sort by world Z ascending (smallest Z = closest to camera in 2D)
            filtered.Sort((a, b) =>
                a.transform.position.z.CompareTo(b.transform.position.z));

            // 3. Compute linear orders
            float minZ = filtered[0].transform.position.z;
            foreach (SpriteRenderer sr in filtered)
            {
                float worldZ = sr.transform.position.z;
                int pending  = so.ComputeSortingOrder(worldZ, minZ);

                scanResults.Add(new SpriteSortingEntry
                {
                    renderer             = sr,
                    worldZ               = worldZ,
                    pendingSortingOrder  = pending,
                    originalSortingOrder = sr.sortingOrder
                });
            }

            Debug.Log($"[LightTool] Scan complete — {scanResults.Count} sprite(s) collected on layer \"{targetLayer}\".");
            Repaint();
        }

        /// <summary>
        /// Writes the pending Sorting Order values computed by RunScan() to each
        /// SpriteRenderer. Records an Undo group so the operation can be reverted.
        /// </summary>
        private void ApplySortingOrders()
        {
            if (scanResults.Count == 0) return;

            Undo.SetCurrentGroupName("LightTool — Apply Sorting Orders");
            int undoGroup = Undo.GetCurrentGroup();

            int applied = 0;
            foreach (SpriteSortingEntry entry in scanResults)
            {
                if (entry.renderer == null) continue;
                if (entry.pendingSortingOrder == entry.originalSortingOrder) continue;

                Undo.RecordObject(entry.renderer, "Set Sorting Order");
                entry.renderer.sortingOrder = entry.pendingSortingOrder;
                entry.originalSortingOrder  = entry.pendingSortingOrder;
                applied++;
            }

            Undo.CollapseUndoOperations(undoGroup);

            Debug.Log($"[LightTool] Applied Sorting Order to {applied} sprite(s). Use Ctrl+Z to revert.");
            Repaint();
        }

        // ─────────────────────────────────────────────────────────────────────
        // Scene View overlay
        // ─────────────────────────────────────────────────────────────────────

        private void OnSceneGUI(SceneView sceneView)
        {
            if (!depthPreviewActiveProp.boolValue) return;

            LightToolSO so = (LightToolSO)target;

            int idx = so.selectedLayerIndex;
            if (so.depthLayers == null || so.depthLayers.Count == 0) return;
            idx = Mathf.Clamp(idx, 0, so.depthLayers.Count - 1);

            UnityEngine.Camera cam = sceneView.camera;

            if (!cam.orthographic)
            {
                // Show a warning handle instead of drawing the overlay
                Handles.BeginGUI();
                GUILayout.BeginArea(new Rect(8f, 8f, 320f, 32f));
                EditorGUILayout.HelpBox("Depth Preview requires Orthographic Scene camera.", MessageType.Warning);
                GUILayout.EndArea();
                Handles.EndGUI();
                return;
            }

            float zPos   = so.GetLayerZPosition(idx);
            float halfH  = cam.orthographicSize;
            float halfW  = halfH * cam.aspect;
            Vector3 center = new Vector3(cam.transform.position.x, cam.transform.position.y, zPos);

            Color overlayColor = new Color(1f, 0f, 1f, so.depthPreviewOpacity);
            Handles.color = overlayColor;

            Vector3[] verts =
            {
                center + new Vector3(-halfW, -halfH, 0f),
                center + new Vector3(-halfW,  halfH, 0f),
                center + new Vector3( halfW,  halfH, 0f),
                center + new Vector3( halfW, -halfH, 0f),
            };

            Handles.DrawSolidRectangleWithOutline(verts, overlayColor, Color.clear);

            GUIStyle labelStyle = new GUIStyle
            {
                normal    = { textColor = Color.white },
                fontSize  = 13,
                fontStyle = FontStyle.Bold,
                alignment = TextAnchor.UpperLeft
            };

            string layerName = so.depthLayers[idx].layerName;
            Handles.Label(
                center + new Vector3(-halfW + 0.2f, halfH - 0.2f, 0f),
                $"[ DEPTH PREVIEW: {layerName} | Z={zPos:F2} ]",
                labelStyle);

            sceneView.Repaint();
        }

        // ─────────────────────────────────────────────────────────────────────
        // Helpers
        // ─────────────────────────────────────────────────────────────────────

        private bool DrawFoldoutSection(string title, bool foldout, System.Action drawContent)
        {
            EditorGUILayout.BeginVertical(EditorStyles.helpBox);

            EditorGUILayout.BeginHorizontal();
            GUIStyle fStyle = new GUIStyle(EditorStyles.foldoutHeader)
            {
                fontStyle = FontStyle.Bold,
                fontSize  = 11
            };
            foldout = EditorGUILayout.Foldout(foldout, title, true, fStyle);
            EditorGUILayout.EndHorizontal();

            if (foldout)
            {
                EditorGUILayout.Space(2f);
                drawContent?.Invoke();
                EditorGUILayout.Space(2f);
            }

            EditorGUILayout.EndVertical();
            return foldout;
        }

        private void InitStyles()
        {
            if (headerStyle != null) return;
            headerStyle = new GUIStyle(EditorStyles.boldLabel)
            {
                fontSize  = 14,
                alignment = TextAnchor.MiddleCenter
            };
        }

        private static void DrawSeparator()
        {
            Rect rect = EditorGUILayout.GetControlRect(false, 1f);
            EditorGUI.DrawRect(rect, new Color(0.4f, 0.4f, 0.4f, 1f));
        }

        /// <summary>Returns all Sorting Layer names defined in the project.</summary>
        private static string[] GetSortingLayerNames()
        {
            var names = new List<string>();
            foreach (var layer in SortingLayer.layers)
                names.Add(layer.name);
            return names.ToArray();
        }
    }
}
