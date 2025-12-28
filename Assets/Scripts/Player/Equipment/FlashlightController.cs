using System;
using UnityEngine;
using UnityEngine.Rendering.Universal;

namespace TheHunt.Equipment
{
    public class FlashlightController : MonoBehaviour
    {
        [Header("Light Settings")]
        [SerializeField] private Light2D flashlightLight;
        [SerializeField] private float maxIntensity = 1.5f;
        [SerializeField] private float outerRadius = 5f;
        [SerializeField] private float innerRadius = 0.5f;
        
        [Header("Battery Settings")]
        [SerializeField] private bool useBattery = true;
        [SerializeField] private float maxBatteryLife = 300f;
        [SerializeField] private float batteryDrainRate = 1f;
        
        [Header("Toggle Settings")]
        [SerializeField] private bool startOn = false;
        
        private bool isOn;
        private float currentBattery;
        
        public bool IsOn => isOn;
        public float BatteryPercentage => currentBattery / maxBatteryLife;
        public bool HasBattery => !useBattery || currentBattery > 0f;
        
        public event Action<bool> OnToggled;
        public event Action OnBatteryDepleted;
        public event Action<float> OnBatteryChanged;
        
        void Awake()
        {
            if (flashlightLight == null)
            {
                flashlightLight = GetComponentInChildren<Light2D>();
            }
            
            if (flashlightLight == null)
            {
                Debug.LogError($"<color=red>[FLASHLIGHT] No Light2D found on {gameObject.name}!</color>");
            }
            
            currentBattery = maxBatteryLife;
        }
        
        void Start()
        {
            if (startOn)
            {
                TurnOn();
            }
            else
            {
                TurnOff();
            }
        }
        
        void Update()
        {
            if (isOn && useBattery)
            {
                DrainBattery();
            }
        }
        
        public void Toggle()
        {
            if (isOn)
            {
                TurnOff();
            }
            else
            {
                TurnOn();
            }
        }
        
        public void TurnOn()
        {
            if (!HasBattery)
            {
                Debug.Log($"<color=yellow>[FLASHLIGHT] Cannot turn on - battery depleted</color>");
                return;
            }
            
            isOn = true;
            
            if (flashlightLight != null)
            {
                flashlightLight.enabled = true;
                flashlightLight.intensity = maxIntensity;
            }
            
            OnToggled?.Invoke(true);
            Debug.Log($"<color=green>[FLASHLIGHT] Turned ON</color>");
        }
        
        public void TurnOff()
        {
            isOn = false;
            
            if (flashlightLight != null)
            {
                flashlightLight.enabled = false;
            }
            
            OnToggled?.Invoke(false);
            Debug.Log($"<color=yellow>[FLASHLIGHT] Turned OFF</color>");
        }
        
        void DrainBattery()
        {
            if (currentBattery <= 0f)
            {
                HandleBatteryDepleted();
                return;
            }
            
            currentBattery -= batteryDrainRate * Time.deltaTime;
            currentBattery = Mathf.Max(0f, currentBattery);
            
            OnBatteryChanged?.Invoke(BatteryPercentage);
            
            if (currentBattery <= 0f)
            {
                HandleBatteryDepleted();
            }
        }
        
        void HandleBatteryDepleted()
        {
            TurnOff();
            OnBatteryDepleted?.Invoke();
            Debug.Log($"<color=red>[FLASHLIGHT] Battery depleted!</color>");
        }
        
        public void RechargeBattery(float amount)
        {
            currentBattery = Mathf.Min(currentBattery + amount, maxBatteryLife);
            OnBatteryChanged?.Invoke(BatteryPercentage);
            Debug.Log($"<color=green>[FLASHLIGHT] Battery recharged: {BatteryPercentage:P0}</color>");
        }
        
        public void SetFullBattery()
        {
            currentBattery = maxBatteryLife;
            OnBatteryChanged?.Invoke(BatteryPercentage);
        }
    }
}
