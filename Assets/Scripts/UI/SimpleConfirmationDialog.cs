using UnityEngine;
using UnityEngine.UI;
using UnityEngine.EventSystems;
using TMPro;
using System;
using TheHunt.Input;

namespace TheHunt.UI
{
    public class SimpleConfirmationDialog : MonoBehaviour
    {
        [Header("Dependencies")]
        [SerializeField] private InputContextManager inputContextManager;
        
        private Canvas canvas;
        private GameObject panel;
        private TextMeshProUGUI titleText;
        private TextMeshProUGUI descriptionText;
        private Button yesButton;
        private Button noButton;
        
        private Action onConfirm;
        private Action onCancel;
        
        private EventSystem eventSystem;
        private int manualSelection = 0;
        
        public bool IsOpen => panel != null && panel.activeSelf;
        
        void Awake()
        {
            if (inputContextManager == null)
            {
                inputContextManager = FindFirstObjectByType<InputContextManager>();
            }
            
            SetupEventSystem();
            CreateDialog();
            Hide();
        }
        
        void SetupEventSystem()
        {
            eventSystem = EventSystem.current;
            
            if (eventSystem == null)
            {
                GameObject esObj = new GameObject("EventSystem");
                eventSystem = esObj.AddComponent<EventSystem>();
                esObj.AddComponent<StandaloneInputModule>();
                Debug.Log("<color=green>[SIMPLE DIALOG] ✓ Created EventSystem</color>");
            }
        }
        
        public void OnNavigate(float direction)
        {
            if (!IsOpen)
                return;
            
            Debug.Log($"<color=magenta>[SIMPLE DIALOG] OnNavigate called with direction: {direction}</color>");
            
            if (direction < -0.5f)
            {
                Debug.Log($"<color=magenta>[SIMPLE DIALOG] Direction < -0.5 (A/Left), selecting NO (right button)</color>");
                SelectNo();
            }
            else if (direction > 0.5f)
            {
                Debug.Log($"<color=magenta>[SIMPLE DIALOG] Direction > 0.5 (D/Right), selecting YES (left button)</color>");
                SelectYes();
            }
        }
        
        public void OnConfirmInput()
        {
            if (!IsOpen)
                return;
            
            ConfirmSelection();
        }
        
        public void OnCancelInput()
        {
            if (!IsOpen)
                return;
            
            OnNoClicked();
        }
        
        void SelectYes()
        {
            manualSelection = 0;
            EventSystem.current.SetSelectedGameObject(null);
            EventSystem.current.SetSelectedGameObject(yesButton.gameObject);
            Debug.Log("<color=cyan>[SIMPLE DIALOG] Selected YES (manualSelection=0)</color>");
        }
        
        void SelectNo()
        {
            manualSelection = 1;
            EventSystem.current.SetSelectedGameObject(null);
            EventSystem.current.SetSelectedGameObject(noButton.gameObject);
            Debug.Log("<color=cyan>[SIMPLE DIALOG] Selected NO (manualSelection=1)</color>");
        }
        
        void ConfirmSelection()
        {
            GameObject selected = EventSystem.current.currentSelectedGameObject;
            Debug.Log($"<color=white>★★★ [SIMPLE DIALOG] ConfirmSelection called!</color>");
            Debug.Log($"<color=white>★★★ EventSystem selected: {selected?.name}</color>");
            Debug.Log($"<color=white>★★★ manualSelection: {manualSelection} (0=YES, 1=NO)</color>");
            
            if (manualSelection == 0)
            {
                Debug.Log($"<color=white>★★★ Calling OnYesClicked() ★★★</color>");
                OnYesClicked();
            }
            else
            {
                Debug.Log($"<color=white>★★★ Calling OnNoClicked() ★★★</color>");
                OnNoClicked();
            }
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
            
            yesButton = CreateButton("YesButton", contentBox.transform, new Vector2(80, -150), "YES", OnYesClicked);
            noButton = CreateButton("NoButton", contentBox.transform, new Vector2(-80, -150), "NO", OnNoClicked);
            
            SetupButtonNavigation();
            
            Debug.Log("<color=green>[SIMPLE DIALOG] ✓✓✓ Dialog created programmatically</color>");
        }
        
        void SetupButtonNavigation()
        {
            Navigation yesNav = new Navigation();
            yesNav.mode = Navigation.Mode.None;
            yesButton.navigation = yesNav;
            
            Navigation noNav = new Navigation();
            noNav.mode = Navigation.Mode.None;
            noButton.navigation = noNav;
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
            
            ColorBlock colors = btn.colors;
            colors.normalColor = new Color(0.2f, 0.6f, 1f, 1f);
            colors.highlightedColor = new Color(0.3f, 0.8f, 1f, 1f);
            colors.pressedColor = new Color(0.1f, 0.4f, 0.8f, 1f);
            colors.selectedColor = new Color(0.4f, 1f, 0.4f, 1f);
            colors.disabledColor = new Color(0.5f, 0.5f, 0.5f, 0.5f);
            colors.colorMultiplier = 1f;
            colors.fadeDuration = 0.1f;
            btn.colors = colors;
            
            GameObject textObj = new GameObject("Text");
            textObj.transform.SetParent(btnObj.transform, false);
            
            RectTransform textRect = textObj.AddComponent<RectTransform>();
            textRect.anchorMin = Vector2.zero;
            textRect.anchorMax = Vector2.one;
            textRect.sizeDelta = Vector2.zero;
            
            TextMeshProUGUI btnText = textObj.AddComponent<TextMeshProUGUI>();
            btnText.text = text;
            btnText.fontSize = 24;
            btnText.fontStyle = FontStyles.Bold;
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
            
            manualSelection = 0;
            EventSystem.current.SetSelectedGameObject(null);
            EventSystem.current.SetSelectedGameObject(yesButton.gameObject);
            
            inputContextManager?.PushContext(InputContext.Dialog);
            
            Debug.Log($"<color=green>[SIMPLE DIALOG] ========== SHOWN ==========</color>");
            Debug.Log($"<color=green>[SIMPLE DIALOG] Title: {title}</color>");
            Debug.Log($"<color=green>[SIMPLE DIALOG] Description: {description}</color>");
            Debug.Log($"<color=green>[SIMPLE DIALOG] manualSelection initialized to: {manualSelection} (0=YES)</color>");
            Debug.Log($"<color=green>[SIMPLE DIALOG] EventSystem selected: {EventSystem.current.currentSelectedGameObject?.name}</color>");
            Debug.Log($"<color=green>[SIMPLE DIALOG] Controls: A/D or Left/Right to navigate, E to confirm</color>");
            Debug.Log($"<color=green>[SIMPLE DIALOG] ===========================</color>");
        }
        
        public void Hide()
        {
            if (panel != null)
            {
                panel.SetActive(false);
                Debug.Log("<color=yellow>[SIMPLE DIALOG] Hidden</color>");
            }
            
            inputContextManager?.PopContext();
            
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
            Debug.Log($"<color=yellow>[SIMPLE DIALOG] onCancel is null? {onCancel == null}</color>");
            onCancel?.Invoke();
            Debug.Log("<color=yellow>[SIMPLE DIALOG] onCancel invoked (if not null)</color>");
            Hide();
        }
    }
}
