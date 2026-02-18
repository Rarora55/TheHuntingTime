using UnityEngine;
using UnityEngine.Rendering.Universal;
using TheHunt.Events;

namespace TheHunt.Lighting
{
    public class DayNightCycle : MonoBehaviour
    {
        [Header("Events")]
        [SerializeField] private GlobalLightCommandEvent onGlobalLightCommand;

        [Header("Time Settings")]
        [SerializeField] private bool enableCycle = true;
        [SerializeField] private float cycleDuration = 300f;
        [SerializeField][Range(0f, 1f)] private float currentTime = 0.5f;
        [SerializeField] private float timeSpeed = 1f;

        [Header("Light References")]
        [SerializeField] private Light2D globalLight;
        [SerializeField] private bool controlGlobalLight = true;

        [Header("Day Settings")]
        [SerializeField] private Gradient dayNightGradient;
        [SerializeField] private AnimationCurve intensityCurve;
        [SerializeField] private Color dayColor = new Color(1f, 0.96f, 0.8f);
        [SerializeField] private float dayIntensity = 1f;

        [Header("Transition Settings")]
        [SerializeField] private Color dawnColor = new Color(1f, 0.7f, 0.5f);
        [SerializeField] private Color duskColor = new Color(0.9f, 0.5f, 0.3f);
        [SerializeField] private float transitionDuration = 0.2f;
        [SerializeField] private float colorTransitionSpeed = 2f;
        [SerializeField] private float intensityTransitionSpeed = 2f;

        [Header("Night Settings")]
        [SerializeField] private Color nightColor = new Color(0.3f, 0.4f, 0.6f);
        [SerializeField] private float nightIntensity = 0.3f;

        [Header("Day Period Thresholds")]
        [SerializeField][Range(0f, 1f)] private float dawnStart = 0.2f;
        [SerializeField][Range(0f, 1f)] private float dayStart = 0.3f;
        [SerializeField][Range(0f, 1f)] private float duskStart = 0.7f;
        [SerializeField][Range(0f, 1f)] private float nightStart = 0.8f;

        [Header("Events")]
        [SerializeField] private bool debugMode = false;
        [SerializeField] private DayPeriodEvent onPeriodChanged;
        [SerializeField] private TimeChangedEvent onTimeChanged;

        [Header("Auto-Control Lights")]
        [SerializeField] private bool autoControlArtificialLights = true;

        private float timeOfDay;
        private bool isNight;
        private DayPeriod currentPeriod;
        private DayPeriod previousPeriod;
        
        private Color currentLightColor;
        private float currentLightIntensity;
        private Color targetLightColor;
        private float targetLightIntensity;

        public float CurrentTime => currentTime;
        public bool IsNight => isNight;
        public bool IsDay => !isNight;
        public DayPeriod CurrentPeriod => currentPeriod;
        public float Hour => currentTime * 24f;
        public DayPeriodEvent PeriodChangedEvent => onPeriodChanged;

        private void Awake()
        {
            InitializeGradient();
            InitializeCurve();
        }

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
            currentPeriod = GetDayPeriod(currentTime);
            previousPeriod = currentPeriod;
            
            // Inicializar color e intensidad actuales
            currentLightColor = globalLight.color;
            currentLightIntensity = globalLight.intensity;
            
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
            UpdateDayPeriod();
            
            onTimeChanged?.Invoke(currentTime);
        }

        private void UpdateLighting()
        {
            if (globalLight == null || !controlGlobalLight) return;

            if (dayNightGradient != null && dayNightGradient.colorKeys.Length > 0)
            {
                targetLightColor = dayNightGradient.Evaluate(currentTime);
            }
            else
            {
                targetLightColor = GetColorForTime(currentTime);
            }

            if (intensityCurve != null && intensityCurve.keys.Length > 0)
            {
                targetLightIntensity = intensityCurve.Evaluate(currentTime);
            }
            else
            {
                targetLightIntensity = GetIntensityForTime(currentTime);
            }

            // Transición suave de color e intensidad
            currentLightColor = Color.Lerp(currentLightColor, targetLightColor, Time.deltaTime * colorTransitionSpeed);
            currentLightIntensity = Mathf.Lerp(currentLightIntensity, targetLightIntensity, Time.deltaTime * intensityTransitionSpeed);

            globalLight.color = currentLightColor;
            globalLight.intensity = currentLightIntensity;

            bool wasNight = isNight;
            isNight = currentTime >= nightStart || currentTime < dawnStart;

            if (wasNight != isNight && debugMode)
            {
                Debug.Log($"<color=cyan>[DAY/NIGHT] {(isNight ? "NIGHT" : "DAY")} has begun</color>");
            }
        }

        private Color GetColorForTime(float time)
        {
            if (time >= nightStart || time < dawnStart)
            {
                return nightColor;
            }
            else if (time >= dawnStart && time < dayStart)
            {
                float t = (time - dawnStart) / (dayStart - dawnStart);
                return Color.Lerp(dawnColor, dayColor, t);
            }
            else if (time >= dayStart && time < duskStart)
            {
                return dayColor;
            }
            else if (time >= duskStart && time < nightStart)
            {
                float t = (time - duskStart) / (nightStart - duskStart);
                return Color.Lerp(duskColor, nightColor, t);
            }
            
            return dayColor;
        }

