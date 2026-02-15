using UnityEngine;
using System;

namespace TheHunt.Events
{
    public enum LightCommand
    {
        TurnOnAll,
        TurnOffAll,
        SetGlobalIntensity
    }

    [CreateAssetMenu(fileName = "GlobalLightCommandEvent", menuName = "TheHunt/Events/Global Light Command Event")]
    public class GlobalLightCommandEvent : ScriptableObject
    {
        private event Action<LightCommand, float> listeners;

        public void Raise(LightCommand command, float value = 0f)
        {
            listeners?.Invoke(command, value);
        }

        public void AddListener(Action<LightCommand, float> listener)
        {
            listeners += listener;
        }

        public void RemoveListener(Action<LightCommand, float> listener)
        {
            listeners -= listener;
        }

        private void OnDisable()
        {
            listeners = null;
        }
    }
}
