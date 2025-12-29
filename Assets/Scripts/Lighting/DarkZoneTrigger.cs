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
            if (other.CompareTag("Player"))
            {
                playerInDarkZone = true;
                targetIntensity = darkZoneIntensity;
                Debug.Log($"<color=yellow>[DARK ZONE] Player entered dark zone</color>");
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
