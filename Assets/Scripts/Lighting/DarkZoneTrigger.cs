using UnityEngine;
using UnityEngine.Rendering.Universal;

namespace TheHunt.Lighting
{
    public class DarkZoneTrigger : MonoBehaviour
    {
        [Header("References")]
        [SerializeField] private Light2D globalLight;

        [Header("Dark Zone Settings")]
        [Tooltip("Amount of intensity subtracted from the current global light when the player enters this zone.")]
        [SerializeField] private float intensityReduction = 0.8f;
        [SerializeField] private float transitionSpeed = 2f;

        private DayNightCycle dayNightCycle;
        private float normalIntensity;
        private float targetIntensity;
        private bool useCycleFallback;

        private void Start()
        {
            dayNightCycle = FindFirstObjectByType<DayNightCycle>();

            if (dayNightCycle != null)
            {
                // DayNightCycle owns globalLight.intensity — inject via offset.
                useCycleFallback = false;
                return;
            }

            // No DayNightCycle present: control globalLight directly.
            useCycleFallback = true;
            if (globalLight == null)
                globalLight = FindFirstObjectByType<Light2D>();

            if (globalLight == null)
            {
                Debug.LogError($"[DarkZoneTrigger] No Light2D or DayNightCycle found on '{name}'. Dark zone will not work.");
                return;
            }

            normalIntensity = globalLight.intensity;
            targetIntensity = normalIntensity;
        }

        private void Update()
        {
            if (!useCycleFallback || globalLight == null) return;

            globalLight.intensity = Mathf.Lerp(
                globalLight.intensity,
                targetIntensity,
                Time.deltaTime * transitionSpeed
            );
        }

        private void OnTriggerEnter2D(Collider2D other)
        {
            if (!other.CompareTag("Player")) return;

            if (!useCycleFallback)
            {
                dayNightCycle.SetDarkZoneOffset(intensityReduction, transitionSpeed);
            }
            else if (globalLight != null)
            {
                normalIntensity = globalLight.intensity;
                targetIntensity = Mathf.Max(0f, normalIntensity - intensityReduction);
            }
        }

        private void OnTriggerExit2D(Collider2D other)
        {
            if (!other.CompareTag("Player")) return;

            if (!useCycleFallback)
            {
                dayNightCycle.SetDarkZoneOffset(0f, transitionSpeed);
            }
            else if (globalLight != null)
            {
                targetIntensity = normalIntensity;
            }
        }

        private void OnDrawGizmosSelected()
        {
            Gizmos.color = new Color(0.05f, 0.05f, 0.2f, 0.4f);
            BoxCollider2D box = GetComponent<BoxCollider2D>();
            if (box == null) return;

            Gizmos.matrix = transform.localToWorldMatrix;
            Gizmos.DrawCube(box.offset, box.size);
            Gizmos.DrawWireCube(box.offset, box.size);
        }
    }
}
