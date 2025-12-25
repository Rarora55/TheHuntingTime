using UnityEngine;

namespace TheHunt.Inventory
{
    public class InventoryDebugger : MonoBehaviour
    {
        [Header("References")]
        [SerializeField] private InventorySystem inventorySystem;
        [SerializeField] private AmmoInventoryManager ammoManager;
        [SerializeField] private WeaponInventoryManager weaponManager;
        [SerializeField] private InventoryUIController inventoryUIController;

        [Header("Debug Items")]
        [SerializeField] private ItemData testConsumable;
        [SerializeField] private ItemData testWeapon;
        [SerializeField] private ItemData testAmmo;

        [Header("Debug Info")]
        [SerializeField] private bool showDebugInfo = true;

        private void Awake()
        {
            if (inventorySystem == null)
                inventorySystem = GetComponent<InventorySystem>();

            if (ammoManager == null)
                ammoManager = GetComponent<AmmoInventoryManager>();

            if (weaponManager == null)
                weaponManager = GetComponent<WeaponInventoryManager>();

            if (inventoryUIController == null)
                inventoryUIController = GetComponent<InventoryUIController>();
        }

        private void Start()
        {
            if (inventorySystem != null)
            {
                inventorySystem.OnItemAdded += OnItemAdded;
                inventorySystem.OnItemRemoved += OnItemRemoved;
                inventorySystem.OnItemUsed += OnItemUsed;
                inventorySystem.OnSelectionChanged += OnSelectionChanged;
                inventorySystem.OnInventoryFull += OnInventoryFull;
            }

            if (weaponManager != null)
            {
                weaponManager.OnWeaponEquipped += OnWeaponEquipped;
                weaponManager.OnWeaponUnequipped += OnWeaponUnequipped;
            }

            if (ammoManager != null)
            {
                ammoManager.OnAmmoChanged += OnAmmoChanged;
            }

            if (inventoryUIController != null)
            {
                inventoryUIController.OnStateChanged += OnStateChanged;
                inventoryUIController.OnContextMenuOpened += OnContextMenuOpened;
                inventoryUIController.OnContextMenuClosed += OnContextMenuClosed;
            }
        }

        private void OnDestroy()
        {
            if (inventorySystem != null)
            {
                inventorySystem.OnItemAdded -= OnItemAdded;
                inventorySystem.OnItemRemoved -= OnItemRemoved;
                inventorySystem.OnItemUsed -= OnItemUsed;
                inventorySystem.OnSelectionChanged -= OnSelectionChanged;
                inventorySystem.OnInventoryFull -= OnInventoryFull;
            }

            if (weaponManager != null)
            {
                weaponManager.OnWeaponEquipped -= OnWeaponEquipped;
                weaponManager.OnWeaponUnequipped -= OnWeaponUnequipped;
            }

            if (ammoManager != null)
            {
                ammoManager.OnAmmoChanged -= OnAmmoChanged;
            }

            if (inventoryUIController != null)
            {
                inventoryUIController.OnStateChanged -= OnStateChanged;
                inventoryUIController.OnContextMenuOpened -= OnContextMenuOpened;
                inventoryUIController.OnContextMenuClosed -= OnContextMenuClosed;
            }
        }

        private void Update()
        {
            if (!showDebugInfo)
                return;

            if (UnityEngine.Input.GetKeyDown(KeyCode.F1) && testConsumable != null)
            {
                inventorySystem.TryAddItem(testConsumable);
            }

            if (UnityEngine.Input.GetKeyDown(KeyCode.F2) && testWeapon != null)
            {
                inventorySystem.TryAddItem(testWeapon);
            }

            if (UnityEngine.Input.GetKeyDown(KeyCode.F3) && testAmmo != null)
            {
                inventorySystem.TryAddItem(testAmmo);
            }

            if (UnityEngine.Input.GetKeyDown(KeyCode.F4))
            {
                PrintInventoryState();
            }
        }

        private void PrintInventoryState()
        {
            Debug.Log("========== INVENTORY STATE ==========");
            Debug.Log($"Selected Slot: {inventorySystem.SelectedSlot}");
            Debug.Log($"Is Full: {inventorySystem.IsFull}");

            if (weaponManager != null)
            {
                Debug.Log($"Primary Weapon: {weaponManager.PrimaryWeapon?.ItemName ?? "None"}");
                Debug.Log($"Secondary Weapon: {weaponManager.SecondaryWeapon?.ItemName ?? "None"}");
            }

            Debug.Log("\n--- Items ---");
            for (int i = 0; i < InventorySystem.MAX_SLOTS; i++)
            {
                ItemInstance item = inventorySystem.Items[i];
                string itemInfo = item != null ? item.DisplayName : "Empty";
                Debug.Log($"Slot {i}: {itemInfo}");
            }

            Debug.Log("\n--- Ammo ---");
            if (ammoManager != null)
            {
                foreach (AmmoType ammoType in System.Enum.GetValues(typeof(AmmoType)))
                {
                    if (ammoType == AmmoType.None)
                        continue;

                    int count = ammoManager.GetAmmoCount(ammoType);
                    Debug.Log($"{ammoType}: {count}");
                }
            }

            Debug.Log("=====================================");
        }

