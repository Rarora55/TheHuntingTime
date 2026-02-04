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
            #if UNITY_EDITOR
            if (!UnityEditor.EditorApplication.isPlayingOrWillChangePlaymode)
            {
                listeners = null;
            }
            #else
            listeners = null;
            #endif
        }
    }
}
