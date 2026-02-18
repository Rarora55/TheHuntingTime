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
            if (hasTriggered && triggerOnce)
                return;

            if (((1 << other.gameObject.layer) & playerLayer) == 0)
                return;

            if (radioEquipmentData == null)
            {
                Debug.LogError($"[RADIO EVENT] RadioEquipmentData reference is missing on {eventID}");
                return;
            }

            if (!radioEquipmentData.HasRadioEquipped)
            {
                Debug.Log($"<color=yellow>[RADIO EVENT] Player needs radio to receive signal: {eventID}</color>");
                return;
            }

            if (onDialogRequested != null)
            {
                onDialogRequested.Raise(dialogMessage);
                hasTriggered = true;
                Debug.Log($"<color=cyan>[RADIO EVENT] Triggered: {eventID}</color>");
            }
            else
            {
                Debug.LogError($"[RADIO EVENT] RadioDialogEvent reference is missing on {eventID}");
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
