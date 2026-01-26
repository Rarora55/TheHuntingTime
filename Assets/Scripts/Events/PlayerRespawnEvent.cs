using UnityEngine;
using System;

namespace TheHunt.Events
{
    [CreateAssetMenu(fileName = "PlayerRespawnEvent", menuName = "TheHunt/Events/Player Respawn Event")]
    public class PlayerRespawnEvent : ScriptableObject
    {
        private event Action listeners;

        public void Raise()
        {
            Debug.Log("<color=green>[RESPAWN EVENT] Raised</color>");
            listeners?.Invoke();
        }

        public void AddListener(Action listener)
        {
            listeners += listener;
        }

        public void RemoveListener(Action listener)
        {
            listeners -= listener;
        }

        private void OnDisable()
        {
            listeners = null;
        }
    }
}
