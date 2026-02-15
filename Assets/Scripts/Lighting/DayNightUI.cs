using UnityEngine;
using TMPro;
using UnityEngine.UI;

namespace TheHunt.Lighting
{
    public class DayNightUI : MonoBehaviour
    {
        [Header("References")]
        [SerializeField] private DayNightCycle dayNightCycle;

        [Header("UI Elements")]
        [SerializeField] private TextMeshProUGUI timeText;
        [SerializeField] private TextMeshProUGUI periodText;
        [SerializeField] private Image timeIcon;

        [Header("Period Icons")]
        [SerializeField] private Sprite dawnIcon;
        [SerializeField] private Sprite dayIcon;
        [SerializeField] private Sprite duskIcon;
        [SerializeField] private Sprite nightIcon;

        [Header("Settings")]
        [SerializeField] private bool show24HourFormat = true;

        private void Start()
        {
            if (dayNightCycle == null)
            {
                dayNightCycle = FindFirstObjectByType<DayNightCycle>();
            }
        }

        private void Update()
        {
            if (dayNightCycle == null) return;

            UpdateTimeDisplay();
            UpdatePeriodDisplay();
        }

        private void UpdateTimeDisplay()
        {
            if (timeText != null)
            {
                if (show24HourFormat)
                {
                    timeText.text = dayNightCycle.GetFormattedTime();
                }
                else
                {
                    float hour = dayNightCycle.Hour;
                    int hours12 = Mathf.FloorToInt(hour % 12);
                    if (hours12 == 0) hours12 = 12;
                    int minutes = Mathf.FloorToInt((hour - Mathf.Floor(hour)) * 60f);
                    string ampm = hour < 12 ? "AM" : "PM";
                    timeText.text = $"{hours12:D2}:{minutes:D2} {ampm}";
                }
            }
        }

        private void UpdatePeriodDisplay()
        {
            if (periodText != null)
            {
                periodText.text = GetPeriodName(dayNightCycle.CurrentPeriod);
            }

            if (timeIcon != null)
            {
                timeIcon.sprite = GetPeriodIcon(dayNightCycle.CurrentPeriod);
            }
        }

        private string GetPeriodName(DayPeriod period)
        {
            switch (period)
            {
                case DayPeriod.Dawn: return "Amanecer";
                case DayPeriod.Day: return "Día";
                case DayPeriod.Dusk: return "Atardecer";
                case DayPeriod.Night: return "Noche";
                default: return "Desconocido";
            }
        }

        private Sprite GetPeriodIcon(DayPeriod period)
        {
            switch (period)
            {
                case DayPeriod.Dawn: return dawnIcon;
                case DayPeriod.Day: return dayIcon;
                case DayPeriod.Dusk: return duskIcon;
                case DayPeriod.Night: return nightIcon;
                default: return null;
            }
        }
    }
}
