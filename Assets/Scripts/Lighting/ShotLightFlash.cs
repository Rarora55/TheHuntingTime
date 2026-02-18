using UnityEngine;
using UnityEngine.Rendering.Universal;

namespace TheHunt.Lighting
{
    public class ShotLightFlash : MonoBehaviour
    {
        [Header("Referencias")]
        [SerializeField] private Light2D shotLight;

        [Header("Configuración de Intensidad")]
        [SerializeField] private float maxIntensity = 4.42f;
        [SerializeField] private float fadeOutSpeed = 20f;

        [Header("Configuración de Timing")]
        [SerializeField] private float flashDelay = 0f;
        [SerializeField] private float flashDuration = 0.05f;

        [Header("Debug")]
        [SerializeField] private bool showDebugLogs = false;

        private float currentIntensity = 0f;
        private bool isFlashing = false;
        private float flashTimer = 0f;

        private void Awake()
        {
            if (shotLight == null)
            {
                shotLight = GetComponent<Light2D>();
            }

            if (shotLight == null)
            {
                Debug.LogError("<color=red>[SHOT LIGHT] No se encontró Light2D en el GameObject!</color>", this);
                enabled = false;
                return;
            }

            currentIntensity = 0f;
            shotLight.intensity = 0f;
        }

        private void Update()
        {
            if (isFlashing)
            {
                flashTimer += Time.deltaTime;

                if (flashTimer >= flashDelay)
                {
                    if (flashTimer < flashDelay + flashDuration)
                    {
                        // Flash rápido a máxima intensidad
                        currentIntensity = maxIntensity;
                    }
                    else
                    {
                        // Fade out gradual
                        currentIntensity = Mathf.MoveTowards(currentIntensity, 0f, fadeOutSpeed * Time.deltaTime);

                        if (currentIntensity <= 0f)
                        {
                            isFlashing = false;
                            currentIntensity = 0f;

                            if (showDebugLogs)
                            {
                                Debug.Log("<color=blue>[SHOT LIGHT] Flash completado</color>");
                            }
                        }
                    }
                }

                shotLight.intensity = currentIntensity;
            }
        }

        public void TriggerFlash()
        {
            isFlashing = true;
            flashTimer = 0f;
            currentIntensity = 0f;

            if (showDebugLogs)
            {
                Debug.Log($"<color=yellow>[SHOT LIGHT] Flash activado - MaxIntensity: {maxIntensity}, FadeSpeed: {fadeOutSpeed}, Delay: {flashDelay}s</color>");
            }
        }

        public void TriggerFlash(float customIntensity)
        {
            float originalIntensity = maxIntensity;
            maxIntensity = customIntensity;
            TriggerFlash();
            maxIntensity = originalIntensity;
        }

        public void SetMaxIntensity(float intensity)
        {
            maxIntensity = intensity;
        }

        public void SetFadeOutSpeed(float speed)
        {
            fadeOutSpeed = speed;
        }

        public void SetFlashDelay(float delay)
        {
            flashDelay = delay;
        }

        public void SetFlashDuration(float duration)
        {
            flashDuration = duration;
        }
    }
}
