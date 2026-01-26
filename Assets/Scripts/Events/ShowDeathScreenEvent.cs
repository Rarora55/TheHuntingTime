using UnityEngine;
using System;

namespace TheHunt.Events
{
    [CreateAssetMenu(fileName = "ShowDeathScreenEvent", menuName = "TheHunt/Events/Show Death Screen Event")]
    public class ShowDeathScreenEvent : ScriptableObject
    {
        private event Action<DeathType> listeners;

        public void Raise(DeathType deathType)
        {
            Debug.Log($"<color=yellow>[SHOW DEATH SCREEN] Type: {deathType}</color>");
            listeners?.Invoke(deathType);
        }

        public void AddListener(Action<DeathType> listener)
        {
            listeners += listener;
        }

        public void RemoveListener(Action<DeathType> listener)
        {
            listeners -= listener;
        }

        private void OnDisable()
        {
            listeners = null;
        }
    }
}
