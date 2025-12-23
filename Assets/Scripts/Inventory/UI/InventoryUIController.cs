using System;
using System.Collections.Generic;
using UnityEngine;

namespace TheHunt.Inventory
{
    public class InventoryUIController : MonoBehaviour
    {
        [Header("References")]
        [SerializeField] private InventorySystem inventorySystem;
        [SerializeField] private WeaponInventoryManager weaponManager;
        [SerializeField] private CombinationManager combinationManager;

        [Header("State")]
        private InventoryState currentState = InventoryState.Closed;
        private int contextMenuIndex = 0;
        private List<ItemContextAction> availableActions = new List<ItemContextAction>();
        private bool isCombineMode = false;
        private int combineSourceSlot = -1;

        public InventoryState CurrentState => currentState;
        public bool IsOpen => currentState != InventoryState.Closed;
        public bool IsInContextMenu => currentState == InventoryState.ContextMenu;
        public int ContextMenuIndex => contextMenuIndex;
        public List<ItemContextAction> AvailableActions => availableActions;

        public event Action<InventoryState> OnStateChanged;
        public event Action<List<ItemContextAction>> OnContextMenuOpened;
        public event Action OnContextMenuClosed;
        public event Action<int> OnContextMenuSelectionChanged;
        public event Action<bool, ItemData> OnCombineModeChanged;

        private void Awake()
        {
            if (inventorySystem == null)
                inventorySystem = GetComponent<InventorySystem>();

            if (weaponManager == null)
                weaponManager = GetComponent<WeaponInventoryManager>();

            if (combinationManager == null)
                combinationManager = GetComponent<CombinationManager>();
        }

        public void ToggleInventory()
        {
            if (currentState == InventoryState.Closed)
            {
                OpenInventory();
            }
            else if (currentState == InventoryState.Open)
            {
                CloseInventory();
            }
        }

        public void OpenInventory()
        {
            if (currentState != InventoryState.Closed)
                return;

            SetState(InventoryState.Open);
            Debug.Log("<color=cyan>[INVENTORY UI] Inventory opened</color>");
        }

        public void CloseInventory()
        {
            if (currentState == InventoryState.Closed)
                return;

            if (currentState == InventoryState.ContextMenu)
            {
                CloseContextMenu();
            }

            SetState(InventoryState.Closed);
            Debug.Log("<color=cyan>[INVENTORY UI] Inventory closed</color>");
        }

        public void NavigateInventory(float direction)
        {
            if (currentState != InventoryState.Open)
                return;

            if (direction > 0)
            {
                inventorySystem.SelectNext();
            }
            else if (direction < 0)
            {
                inventorySystem.SelectPrevious();
            }
        }

        public void InteractWithCurrentItem()
        {
            if (isCombineMode)
            {
                HandleCombineInput();
                return;
            }

            if (currentState == InventoryState.Open)
            {
                OpenContextMenu();
            }
            else if (currentState == InventoryState.ContextMenu)
            {
                ExecuteContextAction();
            }
        }

        public void CancelCurrentAction()
        {
            if (isCombineMode)
            {
                CancelCombineMode();
                return;
            }

            if (currentState == InventoryState.ContextMenu)
            {
                CloseContextMenu();
            }
            else if (currentState == InventoryState.Open)
            {
                CloseInventory();
            }
        }

        private void OpenContextMenu()
        {
            ItemInstance currentItem = inventorySystem.CurrentItem;

            if (currentItem == null)
            {
                Debug.Log("<color=yellow>[INVENTORY UI] No item selected</color>");
                return;
            }

            availableActions.Clear();
            contextMenuIndex = 0;

            if (currentItem.itemData is IUsable usable)
            {
                if (usable.CanUse(gameObject))
                {
                    availableActions.Add(ItemContextAction.Use);
                }
            }

            if (currentItem.itemData.CanBeExamined)
            {
                availableActions.Add(ItemContextAction.Examine);
            }

            if (currentItem.itemData is WeaponItemData)
            {
                availableActions.Add(ItemContextAction.EquipPrimary);
                availableActions.Add(ItemContextAction.EquipSecondary);
            }

            if (currentItem.itemData.CanBeCombined && combinationManager != null)
            {
                List<ItemData> possibleCombinations = combinationManager.GetCombinableItemsFor(currentItem.itemData);
                if (possibleCombinations != null && possibleCombinations.Count > 0)
                {
                    availableActions.Add(ItemContextAction.Combine);
                }
            }

            availableActions.Add(ItemContextAction.Drop);

            if (availableActions.Count > 0)
            {
                SetState(InventoryState.ContextMenu);
                OnContextMenuOpened?.Invoke(availableActions);
                Debug.Log($"<color=cyan>[INVENTORY UI] Context menu opened with {availableActions.Count} options</color>");
            }
        }

        private void CloseContextMenu()
        {
            availableActions.Clear();
            contextMenuIndex = 0;
            SetState(InventoryState.Open);
            OnContextMenuClosed?.Invoke();
            Debug.Log("<color=cyan>[INVENTORY UI] Context menu closed</color>");
        }

