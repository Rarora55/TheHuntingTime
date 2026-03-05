using System;
using UnityEngine;
using TheHunt.Interaction;

namespace TheHunt.Player
{
    /// <summary>
    /// Manages the player's hide/reveal state.
    /// Implements IHideable so enemies and other systems can subscribe to OnHideStateChanged.
    /// When hiding, the player's SpriteRenderer is disabled and their Collider is turned off.
    /// </summary>
    [RequireComponent(typeof(SpriteRenderer))]
    public class PlayerHideState : MonoBehaviour, IHideable
    {
        // ── IHideable ────────────────────────────────────────────────────────

        public bool IsHiding { get; private set; }
        public event Action<bool> OnHideStateChanged;

        /// <summary>The zone currently hiding the player, or null.</summary>
        public HideAndSeekZone ActiveZone { get; private set; }

        // ── Dependencies ─────────────────────────────────────────────────────

        private SpriteRenderer spriteRenderer;

        [Header("Hide Settings")]
        [Tooltip("Additional colliders to disable while hiding (e.g. attack hitboxes). The main physics collider stays on so the player stays grounded.")]
        [SerializeField] private Collider2D[] collidersToDisable;

        // ── Unity lifecycle ──────────────────────────────────────────────────

        private void Awake()
        {
            spriteRenderer = GetComponent<SpriteRenderer>();
        }

        // ── IHideable implementation ─────────────────────────────────────────

        /// <summary>Called by HideAndSeekZone when the player enters hide mode.</summary>
        public void EnterHide(HideAndSeekZone zone)
        {
            if (IsHiding) return;

            IsHiding   = true;
            ActiveZone = zone;

            spriteRenderer.enabled = false;

            foreach (var col in collidersToDisable)
                if (col != null) col.enabled = false;

            OnHideStateChanged?.Invoke(true);
        }

        /// <summary>Called by HideAndSeekZone when the player leaves hide mode.</summary>
        public void ExitHide()
        {
            if (!IsHiding) return;

            IsHiding   = false;
            ActiveZone = null;

            spriteRenderer.enabled = true;

            foreach (var col in collidersToDisable)
                if (col != null) col.enabled = true;

            OnHideStateChanged?.Invoke(false);
        }
    }
}
