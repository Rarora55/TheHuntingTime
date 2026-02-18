using UnityEngine;
using System;
using TheHunt.Inventory;

namespace TheHunt.Radio.Events
{
    [CreateAssetMenu(fileName = "RadioEquipEvent", menuName = "TheHunt/Events/Radio Equip Event")]
    public class RadioEquipEvent : ScriptableObject
    {
        private event Action<RadioItemData> listeners;

        public void Raise(RadioItemData radio)
        {
            listeners?.Invoke(radio);
        }

        public void AddListener(Action<RadioItemData> listener)
        {
            listeners += listener;
        }

        public void RemoveListener(Action<RadioItemData> listener)
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
