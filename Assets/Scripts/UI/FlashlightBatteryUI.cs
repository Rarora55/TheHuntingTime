using UnityEngine;
using UnityEngine.UI;
using TMPro;
using TheHunt.Equipment;

namespace TheHunt.UI
{
    public class FlashlightBatteryUI : MonoBehaviour
    {
        [Header("References")]
        [SerializeField] private SecondaryEquipmentController equipmentController;
        
        [Header("UI Elements")]
        [SerializeField] private GameObject batteryPanel;
        [SerializeField] private Image batteryFillImage;
        [SerializeField] private TextMeshProUGUI batteryPercentageText;
        [SerializeField] private Image batteryIcon;
        
        [Header("Visual Settings")]
        [SerializeField] private Color fullBatteryColor = Color.green;
        [SerializeField] private Color mediumBatteryColor = Color.yellow;
        [SerializeField] private Color lowBatteryColor = Color.red;
        [SerializeField] private float lowBatteryThreshold = 0.25f;
        [SerializeField] private float mediumBatteryThreshold = 0.5f;
        
        [Header("Auto-Hide Settings")]
        [SerializeField] private bool autoHide = true;
        
        void Awake()
        {
            if (equipmentController == null)
            {
                equipmentController = FindFirstObjectByType<SecondaryEquipmentController>();
            }
        }
        
        void OnEnable()
        {
            if (equipmentController != null && equipmentController.Flashlight != null)
            {
                equipmentController.Flashlight.OnBatteryChanged += UpdateBatteryUI;
                equipmentController.Flashlight.OnToggled += HandleFlashlightToggled;
            }
        }
        
        void OnDisable()
        {
            if (equipmentController != null && equipmentController.Flashlight != null)
            {
                equipmentController.Flashlight.OnBatteryChanged -= UpdateBatteryUI;
                equipmentController.Flashlight.OnToggled -= HandleFlashlightToggled;
            }
        }
        
        void Start()
        {
            if (batteryPanel != null && autoHide)
            {
                batteryPanel.SetActive(false);
            }
        }
        
        void UpdateBatteryUI(float percentage)
        {
            if (batteryFillImage != null)
            {
                batteryFillImage.fillAmount = percentage;
                batteryFillImage.color = GetBatteryColor(percentage);
            }
            
            if (batteryPercentageText != null)
            {
                batteryPercentageText.text = $"{Mathf.RoundToInt(percentage * 100)}%";
            }
            
            if (batteryIcon != null)
            {
                batteryIcon.color = GetBatteryColor(percentage);
            }
        }
        
        Color GetBatteryColor(float percentage)
        {
            if (percentage <= lowBatteryThreshold)
            {
                return lowBatteryColor;
            }
            else if (percentage <= mediumBatteryThreshold)
            {
                return mediumBatteryColor;
            }
            else
            {
                return fullBatteryColor;
            }
        }
        
        void HandleFlashlightToggled(bool isOn)
        {
            if (batteryPanel != null && autoHide)
            {
                batteryPanel.SetActive(isOn);
            }
        }
        
        public void Show()
        {
            if (batteryPanel != null)
            {
                batteryPanel.SetActive(true);
            }
        }
        
        public void Hide()
        {
            if (batteryPanel != null)
            {
                batteryPanel.SetActive(false);
            }
        }
    }
}
