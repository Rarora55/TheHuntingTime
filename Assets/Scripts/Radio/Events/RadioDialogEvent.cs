using UnityEngine;
using System;

namespace TheHunt.Radio.Events
{
    [CreateAssetMenu(fileName = "RadioDialogEvent", menuName = "TheHunt/Events/Radio Dialog Event")]
    public class RadioDialogEvent : ScriptableObject
    {
        private event Action<string> listeners;

        public void Raise(string message)
        {
            listeners?.Invoke(message);
        }

        public void AddListener(Action<string> listener)
        {
            listeners += listener;
        }

        public void RemoveListener(Action<string> listener)
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
