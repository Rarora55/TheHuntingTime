using System;
using System.Collections.Generic;
using UnityEngine;
using TheHunt.Radio.UI;

namespace TheHunt.Inventory
{
    public class InventoryUIController : MonoBehaviour
    {
        [Header("UI Prefab")]
        [SerializeField] private GameObject inventoryCanvasPrefab;
        
        [Header("References")]
        [SerializeField] private InventorySystem inventorySystem;
        [SerializeField] private WeaponInventoryManager weaponManager;
        [SerializeField] private RadioEquipmentManager radioManager;
        [SerializeField] private CombinationManager combinationManager;
        [SerializeField] private WeaponEquipmentPanel weaponEquipmentPanel;
        [SerializeField] private RadioEquipmentPanel radioEquipmentPanel;

        [Header("Runtime UI References")]
        private GameObject canvasInstance;
        private ItemExaminationPanel examinationPanel;

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
            if (inventoryCanvasPrefab != null && canvasInstance == null)
            {
                canvasInstance = Instantiate(inventoryCanvasPrefab);
                canvasInstance.name = "InventoryCanvas (Runtime)";
                
                examinationPanel = canvasInstance.GetComponentInChildren<ItemExaminationPanel>(true);
                weaponEquipmentPanel = canvasInstance.GetComponentInChildren<WeaponEquipmentPanel>(true);
                radioEquipmentPanel = canvasInstance.GetComponentInChildren<RadioEquipmentPanel>(true);
                
                if (examinationPanel == null)
                    Debug.LogWarning("[INVENTORY UI] ItemExaminationPanel not found in prefab!");
                
                if (weaponEquipmentPanel == null)
                    Debug.LogWarning("<color=yellow>[INVENTORY UI] WeaponEquipmentPanel not found in prefab!</color>");
                else
                    Debug.Log("<color=green>[INVENTORY UI] WeaponEquipmentPanel found and assigned!</color>");
                
                if (radioEquipmentPanel == null)
                    Debug.LogWarning("<color=yellow>[INVENTORY UI] RadioEquipmentPanel not found in prefab!</color>");
                else
                    Debug.Log("<color=green>[INVENTORY UI] RadioEquipmentPanel found and assigned!</color>");
            }
            
            if (inventorySystem == null)
                inventorySystem = GetComponent<InventorySystem>();

            if (weaponManager == null)
                weaponManager = GetComponent<WeaponInventoryManager>();
            
            if (radioManager == null)
                radioManager = GetComponent<RadioEquipmentManager>();

            if (combinationManager == null)
                combinationManager = GetComponent<CombinationManager>();
            
            if (weaponEquipmentPanel == null)
                weaponEquipmentPanel = FindFirstObjectByType<WeaponEquipmentPanel>();
            
            if (radioEquipmentPanel == null)
                radioEquipmentPanel = FindFirstObjectByType<RadioEquipmentPanel>();

            if (examinationPanel == null)
                examinationPanel = FindFirstObjectByType<ItemExaminationPanel>();
        }
        
        private void OnDestroy()
        {
            if (canvasInstance != null)
            {
                Destroy(canvasInstance);
            }
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

            // Cerrar panel de examen si está abierto
            if (examinationPanel != null && examinationPanel.IsVisible)
            {
                examinationPanel.Hide();
                Debug.Log("<color=cyan>[INVENTORY UI] Examination panel closed when closing inventory</color>");
            }

            // Salir del modo de selección de armas si está activo
            if (weaponEquipmentPanel != null && weaponEquipmentPanel.IsInWeaponSlotSelectionMode)
            {
                weaponEquipmentPanel.ExitWeaponSlotSelectionMode();
                Debug.Log("<color=cyan>[INVENTORY UI] Weapon slot selection mode exited when closing inventory</color>");
            }

            // Cancelar modo combine si está activo
            if (isCombineMode)
            {
                CancelCombineMode();
                Debug.Log("<color=cyan>[INVENTORY UI] Combine mode cancelled when closing inventory</color>");
            }

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
            
            if (weaponEquipmentPanel != null && weaponEquipmentPanel.IsInWeaponSlotSelectionMode)
            {
                weaponEquipmentPanel.NavigateWeaponSlots(direction);
                return;
            }

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
            if (weaponEquipmentPanel != null && weaponEquipmentPanel.IsInWeaponSlotSelectionMode)
            {
                weaponEquipmentPanel.UnequipCurrentSelectedWeapon();
                return;
            }
            
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
            if (weaponEquipmentPanel != null && weaponEquipmentPanel.IsInWeaponSlotSelectionMode)
            {
                weaponEquipmentPanel.ExitWeaponSlotSelectionMode();
                return;
            }
            
            if (examinationPanel != null && examinationPanel.IsVisible)
            {
                examinationPanel.Hide();
                return;
            }

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

            if (currentItem.itemData is WeaponItemData weaponData && weaponManager != null)
            {
                if (weaponManager.IsWeaponEquipped(weaponData))
                {
                    Debug.Log("<color=yellow>[INVENTORY UI] Cannot interact with equipped weapon</color>");
                    return;
                }
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

            if (currentItem.itemData is RadioItemData)
            {
                availableActions.Add(ItemContextAction.EquipRadio);
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
                    CloseContextMenu();
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

                case ItemContextAction.EquipRadio:
                    if (currentItem.itemData is RadioItemData radio && radioManager != null)
                    {
                        radioManager.TryEquipRadio(radio);
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
        
        public void ToggleWeaponSlotSelectionMode()
        {
            Debug.Log($"<color=magenta>[INVENTORY UI] ToggleWeaponSlotSelectionMode called - State: {currentState}, WeaponPanel: {weaponEquipmentPanel != null}</color>");
            
            if (currentState != InventoryState.Open)
            {
                Debug.Log("<color=yellow>[INVENTORY UI] Inventory not open, cannot toggle weapon slot selection</color>");
                return;
            }
            
            if (weaponEquipmentPanel == null)
            {
                Debug.LogWarning("<color=red>[INVENTORY UI] WeaponEquipmentPanel not found! Cannot toggle weapon slot selection</color>");
                return;
            }
            
            if (weaponEquipmentPanel.IsInWeaponSlotSelectionMode)
            {
                weaponEquipmentPanel.ExitWeaponSlotSelectionMode();
                Debug.Log("<color=cyan>[INVENTORY UI] Exited weapon slot selection mode</color>");
            }
            else
            {
                weaponEquipmentPanel.EnterWeaponSlotSelectionMode();
                Debug.Log("<color=cyan>[INVENTORY UI] Entered weapon slot selection mode</color>");
            }
        }
    }
}
