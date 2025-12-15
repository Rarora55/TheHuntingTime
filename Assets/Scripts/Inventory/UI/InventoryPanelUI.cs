using UnityEngine;
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
        [SerializeField] private GameObject inventoryPanel;
        [SerializeField] private GameObject contextMenuPanel;

        private List<InventorySlotUI> slotUIList = new List<InventorySlotUI>();
        private int currentHighlightedSlot = 0;

        private void Awake()
        {
            if (inventorySystem == null)
                inventorySystem = GetComponent<InventorySystem>();

            if (uiController == null)
                uiController = GetComponent<InventoryUIController>();

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
            
            if (inventoryPanel != null)
                inventoryPanel.SetActive(false);

            if (contextMenuPanel != null)
                contextMenuPanel.SetActive(false);
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

            Debug.Log($"<color=cyan>[INVENTORY UI] Created {slotUIList.Count} slots</color>");
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
                Debug.Log($"<color=cyan>[INVENTORY UI] Updated slot {slotIndex}</color>");
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
                    ShowContextMenu();
                    break;
            }
        }

        private void ShowInventory()
        {
            if (inventoryPanel != null)
            {
                inventoryPanel.SetActive(true);
                RefreshAllSlots();
            }

            if (contextMenuPanel != null)
            {
                contextMenuPanel.SetActive(false);
            }

            Debug.Log("<color=cyan>[INVENTORY UI] Inventory panel shown</color>");
        }

        private void HideInventory()
        {
            if (inventoryPanel != null)
            {
                inventoryPanel.SetActive(false);
            }

            if (contextMenuPanel != null)
            {
                contextMenuPanel.SetActive(false);
            }

            Debug.Log("<color=cyan>[INVENTORY UI] Inventory panel hidden</color>");
        }

        private void ShowContextMenu()
        {
            if (contextMenuPanel != null)
            {
                contextMenuPanel.SetActive(true);
            }

            Debug.Log("<color=cyan>[INVENTORY UI] Context menu shown</color>");
        }
    }
}
