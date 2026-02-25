using UnityEngine;
using UnityEngine.Rendering.Universal;

namespace TheHunt.Lighting
{
    public class DarkZoneTrigger : MonoBehaviour
    {
        [Header("References")]
        [SerializeField] private Light2D globalLight;
        
        [Header("Light Settings")]
        [SerializeField] private float normalIntensity = 1f;
        [SerializeField] private float darkZoneIntensity = 0.1f;
        [SerializeField] private float transitionSpeed = 2f;
        
        private float targetIntensity;
        private bool playerInDarkZone = false;
        
        void Start()
        {
            if (globalLight == null)
            {
                globalLight = FindFirstObjectByType<Light2D>();
                Debug.LogWarning("<color=yellow>[DARK ZONE] GlobalLight not assigned, attempting to find automatically...</color>");
            }
            
            if (globalLight != null)
            {
                Debug.Log($"<color=green>[DARK ZONE] Initialized - GlobalLight found: {globalLight.name}, Normal Intensity: {normalIntensity}, Dark Intensity: {darkZoneIntensity}</color>");
            }
            else
            {
                Debug.LogError("<color=red>[DARK ZONE] Failed to find GlobalLight! Dark zone will not work.</color>");
            }
            
            targetIntensity = normalIntensity;
        }
        
        void Update()
        {
            if (globalLight != null)
            {
                globalLight.intensity = Mathf.Lerp(
                    globalLight.intensity, 
                    targetIntensity, 
                    Time.deltaTime * transitionSpeed
                );
            }
        }
        
        void OnTriggerEnter2D(Collider2D other)
        {
            Debug.Log($"<color=cyan>[DARK ZONE] OnTriggerEnter2D - Object: {other.gameObject.name}, Tag: {other.tag}</color>");
            
            if (other.CompareTag("Player"))
            {
                playerInDarkZone = true;
                targetIntensity = darkZoneIntensity;
                Debug.Log($"<color=yellow>[DARK ZONE] Player entered dark zone - Target intensity: {darkZoneIntensity}</color>");
            }
        }
        
        void OnTriggerExit2D(Collider2D other)
        {
            if (other.CompareTag("Player"))
            {
                playerInDarkZone = false;
                targetIntensity = normalIntensity;
                Debug.Log($"<color=green>[DARK ZONE] Player exited dark zone</color>");
            }
        }
    }
}
