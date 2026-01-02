using UnityEngine;
using TheHunt.Interaction;

namespace TheHunt.Lighting
{
    public class InteractiveLightSwitch : MonoBehaviour, IInteractable
    {
        [Header("Light References")]
        [SerializeField] private BaseLightController[] controlledLights;
        [SerializeField] private bool findLightsInChildren = true;

        [Header("Switch Settings")]
        [SerializeField] private bool startsOn = true;
        [SerializeField] private string switchPrompt = "Toggle Light";
        [SerializeField] private bool canInteract = true;

        [Header("Visual Feedback")]
        [SerializeField] private SpriteRenderer switchSprite;
        [SerializeField] private Sprite onSprite;
        [SerializeField] private Sprite offSprite;

        [Header("Audio")]
        [SerializeField] private AudioClip switchSound;
        [SerializeField] private AudioSource audioSource;

        private bool isOn;

        public bool IsInteractable => canInteract;
        public string InteractionPrompt => switchPrompt;

        private void Awake()
        {
            if (findLightsInChildren && (controlledLights == null || controlledLights.Length == 0))
            {
                controlledLights = GetComponentsInChildren<BaseLightController>();
            }

            if (audioSource == null)
            {
                audioSource = GetComponent<AudioSource>();
            }
        }

        private void Start()
        {
            isOn = startsOn;
            UpdateLightsState(false);
            UpdateVisual();
        }

        public bool CanInteract(GameObject interactor)
        {
            return canInteract;
        }

        public void Interact(GameObject interactor)
        {
            ToggleLights();
        }

        private void ToggleLights()
        {
            isOn = !isOn;
            UpdateLightsState(true);
            UpdateVisual();
            PlaySound();

            Debug.Log($"<color=cyan>[LIGHT SWITCH] Toggled {(isOn ? "ON" : "OFF")}</color>");
        }

        private void UpdateLightsState(bool immediate)
        {
            if (controlledLights == null) return;

            foreach (var light in controlledLights)
            {
                if (light == null) continue;

                if (isOn)
                {
                    light.TurnOn();
                }
                else
                {
                    light.TurnOff();
                }
            }
        }

        private void UpdateVisual()
        {
            if (switchSprite != null)
            {
                switchSprite.sprite = isOn ? onSprite : offSprite;
            }
        }

        private void PlaySound()
        {
            if (audioSource != null && switchSound != null)
            {
                audioSource.PlayOneShot(switchSound);
            }
        }

        public void SetLightsState(bool turnOn)
        {
            isOn = turnOn;
            UpdateLightsState(true);
            UpdateVisual();
        }
    }
}
