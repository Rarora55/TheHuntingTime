using UnityEngine;
using System;
using TheHunt.Lighting;

namespace TheHunt.Events
{
    [CreateAssetMenu(fileName = "LightControlEvent", menuName = "TheHunt/Events/Light Control Event")]
    public class LightControlEvent : ScriptableObject
    {
        private event Action<BaseLightController> listeners;

        public void Raise(BaseLightController light)
        {
            listeners?.Invoke(light);
        }

        public void AddListener(Action<BaseLightController> listener)
        {
            listeners += listener;
        }

        public void RemoveListener(Action<BaseLightController> listener)
        {
            listeners -= listener;
        }

        private void OnDisable()
        {
            listeners = null;
        }
    }
}
