using UnityEngine;
using UnityEngine.Rendering.Universal;

namespace TheHunt.Lighting
{
    public class LightZone : MonoBehaviour
    {
        [Header("Zone Settings")]
        [SerializeField] private string zoneName = "Light Zone";
        [SerializeField] private Color zoneColor = Color.white;
        [SerializeField] private float zoneIntensity = 1f;
        [SerializeField] private float transitionSpeed = 2f;

        [Header("Affected Lights")]
        [SerializeField] private bool affectAllLightsInZone = true;
        [SerializeField] private BaseLightController[] specificLights;

        [Header("Global Light Override")]
        [SerializeField] private bool overrideGlobalLight = false;
        [SerializeField] private Light2D globalLight;
        [SerializeField] private float globalLightIntensity = 1f;

        private Light2D playerGlobalLight;
        private float originalGlobalIntensity;
        private bool playerInZone = false;

        private void Start()
        {
            if (overrideGlobalLight && globalLight == null)
            {
                globalLight = FindFirstObjectByType<Light2D>();
            }

            if (globalLight != null)
            {
                originalGlobalIntensity = globalLight.intensity;
            }

            if (affectAllLightsInZone && (specificLights == null || specificLights.Length == 0))
            {
                specificLights = GetComponentsInChildren<BaseLightController>();
            }
        }

        private void Update()
        {
            if (playerInZone && globalLight != null && overrideGlobalLight)
            {
                globalLight.intensity = Mathf.Lerp(
                    globalLight.intensity,
                    globalLightIntensity,
                    Time.deltaTime * transitionSpeed
                );
            }
            else if (!playerInZone && globalLight != null && overrideGlobalLight)
            {
                globalLight.intensity = Mathf.Lerp(
                    globalLight.intensity,
                    originalGlobalIntensity,
                    Time.deltaTime * transitionSpeed
                );
            }
        }

        private void OnTriggerEnter2D(Collider2D other)
        {
            if (other.CompareTag("Player"))
            {
                playerInZone = true;
                ApplyZoneEffects();
                Debug.Log($"<color=cyan>[LIGHT ZONE] Player entered '{zoneName}'</color>");
            }
        }

        private void OnTriggerExit2D(Collider2D other)
        {
            if (other.CompareTag("Player"))
            {
                playerInZone = false;
                RemoveZoneEffects();
                Debug.Log($"<color=yellow>[LIGHT ZONE] Player exited '{zoneName}'</color>");
            }
        }

        private void ApplyZoneEffects()
        {
            if (specificLights == null) return;

            foreach (var light in specificLights)
            {
                if (light == null) continue;

                light.SetColor(zoneColor);
                light.SetIntensity(zoneIntensity);
            }
        }

        private void RemoveZoneEffects()
        {
            if (specificLights == null) return;

            foreach (var light in specificLights)
            {
                if (light == null) continue;

                float baseIntensity = light.GetBaseIntensity();
                light.SetIntensity(baseIntensity);
            }
        }

        public void SetZoneActive(bool active)
        {
            if (active)
            {
                ApplyZoneEffects();
            }
            else
            {
                RemoveZoneEffects();
            }
        }

        private void OnDrawGizmosSelected()
        {
            Gizmos.color = new Color(zoneColor.r, zoneColor.g, zoneColor.b, 0.3f);
            
            BoxCollider2D boxCollider = GetComponent<BoxCollider2D>();
            if (boxCollider != null)
            {
                Gizmos.matrix = transform.localToWorldMatrix;
                Gizmos.DrawCube(boxCollider.offset, boxCollider.size);
                Gizmos.DrawWireCube(boxCollider.offset, boxCollider.size);
            }
        }
    }
}