        private float GetIntensityForTime(float time)
        {
            if (time >= nightStart || time < dawnStart)
            {
                return nightIntensity;
            }
            else if (time >= dawnStart && time < dayStart)
            {
                float t = (time - dawnStart) / (dayStart - dawnStart);
                return Mathf.Lerp(nightIntensity, dayIntensity, t);
            }
            else if (time >= dayStart && time < duskStart)
            {
                return dayIntensity;
            }
            else if (time >= duskStart && time < nightStart)
            {
                float t = (time - duskStart) / (nightStart - duskStart);
                return Mathf.Lerp(dayIntensity, nightIntensity, t);
            }
            
            return dayIntensity;
        }

        private void UpdateDayPeriod()
        {
            currentPeriod = GetDayPeriod(currentTime);

            if (currentPeriod != previousPeriod)
            {
                OnPeriodChanged(currentPeriod);
                previousPeriod = currentPeriod;
            }
        }

        private DayPeriod GetDayPeriod(float time)
        {
            if (time >= nightStart || time < dawnStart)
            {
                return DayPeriod.Night;
            }
            else if (time >= dawnStart && time < dayStart)
            {
                return DayPeriod.Dawn;
            }
            else if (time >= dayStart && time < duskStart)
            {
                return DayPeriod.Day;
            }
            else
            {
                return DayPeriod.Dusk;
            }
        }

        private void OnPeriodChanged(DayPeriod newPeriod)
        {
            if (debugMode)
            {
                Debug.Log($"<color=yellow>[DAY/NIGHT] Period changed to: {newPeriod}</color>");
            }

            onPeriodChanged?.Invoke(newPeriod);

            if (autoControlArtificialLights && onGlobalLightCommand != null)
            {
                bool shouldLightsBeOn = newPeriod == DayPeriod.Dusk || newPeriod == DayPeriod.Night;
                
                LightCommand command = shouldLightsBeOn ? LightCommand.TurnOnAll : LightCommand.TurnOffAll;
                onGlobalLightCommand.Raise(command);
            }
        }

        private void InitializeGradient()
        {
            // Forzar reinicialización si el gradiente es null, tiene pocos colores, o todos son blancos
            bool needsInit = dayNightGradient == null || 
                           dayNightGradient.colorKeys.Length <= 2 ||
                           (dayNightGradient.colorKeys.Length == 2 && 
                            dayNightGradient.colorKeys[0].color == Color.white);
            
            if (needsInit)
            {
                dayNightGradient = new Gradient();
                
                GradientColorKey[] colorKeys = new GradientColorKey[8];
                colorKeys[0] = new GradientColorKey(new Color(0.05f, 0.05f, 0.15f), 0f);
                colorKeys[1] = new GradientColorKey(new Color(0.8f, 0.4f, 0.3f), 0.25f);
                colorKeys[2] = new GradientColorKey(new Color(1f, 0.9f, 0.7f), 0.375f);
                colorKeys[3] = new GradientColorKey(new Color(1f, 0.98f, 0.9f), 0.5f);
                colorKeys[4] = new GradientColorKey(new Color(1f, 0.95f, 0.8f), 0.67f);
                colorKeys[5] = new GradientColorKey(new Color(1f, 0.4f, 0.2f), 0.8125f);
                colorKeys[6] = new GradientColorKey(new Color(0.15f, 0.1f, 0.25f), 0.92f);
                colorKeys[7] = new GradientColorKey(new Color(0.05f, 0.05f, 0.15f), 1f);

                GradientAlphaKey[] alphaKeys = new GradientAlphaKey[2];
                alphaKeys[0] = new GradientAlphaKey(1f, 0f);
                alphaKeys[1] = new GradientAlphaKey(1f, 1f);

                dayNightGradient.SetKeys(colorKeys, alphaKeys);
                dayNightGradient.mode = GradientMode.Blend;
                
                if (debugMode)
                {
                    Debug.Log("<color=green>[DAY/NIGHT] Gradiente inicializado con 8 puntos de color (Unity max)</color>");
                }
            }
        }

        private void InitializeCurve()
        {
            if (intensityCurve == null || intensityCurve.keys.Length == 0)
            {
                intensityCurve = AnimationCurve.EaseInOut(0f, nightIntensity, 1f, nightIntensity);
                intensityCurve.AddKey(new Keyframe(0.25f, dayIntensity));
                intensityCurve.AddKey(new Keyframe(0.5f, dayIntensity));
                intensityCurve.AddKey(new Keyframe(0.75f, dayIntensity));
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

        public string GetFormattedTime()
        {
            float hour = currentTime * 24f;
            int hours = Mathf.FloorToInt(hour);
            int minutes = Mathf.FloorToInt((hour - hours) * 60f);
            return $"{hours:D2}:{minutes:D2}";
        }

        public void SkipToNextPeriod()
        {
            DayPeriod nextPeriod = (DayPeriod)(((int)currentPeriod + 1) % 4);
            
            switch (nextPeriod)
            {
                case DayPeriod.Dawn:
                    SetTimeOfDay(dawnStart);
                    break;
                case DayPeriod.Day:
                    SetTimeOfDay(dayStart);
                    break;
                case DayPeriod.Dusk:
                    SetTimeOfDay(duskStart);
                    break;
                case DayPeriod.Night:
                    SetTimeOfDay(nightStart);
                    break;
            }
        }
    }
}
