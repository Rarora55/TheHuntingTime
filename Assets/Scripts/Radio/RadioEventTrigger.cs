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
        private BoxCollider2D triggerCollider;

        private void Awake()
        {
            triggerCollider = GetComponent<BoxCollider2D>();
            triggerCollider.isTrigger = true;
        }

        private void OnTriggerEnter2D(Collider2D other)
        {
            Debug.Log($"<color=magenta>[RADIO EVENT] {eventID} - OnTriggerEnter2D with {other.gameObject.name} (Layer: {LayerMask.LayerToName(other.gameObject.layer)})</color>");
            
            if (hasTriggered && triggerOnce)
            {
                Debug.Log($"<color=yellow>[RADIO EVENT] {eventID} - Already triggered, ignoring</color>");
                return;
            }

            if (((1 << other.gameObject.layer) & playerLayer) == 0)
            {
                Debug.Log($"<color=yellow>[RADIO EVENT] {eventID} - Layer mismatch. Expected Player layer, got {LayerMask.LayerToName(other.gameObject.layer)}</color>");
                return;
            }

            Debug.Log($"<color=cyan>[RADIO EVENT] {eventID} - Player detected!</color>");

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

            Debug.Log($"<color=green>[RADIO EVENT] {eventID} - Player has radio equipped! Triggering dialog...</color>");

            if (onDialogRequested != null)
            {
                onDialogRequested.Raise(dialogMessage);
                hasTriggered = true;
                Debug.Log($"<color=cyan>[RADIO EVENT] {eventID} - Dialog event raised successfully!</color>");
            }
            else
            {
                Debug.LogError($"<color=red>[RADIO EVENT] {eventID} - RadioDialogEvent reference is missing!</color>");
            }
        }

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
