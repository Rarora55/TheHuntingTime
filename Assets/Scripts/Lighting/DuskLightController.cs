using UnityEngine;
using UnityEngine.Rendering.Universal;

namespace TheHunt.Lighting
{
    /// <summary>
    /// Controls the intensity of a Light2D (DuskLight) based on the current progress
    /// within the Dusk period, as reported by DayNightCycle.
    ///
    /// Intensity envelope:
    ///   [0%  – fadeInEnd%]   of Dusk  →  0 to maxIntensity  (fade in)
    ///   [fadeInEnd% – fadeOutStart%]   →  maxIntensity       (stable plateau)
    ///   [fadeOutStart% – 100%] of Dusk →  maxIntensity to 0  (fade out)
    ///
    /// All thresholds and intensity values are configurable via the Inspector.
    /// </summary>
    [RequireComponent(typeof(Light2D))]
    public class DuskLightController : MonoBehaviour
    {
        [Header("References")]
        [SerializeField] private DayNightCycle dayNightCycle;
        [SerializeField] private Light2D targetLight;

        [Header("Dusk Period Thresholds (fraction within the Dusk window, 0 to 1)")]
        [Tooltip("Fraction of the Dusk period at which fade-in completes. Default 0.2 = 20%.")]
        [Range(0f, 1f)]
        [SerializeField] private float fadeInEnd = 0.2f;

        [Tooltip("Fraction of the Dusk period at which fade-out begins. Default 0.8 = 80%.")]
        [Range(0f, 1f)]
        [SerializeField] private float fadeOutStart = 0.8f;

        [Header("Intensity")]
        [Tooltip("Peak intensity held between fadeInEnd and fadeOutStart.")]
        [SerializeField] private float maxIntensity = 4f;

        [Header("Curves")]
        [SerializeField] private AnimationCurve fadeInCurve = AnimationCurve.EaseInOut(0f, 0f, 1f, 1f);
        [SerializeField] private AnimationCurve fadeOutCurve = AnimationCurve.EaseInOut(0f, 1f, 1f, 0f);

        [Header("Debug")]
        [SerializeField] private bool showDebugLogs = false;

        private float duskStart;
        private float nightStart;

        private void Awake()
        {
            if (targetLight == null)
                targetLight = GetComponent<Light2D>();

            if (dayNightCycle == null)
                dayNightCycle = FindFirstObjectByType<DayNightCycle>();

            if (dayNightCycle == null)
            {
                Debug.LogError($"<color=red>[DUSK LIGHT] {gameObject.name} could not find DayNightCycle.</color>");
                enabled = false;
            }
        }

        private void Start()
        {
            if (dayNightCycle == null) return;

            duskStart = dayNightCycle.DuskStart;
            nightStart = dayNightCycle.NightStart;

            dayNightCycle.OnTimeChanged.AddListener(HandleTimeChanged);

            // Apply the correct intensity immediately based on the current time.
            HandleTimeChanged(dayNightCycle.CurrentTime);

            if (showDebugLogs)
                Debug.Log($"<color=cyan>[DUSK LIGHT] Initialized. Dusk window: [{duskStart:F3} – {nightStart:F3}]</color>");
        }

        private void OnDestroy()
        {
            if (dayNightCycle != null)
                dayNightCycle.OnTimeChanged.RemoveListener(HandleTimeChanged);
        }

        /// <summary>
        /// Receives the normalised cycle time [0,1] every frame from DayNightCycle
        /// and updates the DuskLight intensity accordingly.
        /// </summary>
        private void HandleTimeChanged(float currentTime)
        {
            if (targetLight == null) return;

            float duskDuration = nightStart - duskStart;

            if (duskDuration <= 0f || currentTime < duskStart || currentTime >= nightStart)
            {
                targetLight.intensity = 0f;
                return;
            }

            float duskProgress = (currentTime - duskStart) / duskDuration;
            float intensity = CalculateIntensity(duskProgress);
            targetLight.intensity = intensity;

            if (showDebugLogs && Time.frameCount % 30 == 0)
                Debug.Log($"<color=gray>[DUSK LIGHT] progress={duskProgress:F3}  intensity={intensity:F3}</color>");
        }

        /// <summary>
        /// Maps a dusk progress value [0,1] to a light intensity following the three-segment envelope.
        /// </summary>
        private float CalculateIntensity(float duskProgress)
        {
            if (duskProgress <= fadeInEnd)
            {
                float t = fadeInEnd > 0f ? duskProgress / fadeInEnd : 1f;
                return maxIntensity * fadeInCurve.Evaluate(t);
            }

            if (duskProgress >= fadeOutStart)
            {
                float denominator = 1f - fadeOutStart;
                float t = denominator > 0f ? (duskProgress - fadeOutStart) / denominator : 1f;
                return maxIntensity * fadeOutCurve.Evaluate(t);
            }

            return maxIntensity;
        }

#if UNITY_EDITOR
        private void OnValidate()
        {
            if (targetLight == null)
                targetLight = GetComponent<Light2D>();

            if (fadeInEnd > fadeOutStart)
                fadeInEnd = fadeOutStart;
        }
#endif
    }
}
