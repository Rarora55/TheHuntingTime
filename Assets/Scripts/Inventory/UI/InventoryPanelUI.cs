using UnityEngine;
using System.Collections;
using System.Collections.Generic;

namespace TheHunt.Inventory
{
    public class InventoryPanelUI : MonoBehaviour
    {
        [Header("References")]
        [SerializeField] private InventorySystem inventorySystem;
        [SerializeField] private InventoryUIController uiController;

        [Header("Slot Settings")]
        [SerializeField] private Transform slotsContainer;
        [SerializeField] private GameObject slotPrefab;

        [Header("Panels")]
        [SerializeField] private CanvasGroup canvasGroup;

        [Header("Carousel Settings")]
        [SerializeField] private int visibleSlots = 3;
        [SerializeField] private float slotSpacing = 220f;
        [SerializeField] private float transitionSpeed = 8f;
        [SerializeField] private AnimationCurve transitionCurve = AnimationCurve.EaseInOut(0f, 0f, 1f, 1f);

        private List<InventorySlotUI> slotUIList = new List<InventorySlotUI>();
        private int currentHighlightedSlot = 0;
        private Vector3[] targetPositions;
        private bool isAnimating = false;

        private void Awake()
        {
            if (inventorySystem == null)
            {
                GameObject player = GameObject.FindGameObjectWithTag("Player");
                if (player != null)
                {
                    inventorySystem = player.GetComponent<InventorySystem>();
                }
            }

            if (uiController == null)
            {
                GameObject player = GameObject.FindGameObjectWithTag("Player");
                if (player != null)
                {
                    uiController = player.GetComponent<InventoryUIController>();
                }
            }

            if (canvasGroup == null)
                canvasGroup = GetComponent<CanvasGroup>();

            CreateSlots();
        }

        private void OnEnable()
        {
            if (inventorySystem != null)
            {
                inventorySystem.OnItemAdded += OnItemAdded;
                inventorySystem.OnItemRemoved += OnItemRemoved;
                inventorySystem.OnSelectionChanged += OnSelectionChanged;
            }

            if (uiController != null)
            {
                uiController.OnStateChanged += OnInventoryStateChanged;
            }
        }

        private void OnDisable()
        {
            if (inventorySystem != null)
            {
                inventorySystem.OnItemAdded -= OnItemAdded;
                inventorySystem.OnItemRemoved -= OnItemRemoved;
                inventorySystem.OnSelectionChanged -= OnSelectionChanged;
            }

            if (uiController != null)
            {
                uiController.OnStateChanged -= OnInventoryStateChanged;
            }
        }

        private void Start()
        {
            RefreshAllSlots();
            InitializeCarouselPositions();
            UpdateCarouselPositions(currentHighlightedSlot, true);
            HideInventory();
        }

        private void CreateSlots()
        {
            if (slotsContainer == null || slotPrefab == null)
            {
                Debug.LogError("[INVENTORY UI] Missing slots container or slot prefab!");
                return;
            }

            for (int i = slotsContainer.childCount - 1; i >= 0; i--)
            {
                Destroy(slotsContainer.GetChild(i).gameObject);
            }

            slotUIList.Clear();

            for (int i = 0; i < InventorySystem.MAX_SLOTS; i++)
            {
                GameObject slotObj = Instantiate(slotPrefab, slotsContainer);
                InventorySlotUI slotUI = slotObj.GetComponent<InventorySlotUI>();

                if (slotUI != null)
                {
                    slotUI.Initialize(i);
                    slotUIList.Add(slotUI);
                }
                else
                {
                    Debug.LogError($"[INVENTORY UI] Slot prefab missing InventorySlotUI component!");
                }
            }
        }

        private void RefreshAllSlots()
        {
            if (inventorySystem == null || slotUIList.Count == 0)
                return;

            ItemInstance[] items = inventorySystem.Items;

            for (int i = 0; i < slotUIList.Count && i < items.Length; i++)
            {
                slotUIList[i].UpdateSlot(items[i]);
            }

            UpdateHighlight(inventorySystem.SelectedSlot);
        }

        private void OnItemAdded(int slotIndex, ItemInstance item)
        {
            if (slotIndex >= 0 && slotIndex < slotUIList.Count)
            {
                slotUIList[slotIndex].UpdateSlot(item);
            }
        }

        private void OnItemRemoved(int slotIndex, ItemInstance item)
        {
            if (slotIndex >= 0 && slotIndex < slotUIList.Count)
            {
                ItemInstance currentItem = inventorySystem.Items[slotIndex];
                slotUIList[slotIndex].UpdateSlot(currentItem);
                Debug.Log($"<color=cyan>[INVENTORY UI] Cleared/Updated slot {slotIndex}</color>");
            }
        }

        private void OnSelectionChanged(int previousSlot, int newSlot)
        {
            UpdateHighlight(newSlot);
            UpdateCarouselPositions(newSlot, false);
        }

