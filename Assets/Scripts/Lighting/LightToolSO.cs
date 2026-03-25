using System.Collections.Generic;
using UnityEngine;

namespace TheHunt.Lighting
{
    /// <summary>
    /// ScriptableObject that holds all configuration for the depth-based
    /// light placement tool. Edit via the custom inspector.
    /// </summary>
    [CreateAssetMenu(fileName = "NewLightTool", menuName = "TheHunt/Lighting/Light Tool")]
    public class LightToolSO : ScriptableObject
    {
        // ── Section 1: Level Section Data ────────────────────────────────────

        [Header("Level Section Data")]
        [Tooltip("ScriptableObject that represents the level section this light tool belongs to.")]
        public ScriptableObject levelSectionData;

        // ── Section 2: Light Data ─────────────────────────────────────────────

        [Header("Light Data")]
        [Tooltip("All depth layers defined for this level section.")]
        public List<DepthLayer> depthLayers = new List<DepthLayer>();

        [Tooltip("Currently selected depth layer index (used by the editor toolbar).")]
        [HideInInspector]
        public int selectedLayerIndex = 0;

        [Tooltip("How layer Z positions are calculated.")]
        public DepthSpacingType spacingType = DepthSpacingType.Manual;

        [Tooltip("Automatic spacing: distance in world units between consecutive depth layers.")]
        public float automaticSpacing = 1f;

        // ── Section 3: Light Settings ─────────────────────────────────────────

        [Header("Light Settings")]
        [Tooltip("Base global light color. Fades across all depth layers.")]
        public Color globalLightColor = Color.white;

        [Tooltip("Global multiplier applied on top of every layer's individual opacity.")]
        [Range(0f, 2f)]
        public float opacityMultiplier = 1f;

        // ── Section 4: Depth Preview ──────────────────────────────────────────

        [Header("Depth Preview")]
        [Tooltip("When enabled, the editor draws a magenta overlay on the active depth layer so you can place assets without overlapping other layers.")]
        [HideInInspector]
        public bool depthPreviewActive = false;

        [Tooltip("Opacity of the magenta preview overlay.")]
        [HideInInspector]
        [Range(0f, 1f)]
        public float depthPreviewOpacity = 0.35f;

        // ── Section 5: Sorting Assigner ───────────────────────────────────────

        [Header("Sorting Assigner")]
        [Tooltip("Sorting layer name to filter Midground sprites. Only SpriteRenderers on this sorting layer will be collected.")]
        public string midgroundSortingLayer = "Default";

        [Tooltip("The Sorting Order value assigned to the sprite with the smallest Z (closest). Subsequent sprites receive incrementally higher values.")]
        public int sortingOrderStart = 0;

        [Tooltip("Step added to the Sorting Order per unit of Z distance. E.g. step=1 with linear Z spacing gives each sprite a unique order.")]
        public int sortingOrderStep = 1;

        // ─────────────────────────────────────────────────────────────────────
        // Helpers
        // ─────────────────────────────────────────────────────────────────────

        /// <summary>Returns the currently selected DepthLayer, or null if none exist.</summary>
        public DepthLayer GetSelectedLayer()
        {
            if (depthLayers == null || depthLayers.Count == 0) return null;
            selectedLayerIndex = Mathf.Clamp(selectedLayerIndex, 0, depthLayers.Count - 1);
            return depthLayers[selectedLayerIndex];
        }

        /// <summary>
        /// Computes the world-space Z position for the given layer index,
        /// respecting the active spacing type.
        /// </summary>
        public float GetLayerZPosition(int index)
        {
            if (depthLayers == null || index < 0 || index >= depthLayers.Count) return 0f;

            if (spacingType == DepthSpacingType.Automatic)
                return index * automaticSpacing;

            return depthLayers[index].zPosition;
        }

        /// <summary>
        /// Returns the effective light color for a given layer, blending the
        /// layer-specific tint with the global color and applying the opacity multiplier.
        /// </summary>
        public Color GetEffectiveLayerColor(int index)
        {
            if (depthLayers == null || index < 0 || index >= depthLayers.Count)
                return globalLightColor;

            DepthLayer layer = depthLayers[index];
            Color blended = Color.Lerp(globalLightColor, layer.layerColor, layer.layerColor.a);
            blended.a = Mathf.Clamp01(layer.opacity * opacityMultiplier);
            return blended;
        }

        /// <summary>
        /// Computes the linear Sorting Order that should be assigned to a sprite at
        /// the given world-space Z, relative to the closest Z value in the scan results.
        /// </summary>
        public int ComputeSortingOrder(float worldZ, float minZ)
        {
            float delta = worldZ - minZ;
            return sortingOrderStart + Mathf.RoundToInt(delta * sortingOrderStep);
        }
    }
}
