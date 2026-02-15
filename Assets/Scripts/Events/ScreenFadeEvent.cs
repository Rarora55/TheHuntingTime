using UnityEngine;
using System;

namespace TheHunt.Events
{
    public enum FadeType
    {
        ToBlack,
        FromBlack,
        ToBlackAndTeleport
    }

    [System.Serializable]
    public class FadeRequest
    {
        public FadeType fadeType;
        public float duration;
        public Vector3 teleportPosition;
        public Transform teleportTarget;
        public Action onFadeComplete;
        public Action onTeleportComplete;
    }

    [CreateAssetMenu(fileName = "ScreenFadeEvent", menuName = "TheHunt/Events/Screen Fade Event")]
    public class ScreenFadeEvent : ScriptableObject
    {
        private event Action<FadeRequest> listeners;

        public void RaiseFadeToBlack(float duration, Action onComplete = null)
        {
            FadeRequest request = new FadeRequest
            {
                fadeType = FadeType.ToBlack,
                duration = duration,
                onFadeComplete = onComplete
            };
            listeners?.Invoke(request);
        }

        public void RaiseFadeFromBlack(float duration, Action onComplete = null)
        {
            FadeRequest request = new FadeRequest
            {
                fadeType = FadeType.FromBlack,
                duration = duration,
                onFadeComplete = onComplete
            };
            listeners?.Invoke(request);
        }

        public void RaiseFadeToBlackAndTeleport(float duration, Vector3 position, Transform target, Action onTeleportComplete = null)
        {
            FadeRequest request = new FadeRequest
            {
                fadeType = FadeType.ToBlackAndTeleport,
                duration = duration,
                teleportPosition = position,
                teleportTarget = target,
                onTeleportComplete = onTeleportComplete
            };
            listeners?.Invoke(request);
        }

        public void AddListener(Action<FadeRequest> listener)
        {
            listeners += listener;
        }

        public void RemoveListener(Action<FadeRequest> listener)
        {
            listeners -= listener;
        }

        private void OnDisable()
        {
            listeners = null;
        }
    }
}