        private void OnItemAdded(int slot, ItemInstance item)
        {
            if (showDebugInfo)
                Debug.Log($"<color=green>🎒 [DEBUG] Item Added to slot {slot}: {item.DisplayName}</color>");
        }

        private void OnItemRemoved(int slot, ItemInstance item)
        {
            if (showDebugInfo)
                Debug.Log($"<color=orange>🎒 [DEBUG] Item Removed from slot {slot}: {item?.DisplayName ?? "Unknown"}</color>");
        }

        private void OnItemUsed(ItemInstance item)
        {
            if (showDebugInfo)
                Debug.Log($"<color=cyan>🎒 [DEBUG] Item Used: {item.DisplayName}</color>");
        }

        private void OnSelectionChanged(int oldIndex, int newIndex)
        {
            if (showDebugInfo)
            {
                ItemInstance newItem = inventorySystem.Items[newIndex];
                string itemName = newItem != null ? newItem.DisplayName : "Empty";
                Debug.Log($"<color=cyan>🎒 [DEBUG] Selection Changed: {oldIndex} → {newIndex} ({itemName})</color>");
            }
        }

        private void OnInventoryFull()
        {
            if (showDebugInfo)
                Debug.Log("<color=red>🎒 [DEBUG] Inventory is FULL!</color>");
        }

        private void OnWeaponEquipped(EquipSlot slot, WeaponItemData weapon)
        {
            if (showDebugInfo)
                Debug.Log($"<color=green>🔫 [DEBUG] Weapon Equipped in {slot}: {weapon.ItemName}</color>");
        }

        private void OnWeaponUnequipped(EquipSlot slot)
        {
            if (showDebugInfo)
                Debug.Log($"<color=orange>🔫 [DEBUG] Weapon Unequipped from {slot}</color>");
        }

        private void OnAmmoChanged(AmmoType type, int count)
        {
            if (showDebugInfo)
                Debug.Log($"<color=yellow>🔥 [DEBUG] Ammo Changed: {type} = {count}</color>");
        }

        private void OnStateChanged(InventoryState state)
        {
            if (showDebugInfo)
                Debug.Log($"<color=magenta>📋 [DEBUG] Inventory State: {state}</color>");
        }

        private void OnContextMenuOpened(System.Collections.Generic.List<ItemContextAction> actions)
        {
            if (showDebugInfo)
            {
                string actionList = string.Join(", ", actions);
                Debug.Log($"<color=cyan>📋 [DEBUG] Context Menu Opened: [{actionList}]</color>");
            }
        }

        private void OnContextMenuClosed()
        {
            if (showDebugInfo)
                Debug.Log("<color=cyan>📋 [DEBUG] Context Menu Closed</color>");
        }

        private void OnGUI()
        {
            if (!showDebugInfo)
                return;

            GUILayout.BeginArea(new Rect(10, 10, 350, 500));
            GUILayout.Box("🎒 INVENTORY DEBUGGER");

            if (inventoryUIController != null)
            {
                GUILayout.Label($"State: {inventoryUIController.CurrentState}");
            }

            GUILayout.Label($"Selected: {inventorySystem.SelectedSlot}");
            GUILayout.Label($"Full: {inventorySystem.IsFull}");

            GUILayout.Space(10);
            GUILayout.Label("--- Quick Add ---");

            if (GUILayout.Button("F1: Add Test Consumable") && testConsumable != null)
            {
                inventorySystem.TryAddItem(testConsumable);
            }

            if (GUILayout.Button("F2: Add Test Weapon") && testWeapon != null)
            {
                inventorySystem.TryAddItem(testWeapon);
            }

            if (GUILayout.Button("F3: Add Test Ammo") && testAmmo != null)
            {
                inventorySystem.TryAddItem(testAmmo);
            }

            if (GUILayout.Button("F4: Print Inventory"))
            {
                PrintInventoryState();
            }

            GUILayout.Space(10);
            GUILayout.Label("--- Current Item ---");
            if (inventorySystem.CurrentItem != null && inventorySystem.CurrentItem.itemData != null)
            {
                GUILayout.Label($"Name: {inventorySystem.CurrentItem.DisplayName}");
                GUILayout.Label($"Type: {inventorySystem.CurrentItem.itemData.ItemType}");
            }
            else
            {
                GUILayout.Label("No item selected");
            }

            if (inventoryUIController != null && inventoryUIController.IsInContextMenu)
            {
                GUILayout.Space(10);
                GUILayout.Label("--- Context Menu ---");

                for (int i = 0; i < inventoryUIController.AvailableActions.Count; i++)
                {
                    ItemContextAction action = inventoryUIController.AvailableActions[i];
                    string prefix = i == inventoryUIController.ContextMenuIndex ? "►" : "  ";
                    string displayName = inventoryUIController.GetContextActionDisplayName(action);
                    GUILayout.Label($"{prefix} {displayName}");
                }
            }

            GUILayout.EndArea();
        }
    }
}
