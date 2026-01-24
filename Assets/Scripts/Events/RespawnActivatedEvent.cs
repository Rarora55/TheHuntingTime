using UnityEngine;
using System;

namespace TheHunt.Events
{
    [CreateAssetMenu(fileName = "RespawnActivatedEvent", menuName = "TheHunt/Events/Respawn Activated Event")]
    public class RespawnActivatedEvent : ScriptableObject
    {
        private event Action<Vector3, string> listeners;

        public void Raise(Vector3 position, string respawnID)
        {
            listeners?.Invoke(position, respawnID);
        }

        public void AddListener(Action<Vector3, string> listener)
        {
            listeners += listener;
        }

        public void RemoveListener(Action<Vector3, string> listener)
        {
            listeners -= listener;
        }

        private void OnDisable()
        {
            listeners = null;
        }
    }
}
