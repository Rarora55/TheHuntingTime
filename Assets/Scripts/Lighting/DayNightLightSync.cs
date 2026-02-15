using UnityEngine;
using UnityEngine.Rendering.Universal;

namespace TheHunt.Lighting
{
    [RequireComponent(typeof(Light2D))]
    public class DayNightLightSync : MonoBehaviour
    {
        [Header("Referencias")]
        [SerializeField] private DayNightCycle dayNightCycle;
        [SerializeField] private Light2D targetLight;

        [Header("Configuración")]
        [SerializeField] private bool syncColor = true;
        [SerializeField] private bool syncIntensity = true;
        
        [Header("Multiplicadores")]
        [SerializeField] private float colorMultiplier = 1f;
        [SerializeField] private float intensityMultiplier = 1f;
        
        [Header("Offset/Ajustes")]
        [SerializeField] private Color colorTint = Color.white;
        [SerializeField] private float baseIntensity = 1f;

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
                Debug.LogWarning($"<color=yellow>[DAY/NIGHT SYNC] {gameObject.name} no encontró DayNightCycle. Desactivando sincronización.</color>");
                enabled = false;
            }
        }

        private void Update()
        {
            if (dayNightCycle == null || targetLight == null) return;

            if (syncColor)
            {
                UpdateColor();
            }

            if (syncIntensity)
            {
                UpdateIntensity();
            }
        }

        private void UpdateColor()
        {
            Light2D globalLight = dayNightCycle.GetComponent<Light2D>();
            if (globalLight == null) return;

            Color globalColor = globalLight.color;
            Color finalColor = globalColor * colorTint * colorMultiplier;
            
            targetLight.color = finalColor;
        }

        private void UpdateIntensity()
        {
            Light2D globalLight = dayNightCycle.GetComponent<Light2D>();
            if (globalLight == null) return;

            float globalIntensity = globalLight.intensity;
            float finalIntensity = (globalIntensity * intensityMultiplier) + baseIntensity;
            
            targetLight.intensity = Mathf.Max(0, finalIntensity);
        }

        private void OnValidate()
        {
            if (targetLight == null)
            {
                targetLight = GetComponent<Light2D>();
            }
        }
    }
}
