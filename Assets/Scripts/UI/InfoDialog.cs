using UnityEngine;
using UnityEngine.UI;
using TMPro;
using TheHunt.Input;
using System;

namespace TheHunt.UI
{
    public class InfoDialog : MonoBehaviour
    {
        [Header("UI References")]
        [SerializeField] private GameObject panel;
        [SerializeField] private TextMeshProUGUI titleText;
        [SerializeField] private TextMeshProUGUI messageText;
        [SerializeField] private TextMeshProUGUI closeHintText;
        
        private Action onClose;
        private bool isOpen = false;
        
        public bool IsOpen => isOpen;
        
        void Awake()
        {
            Debug.Log("<color=yellow>[INFO DIALOG] Awake</color>");
            Hide();
        }
        
        public void Show(string title, string message, Action onClose = null)
        {
            this.onClose = onClose;
            
            if (titleText != null)
            {
                titleText.text = title;
                Debug.Log($"<color=cyan>[INFO DIALOG] Title set to: {title}</color>");
            }
            
            if (messageText != null)
            {
                messageText.text = message;
                Debug.Log($"<color=cyan>[INFO DIALOG] Message set to: {message}</color>");
            }
            
            if (closeHintText != null)
            {
                closeHintText.text = "Press E to continue";
            }
            
            if (panel != null)
            {
                panel.SetActive(true);
                Debug.Log("<color=green>[INFO DIALOG] Panel activated</color>");
            }
            
            isOpen = true;
            
            InputContextManager.Instance?.PushContext(InputContext.Dialog);
            
            Debug.Log($"<color=green>[INFO DIALOG] ========== SHOWN ==========</color>");
            Debug.Log($"<color=green>[INFO DIALOG] Title: {title}</color>");
            Debug.Log($"<color=green>[INFO DIALOG] Message: {message}</color>");
            Debug.Log($"<color=green>[INFO DIALOG] Press E to close</color>");
            Debug.Log($"<color=green>[INFO DIALOG] ===========================</color>");
        }
        
        public void Hide()
        {
            if (panel != null)
                panel.SetActive(false);
            
            isOpen = false;
            
            if (InputContextManager.Instance != null && 
                InputContextManager.Instance.IsInContext(InputContext.Dialog))
            {
                InputContextManager.Instance.PopContext();
            }
            
            onClose?.Invoke();
            onClose = null;
            
            Debug.Log("<color=yellow>[INFO DIALOG] Hidden</color>");
        }
        
        public void OnConfirmInput()
        {
            if (!isOpen)
                return;
            
            Debug.Log("<color=cyan>[INFO DIALOG] Confirm pressed - closing dialog</color>");
            Hide();
        }
        
        public void OnCancelInput()
        {
            if (!isOpen)
                return;
            
            Debug.Log("<color=cyan>[INFO DIALOG] Cancel pressed - closing dialog</color>");
            Hide();
        }
    }
}
