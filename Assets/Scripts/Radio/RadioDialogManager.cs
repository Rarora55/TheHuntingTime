using UnityEngine;
using TheHunt.Radio.Events;

namespace TheHunt.Radio
{
    public class RadioDialogManager : MonoBehaviour
    {
        [Header("Data Reference")]
        [SerializeField] private RadioEquipmentData radioEquipmentData;

        [Header("Events")]
        [SerializeField] private RadioDialogEvent onRadioDialogRequested;

        [Header("Visual Reference")]
        [SerializeField] private RadioDialogVisual radioDialogVisual;

        [Header("Debug")]
        [SerializeField] private bool showDebugLogs = true;

        private void OnEnable()
        {
            if (onRadioDialogRequested != null)
            {
                onRadioDialogRequested.AddListener(HandleRadioDialogRequest);
            }
        }

        private void OnDisable()
        {
            if (onRadioDialogRequested != null)
            {
                onRadioDialogRequested.RemoveListener(HandleRadioDialogRequest);
            }
        }

        private void HandleRadioDialogRequest(string message)
        {
            if (radioEquipmentData == null)
            {
                Debug.LogError("[RADIO DIALOG] RadioEquipmentData reference is missing!");
                return;
            }

            if (!radioEquipmentData.HasRadioEquipped)
            {
                if (showDebugLogs)
                    Debug.Log("<color=yellow>[RADIO DIALOG] No radio equipped. Cannot show dialog.</color>");
                return;
            }

            if (radioDialogVisual == null)
            {
                Debug.LogError("[RADIO DIALOG] RadioDialogVisual reference is missing!");
                return;
            }

            if (showDebugLogs)
                Debug.Log($"<color=cyan>[RADIO DIALOG] Showing: {message}</color>");

            radioDialogVisual.ShowDialog();
        }
    }
}
