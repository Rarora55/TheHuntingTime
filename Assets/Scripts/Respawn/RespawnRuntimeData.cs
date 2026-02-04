using UnityEngine;
using System.Collections.Generic;

namespace TheHunt.Respawn
{
    [CreateAssetMenu(fileName = "RespawnRuntimeData", menuName = "TheHunt/Data/Respawn Runtime Data")]
    public class RespawnRuntimeData : ScriptableObject
    {
        [Header("Current Respawn State")]
        [SerializeField] private Vector3 currentRespawnPosition;
        [SerializeField] private string currentRespawnID;
        
        [Header("Registered Respawn Points")]
        [SerializeField] private List<string> registeredRespawnIDs = new List<string>();

        public Vector3 CurrentRespawnPosition => currentRespawnPosition;
        public string CurrentRespawnID => currentRespawnID;
        public bool HasRespawnPoint => !string.IsNullOrEmpty(currentRespawnID);

        public void SetRespawn(Vector3 position, string respawnID)
        {
            currentRespawnPosition = position;
            currentRespawnID = respawnID;

            if (!registeredRespawnIDs.Contains(respawnID))
            {
                registeredRespawnIDs.Add(respawnID);
            }
        }

        public void Reset()
        {
            currentRespawnPosition = Vector3.zero;
            currentRespawnID = "";
            registeredRespawnIDs.Clear();
        }

        public bool IsRespawnRegistered(string respawnID)
        {
            return registeredRespawnIDs.Contains(respawnID);
        }

        private void OnEnable()
        {
            hideFlags = HideFlags.DontUnloadUnusedAsset;
        }

        private void OnDisable()
        {
            #if UNITY_EDITOR
            if (!UnityEditor.EditorApplication.isPlayingOrWillChangePlaymode)
            {
                Reset();
            }
            #endif
        }
    }
}
