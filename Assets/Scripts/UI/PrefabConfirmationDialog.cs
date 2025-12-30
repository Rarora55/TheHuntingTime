using UnityEngine;
using UnityEngine.UI;
using UnityEngine.EventSystems;
using TMPro;
using System;
using TheHunt.Input;

namespace TheHunt.UI
{
    public class PrefabConfirmationDialog : MonoBehaviour
    {
        [Header("UI References")]
        [SerializeField] private GameObject panel;
        [SerializeField] private TextMeshProUGUI titleText;
        [SerializeField] private TextMeshProUGUI descriptionText;
        [SerializeField] private Button yesButton;
        [SerializeField] private Button noButton;
        
        [Header("Dependencies")]
        [SerializeField] private InputContextManager inputContextManager;
        
        private Action onConfirm;
        private Action onCancel;
        private int manualSelection = 0;
        
        public bool IsOpen => panel != null && panel.activeSelf;
        
        void Awake()
        {
            if (inputContextManager == null)
            {
                inputContextManager = FindFirstObjectByType<InputContextManager>();
            }
            
            if (yesButton != null)
                yesButton.onClick.AddListener(OnYesClicked);
            
            if (noButton != null)
                noButton.onClick.AddListener(OnNoClicked);
            
            SetupButtonNavigation();
            Hide();
        }
        
        void SetupButtonNavigation()
        {
            if (yesButton != null)
            {
                Navigation yesNav = new Navigation();
                yesNav.mode = Navigation.Mode.None;
                yesButton.navigation = yesNav;
            }
            
            if (noButton != null)
            {
                Navigation noNav = new Navigation();
                noNav.mode = Navigation.Mode.None;
                noButton.navigation = noNav;
            }
        }
        
        public void OnNavigate(float direction)
        {
            if (!IsOpen)
                return;
            
            Debug.Log($"<color=magenta>[PREFAB DIALOG] OnNavigate called with direction: {direction}</color>");
            
            if (direction < -0.5f)
            {
                Debug.Log($"<color=magenta>[PREFAB DIALOG] Direction < -0.5 (A/Left), selecting YES (left button)</color>");
                SelectYes();
            }
            else if (direction > 0.5f)
            {
                Debug.Log($"<color=magenta>[PREFAB DIALOG] Direction > 0.5 (D/Right), selecting NO (right button)</color>");
                SelectNo();
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
            Debug.Log("<color=cyan>[PREFAB DIALOG] Selected YES (manualSelection=0)</color>");
        }
        
        void SelectNo()
        {
            manualSelection = 1;
            EventSystem.current.SetSelectedGameObject(null);
            EventSystem.current.SetSelectedGameObject(noButton.gameObject);
            Debug.Log("<color=cyan>[PREFAB DIALOG] Selected NO (manualSelection=1)</color>");
        }
        
        void ConfirmSelection()
        {
            GameObject selected = EventSystem.current.currentSelectedGameObject;
            Debug.Log($"<color=white>★★★ [PREFAB DIALOG] ConfirmSelection called!</color>");
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
        
        public void Show(string title, string description, Action onConfirm, Action onCancel = null)
        {
            this.onConfirm = onConfirm;
            this.onCancel = onCancel;
            
            Debug.Log($"<color=magenta>[PREFAB DIALOG] Show() called - panel is {(panel != null ? "NOT NULL" : "NULL")}</color>");
            
            if (titleText != null)
            {
                titleText.text = title;
                Debug.Log($"<color=magenta>[PREFAB DIALOG] Title text set to: {title}</color>");
            }
            else
            {
                Debug.LogError("[PREFAB DIALOG] titleText is NULL!");
            }
            
            if (descriptionText != null)
            {
                descriptionText.text = description;
                Debug.Log($"<color=magenta>[PREFAB DIALOG] Description text set to: {description}</color>");
            }
            else
            {
                Debug.LogError("[PREFAB DIALOG] descriptionText is NULL!");
            }
            
            if (panel != null)
            {
                bool wasActive = panel.activeSelf;
                panel.SetActive(true);
                Debug.Log($"<color=magenta>[PREFAB DIALOG] Panel activated! (was {wasActive}, now {panel.activeSelf})</color>");
            }
            else
            {
                Debug.LogError("[PREFAB DIALOG] panel is NULL! Cannot show dialog!");
            }
            
            manualSelection = 0;
            EventSystem.current.SetSelectedGameObject(null);
            EventSystem.current.SetSelectedGameObject(yesButton.gameObject);
            
            inputContextManager?.PushContext(InputContext.Dialog);
            
            Debug.Log($"<color=green>[PREFAB DIALOG] ========== SHOWN ==========</color>");
            Debug.Log($"<color=green>[PREFAB DIALOG] Title: {title}</color>");
            Debug.Log($"<color=green>[PREFAB DIALOG] Description: {description}</color>");
            Debug.Log($"<color=green>[PREFAB DIALOG] manualSelection initialized to: {manualSelection} (0=YES)</color>");
            Debug.Log($"<color=green>[PREFAB DIALOG] EventSystem selected: {EventSystem.current.currentSelectedGameObject?.name}</color>");
            Debug.Log($"<color=green>[PREFAB DIALOG] Controls: A/D or Left/Right to navigate, E to confirm</color>");
            Debug.Log($"<color=green>[PREFAB DIALOG] ===========================</color>");
        }
        
        public void Hide()
        {
            if (panel != null)
                panel.SetActive(false);
            
            inputContextManager?.PopContext();
            
            onConfirm = null;
            onCancel = null;
            
        }
        
        void OnYesClicked()
        {
            onConfirm?.Invoke();
            Hide();
        }
        
        void OnNoClicked()
        {
            onCancel?.Invoke();
            Hide();
        }
    }
}
