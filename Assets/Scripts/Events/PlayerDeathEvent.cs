using UnityEngine;
using System;

namespace TheHunt.Events
{
    [CreateAssetMenu(fileName = "PlayerDeathEvent", menuName = "TheHunt/Events/Player Death Event")]
    public class PlayerDeathEvent : ScriptableObject
    {
        private event Action<DeathType, Vector3> listeners;

        public void Raise(DeathType deathType, Vector3 deathPosition)
        {
            Debug.Log($"<color=red>[DEATH EVENT] Raised - Type: {deathType}</color>");
            listeners?.Invoke(deathType, deathPosition);
        }

        public void AddListener(Action<DeathType, Vector3> listener)
        {
            listeners += listener;
        }

        public void RemoveListener(Action<DeathType, Vector3> listener)
        {
            listeners -= listener;
        }

        private void OnDisable()
        {
            listeners = null;
        }
    }

    public enum DeathType
    {
        Normal,
        Fall,
        Instant
    }
}
