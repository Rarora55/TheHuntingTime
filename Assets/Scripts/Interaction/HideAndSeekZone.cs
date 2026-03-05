using System.Collections.Generic;
using UnityEngine;
using TheHunt.UI;

namespace TheHunt.Interaction
{
    /// <summary>
    /// Place this component on a GameObject with a BoxCollider2D (trigger).
    /// When the player enters the zone and presses E, they become hidden inside it.
    /// Pressing E again reveals the player.
    /// Requires an InteractionFader sibling component to animate the transition.
    /// </summary>
    [RequireComponent(typeof(BoxCollider2D))]
    [RequireComponent(typeof(InteractionFader))]
    public class HideAndSeekZone : InteractableObject
    {
        private const string HidePrompt   = "Press E to hide";
        private const string RevealPrompt = "Press E to leave hiding spot";

        private InteractionFader fader;

        private readonly HashSet<IHideable> hiddeablesInside = new();

        private IHideable currentlyHidden;

        public bool IsOccupied => currentlyHidden != null;

        private void Awake()
        {
            fader = GetComponent<InteractionFader>();
        }

        // ── InteractableObject overrides ────────────────────────────────────

        public override bool CanInteract(GameObject interactor)
        {
            if (!isInteractable) return false;

            var hideable = interactor.GetComponent<IHideable>();
            if (hideable == null) return false;

            // Allow interaction if the interactor is inside the zone, or is already hiding here.
            return hiddeablesInside.Contains(hideable) || currentlyHidden == hideable;
        }

        protected override void OnInteract(GameObject interactor)
        {
            var hideable = interactor.GetComponent<IHideable>();
            if (hideable == null) return;

            if (currentlyHidden == hideable)
            {
                fader.Execute(() => RevealEntity(hideable));
            }
            else if (hiddeablesInside.Contains(hideable) && currentlyHidden == null)
            {
                fader.Execute(() => HideEntity(hideable));
            }
        }

        // ── Prompt ──────────────────────────────────────────────────────────
        // InteractionPrompt is backed by InteractableObject.interactionPrompt field.
        // We update it dynamically so the UI always shows the correct hint.

        private void UpdatePrompt()
        {
            interactionPrompt = currentlyHidden != null ? RevealPrompt : HidePrompt;
        }

        // ── Trigger detection ────────────────────────────────────────────────

        private void OnTriggerEnter2D(Collider2D other)
        {
            var hideable = other.GetComponent<IHideable>();
            if (hideable != null)
                hiddeablesInside.Add(hideable);
        }

        private void OnTriggerExit2D(Collider2D other)
        {
            var hideable = other.GetComponent<IHideable>();
            if (hideable == null) return;

            hiddeablesInside.Remove(hideable);

            // If the hidden entity somehow leaves the zone, reveal them.
            if (currentlyHidden == hideable)
                RevealEntity(hideable);
        }

        // ── Internal helpers ─────────────────────────────────────────────────

        private void HideEntity(IHideable hideable)
        {
            currentlyHidden = hideable;
            UpdatePrompt();
            hideable.EnterHide(this);
            Debug.Log($"<color=cyan>[HIDE ZONE] {((Component)hideable).gameObject.name} is now hiding in {gameObject.name}</color>");
        }

        private void RevealEntity(IHideable hideable)
        {
            currentlyHidden = null;
            UpdatePrompt();
            hideable.ExitHide();
            Debug.Log($"<color=yellow>[HIDE ZONE] {((Component)hideable).gameObject.name} left hiding spot in {gameObject.name}</color>");
        }
    }
}
