using UnityEngine;

namespace TheHunt.Testing
{
    public class GridDrawer : MonoBehaviour
    {
        [Header("Grid Settings")]
        [SerializeField] private int gridWidth = 50;
        [SerializeField] private int gridHeight = 30;
        [SerializeField] private float cellSize = 1f;

        [Header("Visual")]
        [SerializeField] private Color gridColor = new Color(1, 1, 1, 0.2f);
        [SerializeField] private Color majorLineColor = new Color(1, 1, 0, 0.4f);
        [SerializeField] private int majorLineInterval = 5;
        [SerializeField] private bool showLabels = true;

        private void OnDrawGizmos()
        {
            Vector3 origin = transform.position;

            for (int y = 0; y <= gridHeight; y++)
            {
                bool isMajor = y % majorLineInterval == 0;
                Gizmos.color = isMajor ? majorLineColor : gridColor;

                Vector3 start = origin + new Vector3(0, y * cellSize, 0);
                Vector3 end = origin + new Vector3(gridWidth * cellSize, y * cellSize, 0);
                Gizmos.DrawLine(start, end);
            }

            for (int x = 0; x <= gridWidth; x++)
            {
                bool isMajor = x % majorLineInterval == 0;
                Gizmos.color = isMajor ? majorLineColor : gridColor;

                Vector3 start = origin + new Vector3(x * cellSize, 0, 0);
                Vector3 end = origin + new Vector3(x * cellSize, gridHeight * cellSize, 0);
                Gizmos.DrawLine(start, end);
            }

#if UNITY_EDITOR
            if (showLabels)
            {
                DrawLabels(origin);
            }
#endif
        }

#if UNITY_EDITOR
        private void DrawLabels(Vector3 origin)
        {
            GUIStyle style = new GUIStyle();
            style.normal.textColor = majorLineColor;
            style.fontSize = 10;

            for (int x = 0; x <= gridWidth; x += majorLineInterval)
            {
                Vector3 worldPos = origin + new Vector3(x * cellSize, -0.5f, 0);
                Vector3 screenPos = UnityEditor.HandleUtility.WorldToGUIPoint(worldPos);
                UnityEditor.Handles.Label(worldPos, $"{x}m", style);
            }

            for (int y = majorLineInterval; y <= gridHeight; y += majorLineInterval)
            {
                Vector3 worldPos = origin + new Vector3(-0.5f, y * cellSize, 0);
                UnityEditor.Handles.Label(worldPos, $"{y}m", style);
            }
        }
#endif
    }
}
