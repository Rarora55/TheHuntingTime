using UnityEngine;
using UnityEngine.Rendering.Universal;

namespace TheHunt.Lighting
{
    public enum AdvancedEffect
    {
        None,
        Thunder,
        Emergency,
        Strobe,
        ColorCycle
    }

    public class AdvancedLightEffects : BaseLightController
    {
        [Header("Advanced Effects")]
        [SerializeField] private AdvancedEffect advancedEffect = AdvancedEffect.None;

        [Header("Thunder Settings")]
        [SerializeField] private float thunderMinInterval = 3f;
        [SerializeField] private float thunderMaxInterval = 10f;
        [SerializeField] private int thunderFlashCount = 3;
        [SerializeField] private float thunderFlashDuration = 0.1f;

        [Header("Emergency Settings")]
        [SerializeField] private float emergencyBlinkSpeed = 2f;
        [SerializeField] private Color emergencyColor = Color.red;

        [Header("Strobe Settings")]
        [SerializeField] private float strobeSpeed = 10f;

        [Header("Color Cycle Settings")]
        [SerializeField] private Color[] cycleColors = { Color.red, Color.green, Color.blue };
        [SerializeField] private float colorCycleSpeed = 1f;

        private float effectTimer;
        private int currentFlashCount;
        private bool isFlashing;
        private int currentColorIndex;
        private Color originalColor;

        protected override void Awake()
        {
            base.Awake();

            if (light2D != null)
            {
                originalColor = light2D.color;
            }
        }

        protected override void Update()
        {
            base.Update();

            if (!light2D.enabled) return;

            switch (advancedEffect)
            {
                case AdvancedEffect.Thunder:
                    UpdateThunderEffect();
                    break;
                case AdvancedEffect.Emergency:
                    UpdateEmergencyEffect();
                    break;
                case AdvancedEffect.Strobe:
                    UpdateStrobeEffect();
                    break;
                case AdvancedEffect.ColorCycle:
                    UpdateColorCycleEffect();
                    break;
            }
        }

        private void UpdateThunderEffect()
        {
            effectTimer += Time.deltaTime;

            if (!isFlashing)
            {
                float nextThunderTime = Random.Range(thunderMinInterval, thunderMaxInterval);

                if (effectTimer >= nextThunderTime)
                {
                    StartThunder();
                }
            }
            else
            {
                if (effectTimer >= thunderFlashDuration)
                {
                    light2D.enabled = !light2D.enabled;
                    effectTimer = 0f;
                    currentFlashCount++;

                    if (currentFlashCount >= thunderFlashCount * 2)
                    {
                        light2D.enabled = true;
                        isFlashing = false;
                        currentFlashCount = 0;
                        effectTimer = 0f;
                    }
                }
            }
        }

        private void StartThunder()
        {
            isFlashing = true;
            currentFlashCount = 0;
            effectTimer = 0f;
            Debug.Log("<color=yellow>[ADVANCED LIGHT] Thunder strike!</color>");
        }

        private void UpdateEmergencyEffect()
        {
            float value = Mathf.PingPong(Time.time * emergencyBlinkSpeed, 1f);
            light2D.color = Color.Lerp(originalColor, emergencyColor, value);
            light2D.intensity = baseIntensity * (0.5f + value * 0.5f);
        }

        private void UpdateStrobeEffect()
        {
            float value = Mathf.PingPong(Time.time * strobeSpeed, 1f);
            light2D.enabled = value > 0.5f;
        }

        private void UpdateColorCycleEffect()
        {
            if (cycleColors == null || cycleColors.Length == 0)
                return;

            effectTimer += Time.deltaTime * colorCycleSpeed;

            if (effectTimer >= 1f)
            {
                effectTimer = 0f;
                currentColorIndex = (currentColorIndex + 1) % cycleColors.Length;
            }

            int nextColorIndex = (currentColorIndex + 1) % cycleColors.Length;
            light2D.color = Color.Lerp(
                cycleColors[currentColorIndex],
                cycleColors[nextColorIndex],
                effectTimer
            );
        }

        public void SetAdvancedEffect(AdvancedEffect effect)
        {
            advancedEffect = effect;
            effectTimer = 0f;
            isFlashing = false;
            currentFlashCount = 0;

            if (light2D != null)
            {
                light2D.color = originalColor;
                light2D.enabled = true;
            }
        }

        public void TriggerThunder()
        {
            if (advancedEffect == AdvancedEffect.Thunder && !isFlashing)
            {
                StartThunder();
            }
        }
    }
}
