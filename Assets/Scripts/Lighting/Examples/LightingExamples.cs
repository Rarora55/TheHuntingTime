using UnityEngine;

namespace TheHunt.Lighting.Examples
{
    public class LightingExamples : MonoBehaviour
    {
        [Header("Example References")]
        [SerializeField] private BaseLightController singleLight;
        [SerializeField] private LightZoneController lightZone;
        [SerializeField] private InteractiveLightSwitch lightSwitch;
        [SerializeField] private LightEffects lightEffects;

        private void Start()
        {
            DemonstrateLightControl();
        }

        private void Update()
        {
            HandleKeyboardInput();
        }

        private void DemonstrateLightControl()
        {
            Debug.Log("<color=cyan>===== LIGHTING SYSTEM EXAMPLES =====</color>");
            Debug.Log("<color=yellow>Press keys to test lighting features:</color>");
            Debug.Log("1 - Turn light ON");
            Debug.Log("2 - Turn light OFF");
            Debug.Log("3 - Fade light IN (2s)");
            Debug.Log("4 - Fade light OUT (2s)");
            Debug.Log("5 - Activate zone");
            Debug.Log("6 - Deactivate zone");
            Debug.Log("7 - Toggle switch");
            Debug.Log("8 - Random color");
        }

        private void HandleKeyboardInput()
        {
            if (UnityEngine.Input.GetKeyDown(KeyCode.Alpha1))
            {
                Example_TurnOnLight();
            }
            else if (UnityEngine.Input.GetKeyDown(KeyCode.Alpha2))
            {
                Example_TurnOffLight();
            }
            else if (UnityEngine.Input.GetKeyDown(KeyCode.Alpha3))
            {
                Example_FadeIn();
            }
            else if (UnityEngine.Input.GetKeyDown(KeyCode.Alpha4))
            {
                Example_FadeOut();
            }
            else if (UnityEngine.Input.GetKeyDown(KeyCode.Alpha5))
            {
                Example_ActivateZone();
            }
            else if (UnityEngine.Input.GetKeyDown(KeyCode.Alpha6))
            {
                Example_DeactivateZone();
            }
            else if (UnityEngine.Input.GetKeyDown(KeyCode.Alpha7))
            {
                Example_ToggleSwitch();
            }
            else if (UnityEngine.Input.GetKeyDown(KeyCode.Alpha8))
            {
                Example_RandomColor();
            }
        }

        private void Example_TurnOnLight()
        {
            if (singleLight != null)
            {
                singleLight.TurnOn();
                Debug.Log("<color=green>[EXAMPLE] Light turned ON</color>");
            }
        }

        private void Example_TurnOffLight()
        {
            if (singleLight != null)
            {
                singleLight.TurnOff();
                Debug.Log("<color=yellow>[EXAMPLE] Light turned OFF</color>");
            }
        }

        private void Example_FadeIn()
        {
            if (lightEffects != null)
            {
                lightEffects.FadeIn(2f);
                Debug.Log("<color=cyan>[EXAMPLE] Fading IN over 2 seconds</color>");
            }
        }

        private void Example_FadeOut()
        {
            if (lightEffects != null)
            {
                lightEffects.FadeOut(2f);
                Debug.Log("<color=orange>[EXAMPLE] Fading OUT over 2 seconds</color>");
            }
        }

        private void Example_ActivateZone()
        {
            if (lightZone != null)
            {
                lightZone.ActivateZone();
                Debug.Log("<color=green>[EXAMPLE] Zone ACTIVATED</color>");
            }
        }

        private void Example_DeactivateZone()
        {
            if (lightZone != null)
            {
                lightZone.DeactivateZone();
                Debug.Log("<color=yellow>[EXAMPLE] Zone DEACTIVATED</color>");
            }
        }

        private void Example_ToggleSwitch()
        {
            if (lightSwitch != null)
            {
                lightSwitch.Interact(gameObject);
                Debug.Log("<color=cyan>[EXAMPLE] Switch TOGGLED</color>");
            }
        }

        private void Example_RandomColor()
        {
            if (singleLight != null)
            {
                Color randomColor = new Color(
                    Random.Range(0.5f, 1f),
                    Random.Range(0.5f, 1f),
                    Random.Range(0.5f, 1f)
                );
                singleLight.SetColor(randomColor);
                Debug.Log($"<color=magenta>[EXAMPLE] Color changed to {randomColor}</color>");
            }
        }

        public void Example_CreateTorchProgrammatically()
        {
            GameObject torchObject = new GameObject("DynamicTorch");
            torchObject.transform.position = transform.position;

            var light2D = torchObject.AddComponent<UnityEngine.Rendering.Universal.Light2D>();
            light2D.lightType = UnityEngine.Rendering.Universal.Light2D.LightType.Point;
            light2D.intensity = 1.3f;
            light2D.color = new Color(1f, 0.7f, 0.4f);
            light2D.pointLightOuterRadius = 4f;
            light2D.pointLightInnerRadius = 0.8f;

            var torchLight = torchObject.AddComponent<TorchLight>();

            Debug.Log("<color=green>[EXAMPLE] Torch created programmatically!</color>");
        }

        public void Example_CreateLightZoneProgrammatically()
        {
            GameObject zoneObject = new GameObject("DynamicLightZone");

            for (int i = 0; i < 3; i++)
            {
                GameObject lightObject = new GameObject($"ZoneLight_{i}");
                lightObject.transform.SetParent(zoneObject.transform);
                lightObject.transform.localPosition = new Vector3(i * 2f, 0f, 0f);

                var light2D = lightObject.AddComponent<UnityEngine.Rendering.Universal.Light2D>();
                light2D.lightType = UnityEngine.Rendering.Universal.Light2D.LightType.Point;
                light2D.intensity = 1f;
                light2D.pointLightOuterRadius = 3f;

                lightObject.AddComponent<BaseLightController>();
            }

            var zoneController = zoneObject.AddComponent<LightZoneController>();

            Debug.Log("<color=green>[EXAMPLE] Light zone with 3 lights created programmatically!</color>");
        }
    }
}
