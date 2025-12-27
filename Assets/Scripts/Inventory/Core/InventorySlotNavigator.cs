using System;
using UnityEngine;

namespace TheHunt.Inventory
{
    public class InventorySlotNavigator
    {
        private readonly InventoryDataSO inventoryData;

        public event Action<int, int> OnSelectionChanged;

        public int CurrentSlot => inventoryData != null ? inventoryData.SelectedIndex : 0;

        public InventorySlotNavigator(InventoryDataSO inventoryData)
        {
            this.inventoryData = inventoryData;
        }

        public void SelectNext()
        {
            if (inventoryData == null)
                return;
                
            int oldIndex = inventoryData.SelectedIndex;
            inventoryData.SelectedIndex = (inventoryData.SelectedIndex + 1) % InventorySystem.MAX_SLOTS;
            OnSelectionChanged?.Invoke(oldIndex, inventoryData.SelectedIndex);
            Debug.Log($"<color=cyan>[SLOT NAVIGATOR] Selected slot {inventoryData.SelectedIndex}</color>");
        }

        public void SelectPrevious()
        {
            if (inventoryData == null)
                return;
                
            int oldIndex = inventoryData.SelectedIndex;
            inventoryData.SelectedIndex--;
            if (inventoryData.SelectedIndex < 0)
                inventoryData.SelectedIndex = InventorySystem.MAX_SLOTS - 1;
            OnSelectionChanged?.Invoke(oldIndex, inventoryData.SelectedIndex);
            Debug.Log($"<color=cyan>[SLOT NAVIGATOR] Selected slot {inventoryData.SelectedIndex}</color>");
        }

        public void SelectSlot(int index)
        {
            if (index < 0 || index >= InventorySystem.MAX_SLOTS || inventoryData == null)
                return;

            int oldIndex = inventoryData.SelectedIndex;
            inventoryData.SelectedIndex = index;
            OnSelectionChanged?.Invoke(oldIndex, inventoryData.SelectedIndex);
        }
    }
}
