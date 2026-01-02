using UnityEngine;

namespace TheHunt.Lighting
{
    public class LightZoneController : MonoBehaviour
    {
        [Header("Zone Settings")]
        [SerializeField] private BaseLightController[] zoneLights;
        [SerializeField] private bool findLightsAutomatically = true;
        [SerializeField] private bool activateOnStart = true;

        [Header("Group Control")]
        [SerializeField] private float intensityMultiplier = 1f;
        [SerializeField] private Color zoneColorTint = Color.white;

        [Header("Trigger Settings")]
        [SerializeField] private bool usePlayerTrigger = false;
        [SerializeField] private bool toggleOnTrigger = false;

        private bool isActive;

        private void Awake()
        {
            if (findLightsAutomatically)
            {
                zoneLights = GetComponentsInChildren<BaseLightController>();
            }
        }

        private void Start()
        {
            if (activateOnStart)
            {
                ActivateZone();
            }
            else
            {
                DeactivateZone();
            }
        }

        public void ActivateZone()
        {
            isActive = true;
            SetAllLightsState(true);
            ApplyZoneSettings();
            Debug.Log($"<color=green>[LIGHT ZONE] {gameObject.name} activated</color>");
        }

        public void DeactivateZone()
        {
            isActive = false;
            SetAllLightsState(false);
            Debug.Log($"<color=yellow>[LIGHT ZONE] {gameObject.name} deactivated</color>");
        }

        public void ToggleZone()
        {
            if (isActive)
            {
                DeactivateZone();
            }
            else
            {
                ActivateZone();
            }
        }

        private void SetAllLightsState(bool turnOn)
        {
            if (zoneLights == null) return;

            foreach (var light in zoneLights)
            {
                if (light == null) continue;

                if (turnOn)
                {
                    light.TurnOn();
                }
                else
                {
                    light.TurnOff();
                }
            }
        }

        private void ApplyZoneSettings()
        {
            if (zoneLights == null) return;

            foreach (var light in zoneLights)
            {
                if (light == null) continue;

                if (intensityMultiplier != 1f)
                {
                    float currentIntensity = light.GetComponent<UnityEngine.Rendering.Universal.Light2D>().intensity;
                    light.SetIntensity(currentIntensity * intensityMultiplier);
                }

                if (zoneColorTint != Color.white)
                {
                    light.SetColor(zoneColorTint);
                }
            }
        }

        public void SetZoneIntensity(float multiplier)
        {
            intensityMultiplier = multiplier;
            if (isActive)
            {
                ApplyZoneSettings();
            }
        }

        public void SetZoneColor(Color color)
        {
            zoneColorTint = color;
            if (isActive)
            {
                ApplyZoneSettings();
            }
        }

        private void OnTriggerEnter2D(Collider2D other)
        {
            if (!usePlayerTrigger) return;

            if (other.CompareTag("Player"))
            {
                if (toggleOnTrigger)
                {
                    ToggleZone();
                }
                else
                {
                    ActivateZone();
                }
            }
        }

        private void OnTriggerExit2D(Collider2D other)
        {
            if (!usePlayerTrigger) return;

            if (other.CompareTag("Player") && !toggleOnTrigger)
            {
                DeactivateZone();
            }
        }
    }
}
