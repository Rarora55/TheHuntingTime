using UnityEngine;
using UnityEngine.Events;

namespace TheHunt.Lighting
{
    public enum DayPeriod
    {
        Dawn,
        Day,
        Dusk,
        Night
    }

    [System.Serializable]
    public class DayPeriodEvent : UnityEvent<DayPeriod> { }

    [System.Serializable]
    public class TimeChangedEvent : UnityEvent<float> { }
}