        private void UpdateHighlight(int slotIndex)
        {
            for (int i = 0; i < slotUIList.Count; i++)
            {
                if (i == slotIndex)
                    slotUIList[i].Highlight();
                else
                    slotUIList[i].Unhighlight();
            }

            currentHighlightedSlot = slotIndex;
        }

        private void OnInventoryStateChanged(InventoryState newState)
        {
            switch (newState)
            {
                case InventoryState.Open:
                    ShowInventory();
                    break;

                case InventoryState.Closed:
                    HideInventory();
                    break;

                case InventoryState.ContextMenu:
                    Debug.Log("<color=cyan>[INVENTORY UI] Context menu state</color>");
                    break;
            }
        }

        private void ShowInventory()
        {
            if (canvasGroup != null)
            {
                canvasGroup.alpha = 1f;
                canvasGroup.interactable = true;
                canvasGroup.blocksRaycasts = true;
                RefreshAllSlots();
            }

            Debug.Log("<color=cyan>[INVENTORY UI] Inventory panel shown</color>");
        }

        private void HideInventory()
        {
            if (canvasGroup != null)
            {
                canvasGroup.alpha = 0f;
                canvasGroup.interactable = false;
                canvasGroup.blocksRaycasts = false;
            }

            Debug.Log("<color=cyan>[INVENTORY UI] Inventory panel hidden</color>");
        }

        private void InitializeCarouselPositions()
        {
            targetPositions = new Vector3[slotUIList.Count];

            for (int i = 0; i < slotUIList.Count; i++)
            {
                targetPositions[i] = Vector3.zero;
            }
        }

        private void UpdateCarouselPositions(int centerSlot, bool immediate)
        {
            if (slotUIList.Count == 0)
                return;

            int halfVisible = visibleSlots / 2;

            for (int i = 0; i < slotUIList.Count; i++)
            {
                int offset = i - centerSlot;
                
                float xPosition = -offset * slotSpacing;
                targetPositions[i] = new Vector3(xPosition, 0f, 0f);

                bool shouldBeVisible = Mathf.Abs(offset) <= halfVisible;
                
                RectTransform rectTransform = slotUIList[i].GetComponent<RectTransform>();
                if (rectTransform != null)
                {
                    if (immediate)
                    {
                        rectTransform.anchoredPosition = targetPositions[i];
                        SetSlotVisibility(slotUIList[i], shouldBeVisible, immediate);
                    }
                    else
                    {
                        if (!isAnimating)
                        {
                            StartCoroutine(AnimateCarousel());
                        }
                    }
                }
            }
        }

        private IEnumerator AnimateCarousel()
        {
            isAnimating = true;
            
            Vector3[] startPositions = new Vector3[slotUIList.Count];
            for (int i = 0; i < slotUIList.Count; i++)
            {
                RectTransform rt = slotUIList[i].GetComponent<RectTransform>();
                if (rt != null)
                {
                    startPositions[i] = rt.anchoredPosition;
                }
            }

            float elapsed = 0f;
            float duration = 1f / transitionSpeed;

            while (elapsed < duration)
            {
                elapsed += Time.unscaledDeltaTime;
                float t = Mathf.Clamp01(elapsed / duration);
                float curveValue = transitionCurve.Evaluate(t);

                int halfVisible = visibleSlots / 2;

                for (int i = 0; i < slotUIList.Count; i++)
                {
                    RectTransform rt = slotUIList[i].GetComponent<RectTransform>();
                    if (rt != null)
                    {
                        rt.anchoredPosition = Vector3.Lerp(startPositions[i], targetPositions[i], curveValue);

                        int offset = Mathf.Abs(i - currentHighlightedSlot);
                        bool shouldBeVisible = offset <= halfVisible;
                        SetSlotVisibility(slotUIList[i], shouldBeVisible, false);
                    }
                }

                yield return null;
            }

            for (int i = 0; i < slotUIList.Count; i++)
            {
                RectTransform rt = slotUIList[i].GetComponent<RectTransform>();
                if (rt != null)
                {
                    rt.anchoredPosition = targetPositions[i];
                }
            }

            isAnimating = false;
        }

        private void SetSlotVisibility(InventorySlotUI slot, bool visible, bool immediate)
        {
            CanvasGroup slotCanvas = slot.GetComponent<CanvasGroup>();
            if (slotCanvas == null)
            {
                slotCanvas = slot.gameObject.AddComponent<CanvasGroup>();
            }

            if (immediate)
            {
                slotCanvas.alpha = visible ? 1f : 0f;
            }
            else
            {
                float targetAlpha = visible ? 1f : 0f;
                slotCanvas.alpha = Mathf.Lerp(slotCanvas.alpha, targetAlpha, Time.unscaledDeltaTime * transitionSpeed);
            }

            slotCanvas.interactable = visible;
            slotCanvas.blocksRaycasts = visible;
        }
    }
}
