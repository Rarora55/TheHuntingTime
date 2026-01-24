using UnityEngine;

namespace TheHunt.Respawn
{
    [CreateAssetMenu(fileName = "newRespawnData", menuName = "Data/Respawn Data")]
    public class RespawnData : ScriptableObject
    {
        [Header("Respawn Settings")]
        [Tooltip("Default respawn position if no checkpoint is set")]
        public Vector3 defaultRespawnPosition = Vector3.zero;
        
        [Header("Dialog Settings")]
        [Tooltip("Default title for respawn confirmation dialogs")]
        public string defaultDialogTitle = "Punto de Descenso";
        
        [Tooltip("Default message for respawn confirmation dialogs")]
        public string defaultDialogMessage = "¿Quieres bajar a este punto?";
        
        [Header("Visual Settings")]
        [Tooltip("Default gizmo color for respawn points")]
        public Color defaultGizmoColor = Color.green;
        
        [Tooltip("Default gizmo radius for respawn points")]
        public float defaultGizmoRadius = 0.5f;
        
        [Header("Behavior Settings")]
        [Tooltip("Should respawn points auto-activate when player enters?")]
        public bool autoActivateOnEnter = true;
        
        [Tooltip("Should respawn points require confirmation dialog?")]
        public bool requireConfirmation = true;
        
        [Tooltip("Should respawn points save to global manager?")]
        public bool saveToGlobalManager = true;
        
        [Header("Advanced Settings")]
        [Tooltip("Delay before teleporting player (in seconds)")]
        public float teleportDelay = 0f;
        
        [Tooltip("Should play sound effect on respawn?")]
        public bool playSoundOnRespawn = false;
        
        [Tooltip("Respawn sound effect")]
        public AudioClip respawnSound;
        
        [Header("Death Settings")]
        [Tooltip("Automatically respawn player on death")]
        public bool autoRespawnOnDeath = true;
        
        [Tooltip("Delay before auto-respawn (in seconds)")]
        public float respawnDelay = 2f;
        
        [Tooltip("Should reset player health on respawn?")]
        public bool resetHealthOnRespawn = true;
        
        [Tooltip("Health percentage to restore on respawn (0-1)")]
        [Range(0f, 1f)]
        public float healthRestorePercentage = 1f;
    }
}
