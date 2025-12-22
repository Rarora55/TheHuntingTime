using UnityEngine;
using UnityEngine.UI;
using TMPro;
using System.Collections;
using System.Collections.Generic;

namespace TheHunt.Inventory
{
    public class ContextMenuUI : MonoBehaviour
    {
        [Header("References")]
        [SerializeField] private InventoryUIController uiController;

        [Header("UI Elements")]
        [SerializeField] private CanvasGroup canvasGroup;
        [SerializeField] private Transform optionsContainer;
        [SerializeField] private GameObject optionPrefab;
        
        [Header("Layout Settings")]
        [SerializeField] private bool autoResizePanel = true;
        [SerializeField] private float padding = 10f;
        [SerializeField] private float minHeight = 80f;
        [SerializeField] private float maxHeight = 300f;

        [Header("Visual Settings")]
        [SerializeField] private Color normalColor = Color.white;
        [SerializeField] private Color selectedColor = Color.yellow;
        [SerializeField] private float selectedFontSizeMultiplier = 1.1f;

        [Header("Animation Settings")]
        [SerializeField] private float animationDuration = 0.3f;
        [SerializeField] private AnimationCurve scaleCurve = AnimationCurve.EaseInOut(0f, 0f, 1f, 1f);
        [SerializeField] private bool animateOnOpen = true;
        [SerializeField] private bool animateOnClose = true;

        private List<TextMeshProUGUI> optionTexts = new List<TextMeshProUGUI>();
        private List<float> originalFontSizes = new List<float>();
        private int currentSelection = 0;
        private RectTransform rectTransform;
        private RectTransform containerRectTransform;
        private Coroutine currentAnimation;

        private void Awake()
        {
            if (uiController == null)
                uiController = FindFirstObjectByType<InventoryUIController>();

            if (canvasGroup == null)
                canvasGroup = GetComponent<CanvasGroup>();

            rectTransform = GetComponent<RectTransform>();
            
            if (optionsContainer != null)
                containerRectTransform = optionsContainer as RectTransform;
        }

        private void Start()
        {
            HideMenu();
        }

        private void OnEnable()
        {
            if (uiController != null)
            {
                uiController.OnContextMenuOpened += OnContextMenuOpened;
                uiController.OnContextMenuClosed += OnContextMenuClosed;
                uiController.OnContextMenuSelectionChanged += OnSelectionChanged;
            }
        }

        private void OnDisable()
        {
            if (uiController != null)
            {
                uiController.OnContextMenuOpened -= OnContextMenuOpened;
                uiController.OnContextMenuClosed -= OnContextMenuClosed;
                uiController.OnContextMenuSelectionChanged -= OnSelectionChanged;
            }
        }

        private void OnContextMenuOpened(List<ItemContextAction> actions)
        {
            ShowMenu();
            ClearOptions();

            foreach (ItemContextAction action in actions)
            {
                CreateOption(action);
            }

            UpdateSelectionVisual(0);
            
            if (autoResizePanel)
            {
                StartCoroutine(ResizePanelAfterLayout());
            }

            Debug.Log($"<color=cyan>[CONTEXT MENU UI] Opened with {actions.Count} actions</color>");
        }

        private void OnContextMenuClosed()
        {
            ClearOptions();
            HideMenu();

            Debug.Log("<color=cyan>[CONTEXT MENU UI] Closed</color>");
        }

        private void OnSelectionChanged(int newIndex)
        {
            UpdateSelectionVisual(newIndex);
        }

        private void CreateOption(ItemContextAction action)
        {
            if (optionsContainer == null || optionPrefab == null)
            {
                Debug.LogWarning("<color=yellow>[CONTEXT MENU UI] Container or prefab is null!</color>");
                return;
            }

            GameObject optionObj = Instantiate(optionPrefab, optionsContainer);
            TextMeshProUGUI textComponent = optionObj.GetComponent<TextMeshProUGUI>();

            if (textComponent != null)
            {
                textComponent.text = uiController.GetContextActionDisplayName(action);
                textComponent.color = normalColor;
                optionTexts.Add(textComponent);
                originalFontSizes.Add(textComponent.fontSize);
                Debug.Log($"<color=green>[CONTEXT MENU UI] Created option: {textComponent.text}</color>");
            }
            else
            {
                Debug.LogWarning("<color=yellow>[CONTEXT MENU UI] Prefab has no TextMeshProUGUI component!</color>");
            }
        }

