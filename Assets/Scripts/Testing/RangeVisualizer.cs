using UnityEngine;

namespace TheHunt.Testing
{
    public class RangeVisualizer : MonoBehaviour
    {
        [Header("Range Settings")]
        [SerializeField] private float[] ranges = { 2f, 5f, 10f, 15f };
        [SerializeField] private string[] rangeLabels = { "Melee", "Close", "Medium", "Far" };

        [Header("Visual")]
        [SerializeField] private Color[] colors = 
        {
            Color.green,
            Color.yellow,
            new Color(1, 0.5f, 0),
            Color.red
        };
        [SerializeField] private int circleSegments = 64;
        [SerializeField] private bool showLabels = true;

        private void OnDrawGizmos()
        {
            Vector3 center = transform.position;

            for (int i = 0; i < ranges.Length; i++)
            {
                Gizmos.color = colors[i];
                DrawCircle(center, ranges[i], circleSegments);

#if UNITY_EDITOR
                if (showLabels && i < rangeLabels.Length)
                {
                    Vector3 labelPos = center + new Vector3(ranges[i], 0, 0);
                    UnityEditor.Handles.Label(labelPos, $"{rangeLabels[i]} ({ranges[i]}m)");
                }
#endif
            }
        }

        private void DrawCircle(Vector3 center, float radius, int segments)
        {
            float angleStep = 360f / segments;
            Vector3 prevPoint = center + new Vector3(radius, 0, 0);

            for (int i = 1; i <= segments; i++)
            {
                float angle = i * angleStep * Mathf.Deg2Rad;
                Vector3 newPoint = center + new Vector3(
                    Mathf.Cos(angle) * radius,
                    Mathf.Sin(angle) * radius,
                    0
                );
                Gizmos.DrawLine(prevPoint, newPoint);
                prevPoint = newPoint;
            }
        }
    }
}
