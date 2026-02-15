using UnityEngine;
using UnityEngine.Rendering.Universal;

namespace TheHunt.Lighting
{
    [RequireComponent(typeof(Light2D))]
    public class DayNightPeriodLight : MonoBehaviour
    {
        [Header("Referencias")]
        [SerializeField] private DayNightCycle dayNightCycle;
        [SerializeField] private Light2D targetLight;

        [Header("Configuración de Periodo")]
        [SerializeField] private DayPeriod activeDuringPeriod = DayPeriod.Dusk;
        [SerializeField] private bool useGameObjectActivation = false;

        [Header("Fade Settings")]
        [SerializeField] private float fadeInDuration = 2f;
        [SerializeField] private float fadeOutDuration = 2f;
        [SerializeField] private AnimationCurve fadeCurve = AnimationCurve.EaseInOut(0, 0, 1, 1);

        [Header("Intensidad")]
        [SerializeField] private float maxIntensity = 5f;
        [SerializeField] private float minIntensity = 0f;

        [Header("Debug")]
        [SerializeField] private bool showDebugLogs = false;

        private float currentIntensity = 0f;
        private float targetIntensity = 0f;
        private float fadeSpeed = 0f;
        private DayPeriod lastPeriod;
        private bool isInitialized = false;

        private void Awake()
        {
            if (targetLight == null)
            {
                targetLight = GetComponent<Light2D>();
            }

            if (dayNightCycle == null)
            {
                dayNightCycle = FindFirstObjectByType<DayNightCycle>();
            }

            if (dayNightCycle == null)
            {
                Debug.LogWarning($"<color=yellow>[PERIOD LIGHT] {gameObject.name} no encontró DayNightCycle.</color>");
                enabled = false;
                return;
            }
        }

        private void Start()
        {
            if (dayNightCycle != null)
            {
                dayNightCycle.PeriodChangedEvent.AddListener(OnPeriodChanged);
                lastPeriod = dayNightCycle.CurrentPeriod;
                
                maxIntensity = targetLight.intensity;
                
                if (ShouldBeActive(lastPeriod))
                {
                    currentIntensity = maxIntensity;
                    targetIntensity = maxIntensity;
                    targetLight.intensity = maxIntensity;
                }
                else
                {
                    currentIntensity = minIntensity;
                    targetIntensity = minIntensity;
                    targetLight.intensity = minIntensity;
                }

                isInitialized = true;
                
                if (showDebugLogs)
                {
                    Debug.Log($"<color=cyan>[PERIOD LIGHT] {gameObject.name} inicializado. Periodo: {lastPeriod}, Intensidad: {targetLight.intensity:F2}</color>");
                }
            }
        }

        private void OnDestroy()
        {
            if (dayNightCycle != null)
            {
                dayNightCycle.PeriodChangedEvent.RemoveListener(OnPeriodChanged);
            }
        }

        private void Update()
        {
            if (!isInitialized) return;

            if (Mathf.Abs(currentIntensity - targetIntensity) > 0.01f)
            {
                currentIntensity = Mathf.MoveTowards(currentIntensity, targetIntensity, fadeSpeed * Time.deltaTime);
                
                float t = Mathf.InverseLerp(minIntensity, maxIntensity, currentIntensity);
                float curvedIntensity = Mathf.Lerp(minIntensity, maxIntensity, fadeCurve.Evaluate(t));
                
                targetLight.intensity = curvedIntensity;
            }
        }

        private void OnPeriodChanged(DayPeriod newPeriod)
        {
            bool wasActive = ShouldBeActive(lastPeriod);
            bool shouldBeActive = ShouldBeActive(newPeriod);

            if (shouldBeActive && !wasActive)
            {
                FadeIn();
            }
            else if (!shouldBeActive && wasActive)
            {
                FadeOut();
            }

            lastPeriod = newPeriod;
        }

        private bool ShouldBeActive(DayPeriod period)
        {
            return period == activeDuringPeriod;
        }

        private void FadeIn()
        {
            targetIntensity = maxIntensity;
            fadeSpeed = (maxIntensity - minIntensity) / fadeInDuration;
            
            if (showDebugLogs)
            {
                Debug.Log($"<color=orange>[PERIOD LIGHT] {gameObject.name} → Fade IN (duración: {fadeInDuration}s, de {currentIntensity:F2} a {targetIntensity:F2})</color>");
            }
        }

        private void FadeOut()
        {
            targetIntensity = minIntensity;
            fadeSpeed = (maxIntensity - minIntensity) / fadeOutDuration;
            
            if (showDebugLogs)
            {
                Debug.Log($"<color=blue>[PERIOD LIGHT] {gameObject.name} → Fade OUT (duración: {fadeOutDuration}s, de {currentIntensity:F2} a {targetIntensity:F2})</color>");
            }
        }

        public void SetFadeDurations(float fadeIn, float fadeOut)
        {
            fadeInDuration = fadeIn;
            fadeOutDuration = fadeOut;
        }

        public void SetActivePeriod(DayPeriod period)
        {
            activeDuringPeriod = period;
        }

        private void OnValidate()
        {
            if (targetLight == null)
            {
                targetLight = GetComponent<Light2D>();
            }

            if (targetLight != null && Application.isPlaying && isInitialized)
            {
                maxIntensity = targetLight.intensity;
            }
        }
    }
}
