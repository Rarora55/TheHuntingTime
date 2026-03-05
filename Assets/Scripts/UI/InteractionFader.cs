using System;
using UnityEngine;
using TheHunt.Events;

namespace TheHunt.UI
{
    /// <summary>
    /// Reusable component that wraps a ScreenFadeEvent to produce a
    /// fade-to-black → action → fade-from-black sequence for any interaction.
    /// Add it to any GameObject that needs a brief screen flash on interact.
    /// </summary>
    public class InteractionFader : MonoBehaviour
    {
        [Header("Fade Event")]
        [SerializeField] private ScreenFadeEvent screenFadeEvent;

        [Header("Timings")]
        [Tooltip("Duration of the fade-to-black transition (seconds).")]
        [SerializeField] private float fadeInDuration = 0.15f;

        [Tooltip("Duration of the fade-from-black transition (seconds).")]
        [SerializeField] private float fadeOutDuration = 0.15f;

        // ── Public API ───────────────────────────────────────────────────────

        /// <summary>
        /// Triggers a full fade-in → midAction → fade-out cycle.
        /// </summary>
        /// <param name="midAction">Invoked at peak black, between the two fades.</param>
        /// <param name="onComplete">Invoked after the fade-out finishes (optional).</param>
        public void Execute(Action midAction, Action onComplete = null)
        {
            if (screenFadeEvent == null)
            {
                Debug.LogWarning($"[InteractionFader] ScreenFadeEvent not assigned on {gameObject.name}. Executing action without fade.");
                midAction?.Invoke();
                onComplete?.Invoke();
                return;
            }

            screenFadeEvent.RaiseFadeToBlack(fadeInDuration, () =>
            {
                midAction?.Invoke();
                screenFadeEvent.RaiseFadeFromBlack(fadeOutDuration, onComplete);
            });
        }

        /// <summary>
        /// Overrides the fade durations at runtime (e.g. from a specific interactable).
        /// </summary>
        public void SetDurations(float fadeIn, float fadeOut)
        {
            fadeInDuration  = fadeIn;
            fadeOutDuration = fadeOut;
        }
    }
}
