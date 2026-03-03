using UnityEngine;
using TheHunt.Radio.Events;

namespace TheHunt.Radio
{
    [RequireComponent(typeof(BoxCollider2D))]
    public class RadioEventTrigger : MonoBehaviour
    {
        [Header("Event Configuration")]
        [SerializeField] private string eventID;
        [SerializeField] [TextArea(2, 5)] private string dialogMessage = "Radio signal detected...";

        [Header("Data Reference")]
        [SerializeField] private RadioEquipmentData radioEquipmentData;

        [Header("Events")]
        [SerializeField] private RadioDialogEvent onDialogRequested;

        [Header("Trigger Settings")]
        [SerializeField] private bool triggerOnce = true;
        [SerializeField] private LayerMask playerLayer;

        private bool hasTriggered;
        private bool isPlayerInside;
        private bool wasRadioHeld;
        private PlayerInputHandler playerInputHandler;
        private BoxCollider2D triggerCollider;

        private void Awake()
        {
            triggerCollider = GetComponent<BoxCollider2D>();
            triggerCollider.isTrigger = true;
        }

        private void Update()
        {
            if (!isPlayerInside || playerInputHandler == null)
                return;

            bool isRadioHeld = playerInputHandler.RadioInput;

            // Fire on the leading edge of the hold (started), not every frame.
            if (isRadioHeld && !wasRadioHeld)
            {
                TryTriggerRadioDialog();
            }

            wasRadioHeld = isRadioHeld;
        }

        private void OnTriggerEnter2D(Collider2D other)
        {
            Debug.Log($"<color=magenta>[RADIO EVENT] {eventID} - OnTriggerEnter2D with {other.gameObject.name} (Layer: {LayerMask.LayerToName(other.gameObject.layer)})</color>");

            if (((1 << other.gameObject.layer) & playerLayer) == 0)
            {
                Debug.Log($"<color=yellow>[RADIO EVENT] {eventID} - Layer mismatch. Expected Player layer, got {LayerMask.LayerToName(other.gameObject.layer)}</color>");
                return;
            }

            playerInputHandler = other.GetComponent<PlayerInputHandler>();
            isPlayerInside = true;
            wasRadioHeld = false;
            Debug.Log($"<color=cyan>[RADIO EVENT] {eventID} - Player entered zone. Waiting for Radio input...</color>");
        }

        private void OnTriggerExit2D(Collider2D other)
        {
            if (((1 << other.gameObject.layer) & playerLayer) == 0)
                return;

            isPlayerInside = false;
            wasRadioHeld = false;
            playerInputHandler = null;
            Debug.Log($"<color=yellow>[RADIO EVENT] {eventID} - Player left zone.</color>");
        }

        private void TryTriggerRadioDialog()
        {
            if (hasTriggered && triggerOnce)
            {
                Debug.Log($"<color=yellow>[RADIO EVENT] {eventID} - Already triggered, ignoring</color>");
                return;
            }

            if (radioEquipmentData == null)
            {
                Debug.LogError($"<color=red>[RADIO EVENT] {eventID} - RadioEquipmentData reference is missing!</color>");
                return;
            }

            if (!radioEquipmentData.HasRadioEquipped)
            {
                Debug.Log($"<color=yellow>[RADIO EVENT] {eventID} - Player needs radio to receive signal</color>");
                return;
            }

            if (onDialogRequested == null)
            {
                Debug.LogError($"<color=red>[RADIO EVENT] {eventID} - RadioDialogEvent reference is missing!</color>");
                return;
            }

            Debug.Log($"<color=green>[RADIO EVENT] {eventID} - Radio held inside zone! Triggering dialog...</color>");
            onDialogRequested.Raise(dialogMessage);
            hasTriggered = true;
            Debug.Log($"<color=cyan>[RADIO EVENT] {eventID} - Dialog event raised successfully!</color>");
        }

        /// <summary>
        /// Resets the trigger so it can fire again.
        /// </summary>
        public void ResetTrigger()
        {
            hasTriggered = false;
        }

        private void OnDrawGizmosSelected()
        {
            BoxCollider2D col = GetComponent<BoxCollider2D>();
            if (col != null)
            {
                Gizmos.color = hasTriggered ? Color.gray : Color.cyan;
                Vector3 center = (Vector2)transform.position + col.offset;
                Gizmos.DrawWireCube(center, col.size);
            }
        }
    }
}
