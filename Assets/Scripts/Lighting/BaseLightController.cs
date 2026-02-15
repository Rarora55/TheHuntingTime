using UnityEngine;
using UnityEngine.Rendering.Universal;
using TheHunt.Events;

namespace TheHunt.Lighting
{
    public enum LightBehavior
    {
        Static,
        Flickering,
        Pulsating,
        Random
    }

    public class BaseLightController : MonoBehaviour
    {
        [Header("Events")]
        [SerializeField] private LightControlEvent onLightRegistered;
        [SerializeField] private LightControlEvent onLightUnregistered;

        [Header("References")]
        [SerializeField] protected Light2D light2D;

        [Header("Base Settings")]
        [SerializeField] protected bool startEnabled = true;
        [SerializeField] protected float baseIntensity = 1f;

        [Header("Behavior")]
        [SerializeField] protected LightBehavior behavior = LightBehavior.Static;
        [SerializeField] protected float flickerSpeed = 5f;
        [SerializeField] protected float flickerAmount = 0.2f;

        [Header("Performance")]
        [SerializeField] protected bool castsShadows = true;
        [SerializeField] protected int shadowQuality = 1;

        protected float currentIntensity;
        protected float timeOffset;

        protected virtual void Awake()
        {
            if (light2D == null)
            {
                light2D = GetComponent<Light2D>();
            }

            if (light2D == null)
            {
                Debug.LogError($"<color=red>[BASE LIGHT] No Light2D component found on {gameObject.name}!</color>");
                enabled = false;
                return;
            }

            timeOffset = Random.Range(0f, 100f);
            ConfigureLight();

            if (onLightRegistered != null)
            {
                onLightRegistered.Raise(this);
            }
        }

        protected virtual void OnDestroy()
        {
            if (onLightUnregistered != null)
            {
                onLightUnregistered.Raise(this);
            }
        }

        protected virtual void Start()
        {
            if (startEnabled)
            {
                TurnOn();
            }
            else
            {
                TurnOff();
            }
        }

        protected virtual void Update()
        {
            if (!light2D.enabled) return;

            switch (behavior)
            {
                case LightBehavior.Flickering:
                    UpdateFlickering();
                    break;
                case LightBehavior.Pulsating:
                    UpdatePulsating();
                    break;
                case LightBehavior.Random:
                    UpdateRandom();
                    break;
            }
        }

        protected virtual void ConfigureLight()
        {
            currentIntensity = baseIntensity;
            light2D.intensity = currentIntensity;
        }

        protected virtual void UpdateFlickering()
        {
            float flicker = Mathf.PerlinNoise(Time.time * flickerSpeed + timeOffset, 0f);
            flicker = (flicker - 0.5f) * 2f * flickerAmount;
            light2D.intensity = baseIntensity + flicker;
        }

        protected virtual void UpdatePulsating()
        {
            float pulse = Mathf.Sin(Time.time * flickerSpeed + timeOffset) * flickerAmount;
            light2D.intensity = baseIntensity + pulse;
        }

        protected virtual void UpdateRandom()
        {
            if (Random.value < 0.05f * Time.deltaTime * flickerSpeed)
            {
                float randomIntensity = baseIntensity + Random.Range(-flickerAmount, flickerAmount);
                light2D.intensity = randomIntensity;
            }
        }

        public virtual void TurnOn()
        {
            if (light2D != null)
            {
                light2D.enabled = true;
                light2D.intensity = baseIntensity;
            }
        }

        public virtual void TurnOff()
        {
            if (light2D != null)
            {
                light2D.enabled = false;
            }
        }

        public virtual void SetIntensity(float intensity)
        {
            baseIntensity = intensity;
            if (light2D != null && behavior == LightBehavior.Static)
            {
                light2D.intensity = baseIntensity;
            }
        }

        public virtual void SetColor(Color color)
        {
            if (light2D != null)
            {
                light2D.color = color;
            }
        }

        public float GetBaseIntensity()
        {
            return baseIntensity;
        }

        public Light2D GetLight2D()
        {
            return light2D;
        }

        private void OnValidate()
        {
            if (light2D == null)
            {
                light2D = GetComponent<Light2D>();
            }
        }
    }
}
