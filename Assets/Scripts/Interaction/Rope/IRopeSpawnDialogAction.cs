using UnityEngine;

namespace TheHunt.Interaction.Rope
{
    /// <summary>
    /// Defines the contract for a dialog button action on a rope spawn point.
    /// Implement this interface to define what happens when the player selects YES or NO.
    /// </summary>
    public interface IRopeSpawnDialogAction
    {
        /// <summary>Execute the action when this button is selected.</summary>
        /// <param name="spawnPoint">The rope spawn point that triggered the dialog.</param>
        void Execute(RopeClimbSpawnPoint spawnPoint);
    }
}
