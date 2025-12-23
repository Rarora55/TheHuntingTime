using UnityEngine;
using UnityEngine.UI;
using TMPro;
using System;

namespace TheHunt.UI
{
    public class ConfirmationDialog : MonoBehaviour
    {
        [Header("UI References")]
        [SerializeField] private GameObject dialogPanel;
        [SerializeField] private TextMeshProUGUI titleText;
        [SerializeField] private TextMeshProUGUI descriptionText;
        [SerializeField] private Button yesButton;
        [SerializeField] private Button noButton;
        [SerializeField] private TextMeshProUGUI yesButtonText;
        [SerializeField] private TextMeshProUGUI noButtonText;
        
        private Action onConfirm;
        private Action onCancel;
        private Canvas debugCanvas;
        
        public bool IsOpen => dialogPanel != null && dialogPanel.activeSelf;
        
        void Awake()
        {
            if (yesButton != null)
                yesButton.onClick.AddListener(OnYesClicked);
            
            if (noButton != null)
                noButton.onClick.AddListener(OnNoClicked);
            
            SetupDebugCanvas();
            
            RectTransform rt = dialogPanel != null ? dialogPanel.GetComponent<RectTransform>() : null;
            if (rt != null)
            {
                rt.anchorMin = new Vector2(0.5f, 0.5f);
                rt.anchorMax = new Vector2(0.5f, 0.5f);
                rt.pivot = new Vector2(0.5f, 0.5f);
                rt.sizeDelta = new Vector2(600, 400);
                rt.anchoredPosition = Vector2.zero;
                Debug.Log($"<color=green>[DIALOG] ✓ Background resized to 600x400 and centered</color>");
            }
            
            if (dialogPanel != null)
            {
                Image img = dialogPanel.GetComponent<Image>();
                if (img != null)
                {
                    img.color = new Color(0, 0, 0, 0.95f);
                    Debug.Log($"<color=green>[DIALOG] ✓ Background color set to black with 0.95 alpha</color>");
                }
            }
            
            Debug.Log($"<color=cyan>[DIALOG] Awake - dialogPanel: {(dialogPanel != null ? dialogPanel.name : "NULL")}</color>");
            Debug.Log($"<color=cyan>[DIALOG] Awake - titleText: {(titleText != null ? "OK" : "NULL")}</color>");
            Debug.Log($"<color=cyan>[DIALOG] Awake - descriptionText: {(descriptionText != null ? "OK" : "NULL")}</color>");
            
            Hide();
        }
        
        void SetupDebugCanvas()
        {
            Canvas parentCanvas = GetComponentInParent<Canvas>();
            if (parentCanvas != null && parentCanvas.gameObject != gameObject)
            {
                parentCanvas.sortingOrder = 100;
                Debug.Log($"<color=green>[DIALOG] ✓ Set parent Canvas sorting order to 100</color>");
            }
        }
        
        void OnDestroy()
        {
            if (yesButton != null)
                yesButton.onClick.RemoveListener(OnYesClicked);
            
            if (noButton != null)
                noButton.onClick.RemoveListener(OnNoClicked);
        }
        
        public void Show(string title, string description, Action onConfirm, Action onCancel = null)
        {
            this.onConfirm = onConfirm;
            this.onCancel = onCancel;
            
            if (titleText != null)
            {
                titleText.text = title;
                titleText.color = Color.white;
                titleText.fontSize = 32;
                Debug.Log($"<color=cyan>[DIALOG] ✓ Title set: '{title}'</color>");
            }
            
            if (descriptionText != null)
            {
                descriptionText.text = description;
                descriptionText.color = Color.white;
                descriptionText.fontSize = 24;
                Debug.Log($"<color=cyan>[DIALOG] ✓ Description set: '{description}'</color>");
            }
            
            if (dialogPanel != null)
            {
                dialogPanel.SetActive(true);
                
                RectTransform rt = dialogPanel.GetComponent<RectTransform>();
                if (rt != null)
                {
                    rt.SetAsLastSibling();
                    Debug.Log($"<color=cyan>[DIALOG] ✓ Panel moved to front (last sibling)</color>");
                    Debug.Log($"<color=cyan>[DIALOG] ✓ Panel size: {rt.rect.width}x{rt.rect.height}</color>");
                    Debug.Log($"<color=cyan>[DIALOG] ✓ Panel position: {rt.position}</color>");
                }
                
                Image img = dialogPanel.GetComponent<Image>();
                if (img != null)
                {
                    Debug.Log($"<color=cyan>[DIALOG] ✓ Image enabled: {img.enabled}, color: {img.color}, sprite: {(img.sprite != null ? img.sprite.name : "NULL")}</color>");
                }
                
                CanvasRenderer cr = dialogPanel.GetComponent<CanvasRenderer>();
                if (cr != null)
                {
                    Debug.Log($"<color=cyan>[DIALOG] ✓ CanvasRenderer cull: {cr.cull}, absolute depth: {cr.absoluteDepth}</color>");
                }
                
                Debug.Log($"<color=cyan>[DIALOG] ✓✓✓ Panel ACTIVATED: {dialogPanel.name}, IsActive: {dialogPanel.activeSelf}</color>");
            }
            else
            {
                Debug.LogError($"<color=red>[DIALOG] ✗ dialogPanel is NULL!</color>");
            }
            
            gameObject.SetActive(true);
            enabled = true;
            
            Debug.Log($"<color=green>[DIALOG] ========== DIALOG SHOWN ==========</color>");
            Debug.Log($"<color=green>[DIALOG] Title: {title}</color>");
            Debug.Log($"<color=green>[DIALOG] Description: {description}</color>");
            Debug.Log($"<color=green>[DIALOG] ===================================</color>");
        }
        
        public void SetButtonLabels(string yesLabel = "Yes", string noLabel = "No")
        {
            if (yesButtonText != null)
                yesButtonText.text = yesLabel;
            
            if (noButtonText != null)
                noButtonText.text = noLabel;
        }
        
        public void Hide()
        {
            if (dialogPanel != null)
            {
                dialogPanel.SetActive(false);
                Debug.Log($"<color=yellow>[DIALOG] ========== DIALOG HIDDEN ==========</color>");
            }
            
            onConfirm = null;
            onCancel = null;
        }
        
        private void OnYesClicked()
        {
            Debug.Log($"<color=green>[DIALOG] ✓✓✓ YES BUTTON CLICKED</color>");
            onConfirm?.Invoke();
            Hide();
        }
        
        private void OnNoClicked()
        {
            Debug.Log($"<color=yellow>[DIALOG] ✗✗✗ NO BUTTON CLICKED</color>");
            onCancel?.Invoke();
            Hide();
        }
    }
}
