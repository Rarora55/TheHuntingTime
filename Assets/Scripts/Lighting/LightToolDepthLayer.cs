using UnityEngine;

namespace TheHunt.Lighting
{
    public enum DepthSpacingType
    {
        Manual,
        Automatic
    }

    /// <summary>
    /// Broad category used to classify scene assets by distance from the camera,
    /// following the Tails of Iron depth system defined in AGENT.md.
    /// </summary>
    public enum DepthCategory
    {
        Foreground,
        Midground,
        Background
    }

    [System.Serializable]
    public class DepthLayer
    {
        [Tooltip("Name to identify this depth layer in the editor.")]
        public string layerName = "Layer";

        [Tooltip("Broad depth category for this layer (Foreground / Midground / Background).")]
        public DepthCategory category = DepthCategory.Midground;

        [Tooltip("World-space Z position of this depth layer (Manual spacing only).")]
        public float zPosition = 0f;

        [Tooltip("Light color tint applied exclusively to this depth layer.")]
        public Color layerColor = Color.white;

        [Tooltip("Opacity (alpha) of the global light at this depth layer.")]
        [Range(0f, 1f)]
        public float opacity = 1f;
    }

    /// <summary>
    /// Snapshot produced by the Sorting Assigner scan. Stores the pending Sorting Order
    /// value computed for one SpriteRenderer before it is committed to the scene.
    /// </summary>
    [System.Serializable]
    public class SpriteSortingEntry
    {
        public SpriteRenderer renderer;
        public float          worldZ;
        public int            pendingSortingOrder;
        public int            originalSortingOrder;
    }
}
