using UnityEngine;
using UnityEngine.UI;
using TMPro;
using System;

namespace TheHunt.UI
{
    public class SimpleConfirmationDialog : MonoBehaviour
    {
        private static SimpleConfirmationDialog instance;
        
        private Canvas canvas;
        private GameObject panel;
        private TextMeshProUGUI titleText;
        private TextMeshProUGUI descriptionText;
        private Button yesButton;
        private Button noButton;
        
        private Action onConfirm;
        private Action onCancel;
        
        public bool IsOpen => panel != null && panel.activeSelf;
        
        void Awake()
        {
            if (instance != null && instance != this)
            {
                Destroy(gameObject);
                return;
            }
            
            instance = this;
            DontDestroyOnLoad(gameObject);
            
            CreateDialog();
            Hide();
        }
        
        void CreateDialog()
        {
            GameObject canvasObj = new GameObject("ConfirmationDialogCanvas");
            canvasObj.transform.SetParent(transform);
            
            canvas = canvasObj.AddComponent<Canvas>();
            canvas.renderMode = RenderMode.ScreenSpaceOverlay;
            canvas.sortingOrder = 999;
            
            CanvasScaler scaler = canvasObj.AddComponent<CanvasScaler>();
            scaler.uiScaleMode = CanvasScaler.ScaleMode.ScaleWithScreenSize;
            scaler.referenceResolution = new Vector2(1920, 1080);
            
            canvasObj.AddComponent<GraphicRaycaster>();
            
            panel = new GameObject("DialogPanel");
            panel.transform.SetParent(canvasObj.transform, false);
            
            RectTransform panelRect = panel.AddComponent<RectTransform>();
            panelRect.anchorMin = Vector2.zero;
            panelRect.anchorMax = Vector2.one;
            panelRect.sizeDelta = Vector2.zero;
            
            Image panelBg = panel.AddComponent<Image>();
            panelBg.color = new Color(0, 0, 0, 0.8f);
            
            GameObject contentBox = new GameObject("ContentBox");
            contentBox.transform.SetParent(panel.transform, false);
            
            RectTransform contentRect = contentBox.AddComponent<RectTransform>();
            contentRect.anchorMin = new Vector2(0.5f, 0.5f);
            contentRect.anchorMax = new Vector2(0.5f, 0.5f);
            contentRect.sizeDelta = new Vector2(600, 400);
            contentRect.anchoredPosition = Vector2.zero;
            
            Image contentBg = contentBox.AddComponent<Image>();
            contentBg.color = new Color(0.1f, 0.1f, 0.1f, 1f);
            
            GameObject titleObj = new GameObject("Title");
            titleObj.transform.SetParent(contentBox.transform, false);
            
            RectTransform titleRect = titleObj.AddComponent<RectTransform>();
            titleRect.anchorMin = new Vector2(0, 1);
            titleRect.anchorMax = new Vector2(1, 1);
            titleRect.sizeDelta = new Vector2(-40, 60);
            titleRect.anchoredPosition = new Vector2(0, -40);
            
            titleText = titleObj.AddComponent<TextMeshProUGUI>();
            titleText.text = "Confirmation";
            titleText.fontSize = 32;
            titleText.alignment = TextAlignmentOptions.Center;
            titleText.color = Color.white;
            
            GameObject descObj = new GameObject("Description");
            descObj.transform.SetParent(contentBox.transform, false);
            
            RectTransform descRect = descObj.AddComponent<RectTransform>();
            descRect.anchorMin = new Vector2(0, 0.5f);
            descRect.anchorMax = new Vector2(1, 0.5f);
            descRect.sizeDelta = new Vector2(-40, 120);
            descRect.anchoredPosition = new Vector2(0, 20);
            
            descriptionText = descObj.AddComponent<TextMeshProUGUI>();
            descriptionText.text = "Are you sure?";
            descriptionText.fontSize = 24;
            descriptionText.alignment = TextAlignmentOptions.Center;
            descriptionText.color = Color.white;
            
            yesButton = CreateButton("YesButton", contentBox.transform, new Vector2(-80, -150), "YES", OnYesClicked);
            noButton = CreateButton("NoButton", contentBox.transform, new Vector2(80, -150), "NO", OnNoClicked);
            
            Debug.Log("<color=green>[SIMPLE DIALOG] ✓✓✓ Dialog created programmatically</color>");
        }
        
        Button CreateButton(string name, Transform parent, Vector2 position, string text, UnityEngine.Events.UnityAction onClick)
        {
            GameObject btnObj = new GameObject(name);
            btnObj.transform.SetParent(parent, false);
            
            RectTransform btnRect = btnObj.AddComponent<RectTransform>();
            btnRect.anchorMin = new Vector2(0.5f, 0.5f);
            btnRect.anchorMax = new Vector2(0.5f, 0.5f);
            btnRect.sizeDelta = new Vector2(140, 50);
            btnRect.anchoredPosition = position;
            
            Image btnImg = btnObj.AddComponent<Image>();
            btnImg.color = new Color(0.2f, 0.6f, 1f, 1f);
            
            Button btn = btnObj.AddComponent<Button>();
            btn.onClick.AddListener(onClick);
            
            GameObject textObj = new GameObject("Text");
            textObj.transform.SetParent(btnObj.transform, false);
            
            RectTransform textRect = textObj.AddComponent<RectTransform>();
            textRect.anchorMin = Vector2.zero;
            textRect.anchorMax = Vector2.one;
            textRect.sizeDelta = Vector2.zero;
            
            TextMeshProUGUI btnText = textObj.AddComponent<TextMeshProUGUI>();
            btnText.text = text;
            btnText.fontSize = 24;
            btnText.alignment = TextAlignmentOptions.Center;
            btnText.color = Color.white;
            
            return btn;
        }
        
        public void Show(string title, string description, Action onConfirm, Action onCancel = null)
        {
            this.onConfirm = onConfirm;
            this.onCancel = onCancel;
            
            titleText.text = title;
            descriptionText.text = description;
            
            panel.SetActive(true);
            
            Debug.Log($"<color=green>[SIMPLE DIALOG] ========== SHOWN ==========</color>");
            Debug.Log($"<color=green>[SIMPLE DIALOG] Title: {title}</color>");
            Debug.Log($"<color=green>[SIMPLE DIALOG] Description: {description}</color>");
            Debug.Log($"<color=green>[SIMPLE DIALOG] ===========================</color>");
        }
        
        public void Hide()
        {
            if (panel != null)
            {
                panel.SetActive(false);
                Debug.Log("<color=yellow>[SIMPLE DIALOG] Hidden</color>");
            }
            
            onConfirm = null;
            onCancel = null;
        }
        
        void OnYesClicked()
        {
            Debug.Log("<color=green>[SIMPLE DIALOG] YES clicked</color>");
            onConfirm?.Invoke();
            Hide();
        }
        
        void OnNoClicked()
        {
            Debug.Log("<color=yellow>[SIMPLE DIALOG] NO clicked</color>");
            onCancel?.Invoke();
            Hide();
        }
        
        public static SimpleConfirmationDialog Instance => instance;
    }
}
