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
        
        [Header("Dependencies")]
        [SerializeField] private InputContextManager inputContextManager;
        
        private Action onClose;
        private bool isOpen = false;
        
        public bool IsOpen => isOpen;
        
        void Awake()
        {
            if (inputContextManager == null)
            {
                inputContextManager = FindFirstObjectByType<InputContextManager>();
            }
            
            Hide();
        }
        
        public void Show(string title, string message, Action onClose = null)
        {
            this.onClose = onClose;
            
            if (titleText != null)
            {
                titleText.text = title;
            }
            
            if (messageText != null)
            {
                messageText.text = message;
            }
            
            if (closeHintText != null)
            {
                closeHintText.text = "Press E to continue";
            }
            
            if (panel != null)
            {
                panel.SetActive(true);
            }
            
            isOpen = true;
            
            inputContextManager?.PushContext(InputContext.Dialog);
        }
        
        public void Hide()
        {
            if (panel != null)
                panel.SetActive(false);
            
            isOpen = false;
            
            if (inputContextManager != null && 
                inputContextManager.IsInContext(InputContext.Dialog))
            {
                inputContextManager.PopContext();
            }
            
            onClose?.Invoke();
            onClose = null;
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
