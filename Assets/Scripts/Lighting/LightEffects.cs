using UnityEngine;
using UnityEngine.Rendering.Universal;

namespace TheHunt.Lighting
{
    public class LightEffects : MonoBehaviour
    {
        [Header("References")]
        [SerializeField] private Light2D light2D;

        [Header("Fade Settings")]
        [SerializeField] private bool fadeOnStart = false;
        [SerializeField] private float fadeDuration = 1f;

        [Header("Distance Fade")]
        [SerializeField] private bool fadeByDistance = false;
        [SerializeField] private Transform playerTransform;
        [SerializeField] private float maxDistance = 10f;
        [SerializeField] private float minIntensity = 0f;
        [SerializeField] private float maxIntensity = 1f;

        private float originalIntensity;
        private float targetIntensity;
        private float fadeTimer;
        private bool isFading;

        private void Awake()
        {
            if (light2D == null)
            {
                light2D = GetComponent<Light2D>();
            }

            originalIntensity = light2D != null ? light2D.intensity : 1f;
        }

        private void Start()
        {
            if (fadeOnStart)
            {
                FadeIn(fadeDuration);
            }

            if (fadeByDistance && playerTransform == null)
            {
                GameObject player = GameObject.FindGameObjectWithTag("Player");
                if (player != null)
                {
                    playerTransform = player.transform;
                }
            }
        }

        private void Update()
        {
            if (isFading)
            {
                UpdateFade();
            }

            if (fadeByDistance && playerTransform != null)
            {
                UpdateDistanceFade();
            }
        }

        public void FadeIn(float duration)
        {
            if (light2D == null) return;

            targetIntensity = originalIntensity;
            light2D.intensity = 0f;
            fadeDuration = duration;
            fadeTimer = 0f;
            isFading = true;

            Debug.Log($"<color=cyan>[LIGHT EFFECTS] Fading in over {duration}s</color>");
        }

        public void FadeOut(float duration)
        {
            if (light2D == null) return;

            targetIntensity = 0f;
            fadeDuration = duration;
            fadeTimer = 0f;
            isFading = true;

            Debug.Log($"<color=cyan>[LIGHT EFFECTS] Fading out over {duration}s</color>");
        }

        public void FadeTo(float intensity, float duration)
        {
            if (light2D == null) return;

            targetIntensity = intensity;
            fadeDuration = duration;
            fadeTimer = 0f;
            isFading = true;

            Debug.Log($"<color=cyan>[LIGHT EFFECTS] Fading to {intensity} over {duration}s</color>");
        }

        private void UpdateFade()
        {
            if (light2D == null)
            {
                isFading = false;
                return;
            }

            fadeTimer += Time.deltaTime;
            float progress = Mathf.Clamp01(fadeTimer / fadeDuration);

            light2D.intensity = Mathf.Lerp(light2D.intensity, targetIntensity, progress);

            if (progress >= 1f)
            {
                light2D.intensity = targetIntensity;
                isFading = false;
            }
        }

        private void UpdateDistanceFade()
        {
            if (light2D == null || playerTransform == null) return;

            float distance = Vector2.Distance(transform.position, playerTransform.position);
            float normalizedDistance = Mathf.Clamp01(distance / maxDistance);

            float targetIntensity = Mathf.Lerp(maxIntensity, minIntensity, normalizedDistance);
            light2D.intensity = Mathf.Lerp(light2D.intensity, targetIntensity, Time.deltaTime * 2f);
        }

        public void SetOriginalIntensity(float intensity)
        {
            originalIntensity = intensity;
        }

        public void ResetToOriginal()
        {
            if (light2D != null)
            {
                light2D.intensity = originalIntensity;
            }
        }
    }
}