        private void ClearOptions()
        {
            if (optionsContainer == null)
                return;

            foreach (Transform child in optionsContainer)
            {
                Destroy(child.gameObject);
            }

            optionTexts.Clear();
            originalFontSizes.Clear();
            currentSelection = 0;
        }

        private void UpdateSelectionVisual(int selectedIndex)
        {
            currentSelection = selectedIndex;

            for (int i = 0; i < optionTexts.Count; i++)
            {
                if (i >= originalFontSizes.Count)
                    continue;

                if (i == selectedIndex)
                {
                    optionTexts[i].color = selectedColor;
                    optionTexts[i].fontSize = originalFontSizes[i] * selectedFontSizeMultiplier;
                }
                else
                {
                    optionTexts[i].color = normalColor;
                    optionTexts[i].fontSize = originalFontSizes[i];
                }
            }
        }

        private void ShowMenu()
        {
            if (canvasGroup != null)
            {
                canvasGroup.alpha = 1f;
                canvasGroup.interactable = true;
                canvasGroup.blocksRaycasts = true;
            }

            if (animateOnOpen)
            {
                if (currentAnimation != null)
                    StopCoroutine(currentAnimation);
                
                currentAnimation = StartCoroutine(AnimateScale(Vector3.one, Vector3.one));
            }
        }

        private void HideMenu()
        {
            if (animateOnClose && gameObject.activeInHierarchy)
            {
                if (currentAnimation != null)
                    StopCoroutine(currentAnimation);
                
                currentAnimation = StartCoroutine(AnimateScaleAndHide());
            }
            else
            {
                if (canvasGroup != null)
                {
                    canvasGroup.alpha = 0f;
                    canvasGroup.interactable = false;
                    canvasGroup.blocksRaycasts = false;
                }
            }
        }

        private IEnumerator AnimateScale(Vector3 from, Vector3 to)
        {
            if (rectTransform == null)
                yield break;

            Vector3 startScale = new Vector3(1f, 0f, 1f);
            Vector3 targetScale = new Vector3(1f, 1f, 1f);

            rectTransform.localScale = startScale;

            float elapsed = 0f;

            while (elapsed < animationDuration)
            {
                elapsed += Time.unscaledDeltaTime;
                float t = Mathf.Clamp01(elapsed / animationDuration);
                float curveValue = scaleCurve.Evaluate(t);

                rectTransform.localScale = Vector3.Lerp(startScale, targetScale, curveValue);

                yield return null;
            }

            rectTransform.localScale = targetScale;
            currentAnimation = null;
        }

        private IEnumerator AnimateScaleAndHide()
        {
            if (rectTransform == null)
                yield break;

            Vector3 startScale = rectTransform.localScale;
            Vector3 targetScale = new Vector3(1f, 0f, 1f);

            float elapsed = 0f;

            while (elapsed < animationDuration)
            {
                elapsed += Time.unscaledDeltaTime;
                float t = Mathf.Clamp01(elapsed / animationDuration);
                float curveValue = scaleCurve.Evaluate(t);

                rectTransform.localScale = Vector3.Lerp(startScale, targetScale, curveValue);

                yield return null;
            }

            rectTransform.localScale = targetScale;

            if (canvasGroup != null)
            {
                canvasGroup.alpha = 0f;
                canvasGroup.interactable = false;
                canvasGroup.blocksRaycasts = false;
            }

            currentAnimation = null;
        }
        
        private IEnumerator ResizePanelAfterLayout()
        {
            yield return null;
            
            if (containerRectTransform != null && rectTransform != null)
            {
                UnityEngine.UI.LayoutRebuilder.ForceRebuildLayoutImmediate(containerRectTransform);
                yield return null;
                
                float containerHeight = UnityEngine.UI.LayoutUtility.GetPreferredHeight(containerRectTransform);
                float titleHeight = 30f;
                
                float totalHeight = titleHeight + containerHeight + (padding * 2);
                totalHeight = Mathf.Clamp(totalHeight, minHeight, maxHeight);
                
                rectTransform.SetSizeWithCurrentAnchors(RectTransform.Axis.Vertical, totalHeight);
                
                Debug.Log($"<color=green>[CONTEXT MENU UI] Resized panel to {totalHeight} (container: {containerHeight})</color>");
            }
        }
    }
}
