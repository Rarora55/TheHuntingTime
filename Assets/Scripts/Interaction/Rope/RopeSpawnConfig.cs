using System;
using System.Collections.Generic;
using UnityEngine;

namespace TheHunt.Interaction.Rope
{
    public enum RopeDialogActionType
    {
        UseRespawn,  // Teletransportar al spawn destino
        PickupRope,  // Coger la cuerda y desactivar spawn points
        DoNothing    // Cerrar el diálogo sin hacer nada
    }

    [Serializable]
    public class RopeDialogEntry
    {
        [Tooltip("En qué número de uso se muestra este diálogo. Ej: 2 = segundo uso.")]
        public int triggerOnUse = 2;

        [Header("Dialog Text")]
        public string title = "Rope";
        public string message = "Pick up rope (YES) or use respawn (NO)?";

        [Header("YES Button")]
        public string yesLabel = "Pick up rope";
        public RopeDialogActionType yesAction = RopeDialogActionType.PickupRope;

        [Header("NO Button")]
        public string noLabel = "Use respawn";
        public RopeDialogActionType noAction = RopeDialogActionType.UseRespawn;
    }

    [CreateAssetMenu(fileName = "RopeSpawnConfig", menuName = "TheHunt/Rope/Spawn Config")]
    public class RopeSpawnConfig : ScriptableObject
    {
        [Tooltip("Número máximo de usos antes de que la cuerda se desgaste.")]
        [Min(1)]
        public int maxUses = 10;

        [Tooltip("Si está activado, muestra un mensaje cuando la cuerda alcanza el último uso.")]
        public bool showLastUseWarning = true;
        public string lastUseWarningTitle = "Rope";
        public string lastUseWarningMessage = "This is the last use of the rope!";

        [Tooltip("Lista de diálogos configurados por número de uso.")]
        public List<RopeDialogEntry> dialogEntries = new List<RopeDialogEntry>();

        /// <summary>Returns the dialog entry for a specific use count, or null if none is configured.</summary>
        public RopeDialogEntry GetDialogForUse(int useCount)
        {
            return dialogEntries.Find(entry => entry.triggerOnUse == useCount);
        }

        /// <summary>Returns true if the rope has exceeded its max uses.</summary>
        public bool IsExhausted(int useCount) => useCount >= maxUses;

        /// <summary>Returns true if this is the last use.</summary>
        public bool IsLastUse(int useCount) => useCount == maxUses - 1;
    }
}
