using UnityEngine;
using UnityEngine.Rendering.Universal;

namespace TheHunt.Lighting
{
    public class DayNightCycle : MonoBehaviour
    {
        [Header("Time Settings")]
        [SerializeField] private bool enableCycle = true;
        [SerializeField] private float cycleDuration = 300f;
        [SerializeField][Range(0f, 1f)] private float currentTime = 0.5f;
        [SerializeField] private float timeSpeed = 1f;

        [Header("Light References")]
        [SerializeField] private Light2D globalLight;
        [SerializeField] private bool controlGlobalLight = true;

        [Header("Day Settings")]
        [SerializeField] private Color dayColor = new Color(1f, 0.96f, 0.8f);
        [SerializeField] private float dayIntensity = 1f;

        [Header("Night Settings")]
        [SerializeField] private Color nightColor = new Color(0.3f, 0.4f, 0.6f);
        [SerializeField] private float nightIntensity = 0.3f;

        [Header("Events")]
        [SerializeField] private bool debugMode = false;

        private float timeOfDay;
        private bool isNight;

        public float CurrentTime => currentTime;
        public bool IsNight => isNight;
        public bool IsDay => !isNight;

        private void Start()
        {
            if (globalLight == null)
            {
                globalLight = FindFirstObjectByType<Light2D>();
            }

            if (globalLight == null)
            {
                Debug.LogError("<color=red>[DAY/NIGHT CYCLE] No Global Light found!</color>");
                enabled = false;
                return;
            }

            timeOfDay = currentTime * cycleDuration;
            UpdateLighting();
        }

        private void Update()
        {
            if (!enableCycle) return;

            timeOfDay += Time.deltaTime * timeSpeed;

            if (timeOfDay >= cycleDuration)
            {
                timeOfDay = 0f;
            }

            currentTime = timeOfDay / cycleDuration;
            UpdateLighting();
        }

        private void UpdateLighting()
        {
            if (globalLight == null || !controlGlobalLight) return;

            float normalizedTime = Mathf.PingPong(currentTime * 2f, 1f);

            Color targetColor = Color.Lerp(nightColor, dayColor, normalizedTime);
            float targetIntensity = Mathf.Lerp(nightIntensity, dayIntensity, normalizedTime);

            globalLight.color = targetColor;
            globalLight.intensity = targetIntensity;

            bool wasNight = isNight;
            isNight = currentTime > 0.75f || currentTime < 0.25f;

            if (wasNight != isNight && debugMode)
            {
                Debug.Log($"<color=cyan>[DAY/NIGHT] {(isNight ? "NIGHT" : "DAY")} has begun</color>");
            }
        }

        public void SetTimeOfDay(float time)
        {
            currentTime = Mathf.Clamp01(time);
            timeOfDay = currentTime * cycleDuration;
            UpdateLighting();
        }

        public void SetToDay()
        {
            SetTimeOfDay(0.5f);
        }

        public void SetToNight()
        {
            SetTimeOfDay(0f);
        }

        public void SetToDawn()
        {
            SetTimeOfDay(0.25f);
        }

        public void SetToDusk()
        {
            SetTimeOfDay(0.75f);
        }

        public void PauseCycle()
        {
            enableCycle = false;
        }

        public void ResumeCycle()
        {
            enableCycle = true;
        }

        public void SetCycleSpeed(float speed)
        {
            timeSpeed = Mathf.Max(0f, speed);
        }
    }
}
