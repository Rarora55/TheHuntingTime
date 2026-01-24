using UnityEngine;
using System;

namespace TheHunt.Events
{
    [CreateAssetMenu(fileName = "RespawnRequestEvent", menuName = "TheHunt/Events/Respawn Request Event")]
    public class RespawnRequestEvent : ScriptableObject
    {
        private event Action<global::Player> listeners;

        public void Raise(global::Player player)
        {
            listeners?.Invoke(player);
        }

        public void AddListener(Action<global::Player> listener)
        {
            listeners += listener;
        }

        public void RemoveListener(Action<global::Player> listener)
        {
            listeners -= listener;
        }

        private void OnDisable()
        {
            listeners = null;
        }
    }
}
