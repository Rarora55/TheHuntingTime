using UnityEngine;
using UnityEngine.Events;

namespace TheHunt.Lighting.Examples
{
    public class DayNightResponder : MonoBehaviour
    {
        [Header("References")]
        [SerializeField] private DayNightCycle dayNightCycle;

        [Header("Responses")]
        [SerializeField] private UnityEvent onDawn;
        [SerializeField] private UnityEvent onDay;
        [SerializeField] private UnityEvent onDusk;
        [SerializeField] private UnityEvent onNight;

        [Header("Auto Disable")]
        [SerializeField] private bool disableDuringNight = false;
        [SerializeField] private GameObject[] objectsToToggle;

        private void OnEnable()
        {
            if (dayNightCycle == null)
            {
                dayNightCycle = FindFirstObjectByType<DayNightCycle>();
            }

            if (dayNightCycle != null)
            {
                SubscribeToEvents();
            }
        }

        private void OnDisable()
        {
            UnsubscribeFromEvents();
        }

        private void SubscribeToEvents()
        {
            UnsubscribeFromEvents();
        }

        private void UnsubscribeFromEvents()
        {
        }

        private void OnPeriodChanged(DayPeriod period)
        {
            switch (period)
            {
                case DayPeriod.Dawn:
                    onDawn?.Invoke();
                    if (disableDuringNight) SetObjectsActive(true);
                    break;

                case DayPeriod.Day:
                    onDay?.Invoke();
                    if (disableDuringNight) SetObjectsActive(true);
                    break;

                case DayPeriod.Dusk:
                    onDusk?.Invoke();
                    break;

                case DayPeriod.Night:
                    onNight?.Invoke();
                    if (disableDuringNight) SetObjectsActive(false);
                    break;
            }
        }

        private void SetObjectsActive(bool active)
        {
            foreach (var obj in objectsToToggle)
            {
                if (obj != null)
                {
                    obj.SetActive(active);
                }
            }
        }
    }
}