        public void NavigateContextMenu(float direction)
        {
            if (currentState != InventoryState.ContextMenu || availableActions.Count == 0)
                return;

            int oldIndex = contextMenuIndex;

            if (direction > 0)
            {
                contextMenuIndex = (contextMenuIndex + 1) % availableActions.Count;
            }
            else if (direction < 0)
            {
                contextMenuIndex--;
                if (contextMenuIndex < 0)
                    contextMenuIndex = availableActions.Count - 1;
            }

            if (oldIndex != contextMenuIndex)
            {
                OnContextMenuSelectionChanged?.Invoke(contextMenuIndex);
                Debug.Log($"<color=cyan>[INVENTORY UI] Context menu selection: {availableActions[contextMenuIndex]}</color>");
            }
        }

        private void ExecuteContextAction()
        {
            if (availableActions.Count == 0 || contextMenuIndex < 0 || contextMenuIndex >= availableActions.Count)
                return;

            ItemContextAction action = availableActions[contextMenuIndex];
            ItemInstance currentItem = inventorySystem.CurrentItem;

            if (currentItem == null)
                return;

            Debug.Log($"<color=green>[INVENTORY UI] Executing action: {action} on {currentItem.itemData.ItemName}</color>");

            switch (action)
            {
                case ItemContextAction.Use:
                    inventorySystem.UseCurrentItem();
                    CloseContextMenu();
                    break;

                case ItemContextAction.Examine:
                    inventorySystem.ExamineCurrentItem();
                    break;

                case ItemContextAction.Drop:
                    inventorySystem.DropCurrentItem();
                    CloseContextMenu();
                    break;

                case ItemContextAction.EquipPrimary:
                    if (currentItem.itemData is WeaponItemData weapon && weaponManager != null)
                    {
                        weaponManager.EquipWeapon(weapon, EquipSlot.Primary);
                    }
                    CloseContextMenu();
                    break;

                case ItemContextAction.EquipSecondary:
                    if (currentItem.itemData is WeaponItemData weaponSecondary && weaponManager != null)
                    {
                        weaponManager.EquipWeapon(weaponSecondary, EquipSlot.Secondary);
                    }
                    CloseContextMenu();
                    break;

                case ItemContextAction.Combine:
                    StartCombineMode();
                    CloseContextMenu();
                    break;
            }
        }

        private void SetState(InventoryState newState)
        {
            if (currentState == newState)
                return;

            InventoryState oldState = currentState;
            currentState = newState;
            OnStateChanged?.Invoke(newState);

            if (newState == InventoryState.Closed)
            {
                Time.timeScale = 1f;
            }
            else if (oldState == InventoryState.Closed)
            {
                Time.timeScale = 0f;
            }
        }

        public string GetContextActionDisplayName(ItemContextAction action)
        {
            switch (action)
            {
                case ItemContextAction.Use: return "Use";
                case ItemContextAction.Examine: return "Examine";
                case ItemContextAction.Drop: return "Drop";
                case ItemContextAction.EquipPrimary: return "Equip Primary";
                case ItemContextAction.EquipSecondary: return "Equip Secondary";
                case ItemContextAction.Combine: return "Combine";
                default: return action.ToString();
            }
        }

        private void StartCombineMode()
        {
            isCombineMode = true;
            combineSourceSlot = inventorySystem.SelectedSlot;
            ItemInstance sourceItem = inventorySystem.CurrentItem;
            
            if (sourceItem != null)
            {
                OnCombineModeChanged?.Invoke(true, sourceItem.itemData);
                Debug.Log($"<color=cyan>[INVENTORY UI] Combine mode started. Select item to combine with {sourceItem.itemData.ItemName}</color>");
            }
        }

        public void CancelCombineMode()
        {
            isCombineMode = false;
            combineSourceSlot = -1;
            OnCombineModeChanged?.Invoke(false, null);
            Debug.Log("<color=cyan>[INVENTORY UI] Combine mode cancelled</color>");
        }

        public void TryCombineWithSelected()
        {
            if (!isCombineMode || combineSourceSlot < 0 || combinationManager == null)
                return;

            int targetSlot = inventorySystem.SelectedSlot;

            if (targetSlot == combineSourceSlot)
            {
                Debug.Log("<color=yellow>[INVENTORY UI] Cannot combine item with itself!</color>");
                return;
            }

            bool success = combinationManager.TryCombine(combineSourceSlot, targetSlot);

            if (success)
            {
                Debug.Log("<color=green>[INVENTORY UI] Items combined successfully!</color>");
            }
            else
            {
                Debug.Log("<color=yellow>[INVENTORY UI] These items cannot be combined.</color>");
            }

            CancelCombineMode();
        }

        public void HandleCombineInput()
        {
            if (isCombineMode)
            {
                TryCombineWithSelected();
            }
        }
    }
}
