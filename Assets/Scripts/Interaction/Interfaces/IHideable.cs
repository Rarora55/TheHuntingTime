using System;
using UnityEngine;

namespace TheHunt.Interaction
{
    /// <summary>
    /// Implemented by any entity that can be hidden inside a HideAndSeekZone.
    /// Enemies and other systems subscribe to OnHideStateChanged to react accordingly.
    /// </summary>
    public interface IHideable
    {
        /// <summary>Whether the entity is currently hidden.</summary>
        bool IsHiding { get; }

        /// <summary>Fired whenever the hide state changes. Bool parameter is the new IsHiding value.</summary>
        event Action<bool> OnHideStateChanged;

        /// <summary>Called by a HideAndSeekZone to begin hiding this entity.</summary>
        void EnterHide(HideAndSeekZone zone);

        /// <summary>Called by a HideAndSeekZone to reveal this entity.</summary>
        void ExitHide();
    }
}
