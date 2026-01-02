using UnityEngine;
using TMPro;

namespace TheHunt.Testing
{
    public class SpeedMeasurer : MonoBehaviour
    {
        [Header("References")]
        [SerializeField] private TextMeshProUGUI timerText;
        [SerializeField] private TextMeshProUGUI speedText;

        [Header("Settings")]
        [SerializeField] private bool isStartTrigger = true;
        [SerializeField] private float trackDistance = 30f;

        private float startTime;
        private bool isRunning = false;
        private Vector3 startPosition;

        private void OnTriggerEnter2D(Collider2D other)
        {
            if (!other.CompareTag("Player")) return;

            if (isStartTrigger)
            {
                StartMeasurement(other.transform.position);
            }
            else
            {
                StopMeasurement(other.transform.position);
            }
        }

        private void Update()
        {
            if (isRunning)
            {
                float elapsed = Time.time - startTime;
                
                if (timerText != null)
                {
                    timerText.text = $"Time: {elapsed:F2}s";
                }
            }
        }

        private void StartMeasurement(Vector3 playerPosition)
        {
            startTime = Time.time;
            startPosition = playerPosition;
            isRunning = true;

            Debug.Log("<color=cyan>[SPEED MEASURER] Started measurement</color>");
        }

        private void StopMeasurement(Vector3 playerPosition)
        {
            if (!isRunning) return;

            float elapsed = Time.time - startTime;
            float distance = Vector3.Distance(startPosition, playerPosition);
            float averageSpeed = distance / elapsed;

            isRunning = false;

            if (timerText != null)
            {
                timerText.text = $"Time: {elapsed:F2}s\nDistance: {distance:F2}m\nAvg Speed: {averageSpeed:F2} m/s";
            }

            Debug.Log($"<color=green>[SPEED MEASURER] Finished!\nTime: {elapsed:F2}s | Distance: {distance:F2}m | Speed: {averageSpeed:F2} m/s</color>");
        }

        public void ResetMeasurement()
        {
            isRunning = false;
            
            if (timerText != null)
            {
                timerText.text = "Ready...";
            }

            if (speedText != null)
            {
                speedText.text = "";
            }
        }

        private void OnDrawGizmos()
        {
            Gizmos.color = isStartTrigger ? Color.green : Color.red;
            
            BoxCollider2D trigger = GetComponent<BoxCollider2D>();
            if (trigger != null)
            {
                Gizmos.matrix = transform.localToWorldMatrix;
                Gizmos.DrawWireCube(trigger.offset, trigger.size);
            }

#if UNITY_EDITOR
            string label = isStartTrigger ? "START" : "FINISH";
            UnityEditor.Handles.Label(transform.position + Vector3.up, label);
#endif
        }
    }
}
