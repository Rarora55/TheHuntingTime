using System.Collections.Generic;
using UnityEngine;

namespace TheHunt.Lighting
{
    public class LightManager : MonoBehaviour
    {
        [Header("Settings")]
        [SerializeField] private bool autoRegisterLights = true;
        [SerializeField] private int maxActiveLights = 15;
        [SerializeField] private bool useCulling = true;
        [SerializeField] private float cullingDistance = 20f;

        [Header("Global Controls")]
        [SerializeField] private bool globalLightsEnabled = true;
        [SerializeField] private float globalIntensityMultiplier = 1f;

        [Header("Debug")]
        [SerializeField] private bool showDebugInfo = false;

        private List<BaseLightController> registeredLights = new List<BaseLightController>();
        private UnityEngine.Camera mainCamera;
        private Transform cameraTransform;

        private static LightManager instance;
        public static LightManager Instance => instance;

        public int ActiveLightCount => registeredLights.Count;

        private void Awake()
        {
            if (instance != null && instance != this)
            {
                Destroy(gameObject);
                return;
            }

            instance = this;

            mainCamera = UnityEngine.Camera.main;
            if (mainCamera != null)
            {
                cameraTransform = mainCamera.transform;
            }

            if (autoRegisterLights)
            {
                RegisterAllLightsInScene();
            }
        }

        private void Start()
        {
            if (showDebugInfo)
            {
                Debug.Log($"<color=cyan>[LIGHT MANAGER] Initialized with {registeredLights.Count} lights</color>");
            }
        }

        private void Update()
        {
            if (useCulling && cameraTransform != null)
            {
                UpdateLightCulling();
            }
        }

        public void RegisterLight(BaseLightController light)
        {
            if (light == null || registeredLights.Contains(light))
                return;

            registeredLights.Add(light);

            if (showDebugInfo)
            {
                Debug.Log($"<color=green>[LIGHT MANAGER] Registered: {light.name}</color>");
            }
        }

        public void UnregisterLight(BaseLightController light)
        {
            if (light == null)
                return;

            registeredLights.Remove(light);

            if (showDebugInfo)
            {
                Debug.Log($"<color=yellow>[LIGHT MANAGER] Unregistered: {light.name}</color>");
            }
        }

        private void RegisterAllLightsInScene()
        {
            BaseLightController[] allLights = FindObjectsByType<BaseLightController>(FindObjectsSortMode.None);
            
            foreach (var light in allLights)
            {
                RegisterLight(light);
            }

            Debug.Log($"<color=cyan>[LIGHT MANAGER] Auto-registered {allLights.Length} lights in scene</color>");
        }

        private void UpdateLightCulling()
        {
            Vector3 cameraPos = cameraTransform.position;

            foreach (var light in registeredLights)
            {
                if (light == null) continue;

                float distance = Vector3.Distance(cameraPos, light.transform.position);
                bool shouldBeActive = distance <= cullingDistance;

                if (light.gameObject.activeSelf != shouldBeActive)
                {
                    light.gameObject.SetActive(shouldBeActive);
                }
            }
        }

        public void SetGlobalLightsEnabled(bool enabled)
        {
            globalLightsEnabled = enabled;

            foreach (var light in registeredLights)
            {
                if (light == null) continue;

                if (enabled)
                {
                    light.TurnOn();
                }
                else
                {
                    light.TurnOff();
                }
            }

            Debug.Log($"<color=cyan>[LIGHT MANAGER] Global lights {(enabled ? "ENABLED" : "DISABLED")}</color>");
        }

        public void SetGlobalIntensityMultiplier(float multiplier)
        {
            globalIntensityMultiplier = Mathf.Max(0f, multiplier);

            foreach (var light in registeredLights)
            {
                if (light == null) continue;

                float baseIntensity = light.GetBaseIntensity();
                light.SetIntensity(baseIntensity * globalIntensityMultiplier);
            }

            Debug.Log($"<color=cyan>[LIGHT MANAGER] Global intensity multiplier set to {globalIntensityMultiplier}</color>");
        }

        public void TurnOnAllLights()
        {
            foreach (var light in registeredLights)
            {
                if (light != null)
                {
                    light.TurnOn();
                }
            }
        }

        public void TurnOffAllLights()
        {
            foreach (var light in registeredLights)
            {
                if (light != null)
                {
                    light.TurnOff();
                }
            }
        }

        public List<BaseLightController> GetLightsInRadius(Vector3 position, float radius)
        {
            List<BaseLightController> lightsInRadius = new List<BaseLightController>();

            foreach (var light in registeredLights)
            {
                if (light == null) continue;

                float distance = Vector3.Distance(position, light.transform.position);
                if (distance <= radius)
                {
                    lightsInRadius.Add(light);
                }
            }

            return lightsInRadius;
        }

        private void OnDrawGizmosSelected()
        {
            if (!showDebugInfo || !useCulling || cameraTransform == null)
                return;

            Gizmos.color = Color.yellow;
            Gizmos.DrawWireSphere(cameraTransform.position, cullingDistance);
        }

        private void OnDestroy()
        {
            if (instance == this)
            {
                instance = null;
            }
        }
    }
}
