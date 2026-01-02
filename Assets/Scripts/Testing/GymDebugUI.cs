using UnityEngine;
using TMPro;

namespace TheHunt.Testing
{
    public class GymDebugUI : MonoBehaviour
    {
        [Header("References")]
        [SerializeField] private Transform player;
        [SerializeField] private Rigidbody2D playerRb;

        [Header("UI Elements")]
        [SerializeField] private TextMeshProUGUI positionText;
        [SerializeField] private TextMeshProUGUI velocityText;
        [SerializeField] private TextMeshProUGUI fpsText;
        [SerializeField] private TextMeshProUGUI stateText;

        [Header("Settings")]
        [SerializeField] private bool autoFindPlayer = true;
        [SerializeField] private Vector3 resetPosition = Vector3.zero;

        private float deltaTime = 0f;

        private void Start()
        {
            if (autoFindPlayer && player == null)
            {
                GameObject playerObj = GameObject.FindGameObjectWithTag("Player");
                if (playerObj != null)
                {
                    player = playerObj.transform;
                    playerRb = playerObj.GetComponent<Rigidbody2D>();
                }
            }
        }

        private void Update()
        {
            UpdatePlayerStats();
            UpdatePerformance();
        }

        private void UpdatePlayerStats()
        {
            if (player != null && positionText != null)
            {
                positionText.text = $"Position: ({player.position.x:F2}, {player.position.y:F2})";
            }

            if (playerRb != null && velocityText != null)
            {
                float speed = playerRb.linearVelocity.magnitude;
                velocityText.text = $"Velocity: {playerRb.linearVelocity:F2}\nSpeed: {speed:F2} m/s";
            }
        }

        private void UpdatePerformance()
        {
            if (fpsText == null) return;

            deltaTime += (Time.unscaledDeltaTime - deltaTime) * 0.1f;
            float fps = 1.0f / deltaTime;
            
            Color fpsColor = fps >= 60 ? Color.green : fps >= 30 ? Color.yellow : Color.red;
            fpsText.text = $"FPS: <color=#{ColorUtility.ToHtmlStringRGB(fpsColor)}>{fps:F0}</color>";
        }

        public void ResetPlayer()
        {
            if (player != null)
            {
                player.position = resetPosition;
                if (playerRb != null)
                {
                    playerRb.linearVelocity = Vector2.zero;
                    playerRb.angularVelocity = 0f;
                }
                Debug.Log("<color=cyan>[GYM] Player reset to spawn point</color>");
            }
        }

        public void TeleportPlayer(Vector3 position)
        {
            if (player != null)
            {
                player.position = position;
                if (playerRb != null)
                {
                    playerRb.linearVelocity = Vector2.zero;
                }
            }
        }

        public void SetResetPosition(Vector3 position)
        {
            resetPosition = position;
        }
    }
}
