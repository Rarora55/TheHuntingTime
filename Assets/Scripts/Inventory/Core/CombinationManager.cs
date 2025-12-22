using UnityEngine;
using System;
using System.Collections.Generic;
using System.Linq;

namespace TheHunt.Inventory
{
    public class CombinationManager : MonoBehaviour
    {
        [Header("Recipe Database")]
        [SerializeField] private List<CombinationRecipe> allRecipes = new List<CombinationRecipe>();

        [Header("Settings")]
        [SerializeField] private bool showDebugLogs = true;
        [SerializeField] private bool allowMultipleCombinations = false;

        private InventorySystem inventory;
        private AudioSource audioSource;

        public event Action<ItemData, ItemData, ItemData> OnCombinationSuccess;
        public event Action<ItemData, ItemData> OnCombinationFailed;
        public event Action<List<CombinationRecipe>> OnAvailableCombinationsChanged;

        private void Awake()
        {
            CleanNullRecipes();

            inventory = GetComponent<InventorySystem>();
            audioSource = GetComponent<AudioSource>();

            if (audioSource == null)
            {
                audioSource = gameObject.AddComponent<AudioSource>();
                audioSource.playOnAwake = false;
            }

            ValidateRecipes();
        }

        private void CleanNullRecipes()
        {
            if (allRecipes == null)
            {
                allRecipes = new List<CombinationRecipe>();
                return;
            }

            int nullCount = allRecipes.RemoveAll(r => r == null);
            if (nullCount > 0)
            {
                LogWarning($"Removed {nullCount} null recipe(s) from allRecipes list");
            }
        }

        private void Start()
        {
            if (inventory != null)
            {
                inventory.OnItemAdded += (slot, item) => UpdateAvailableCombinations();
                inventory.OnItemRemoved += (slot, item) => UpdateAvailableCombinations();
            }
        }

        public bool TryCombine(int slotA, int slotB)
        {
            if (inventory == null)
            {
                LogWarning("InventorySystem not found!");
                return false;
            }

            ItemInstance itemInstanceA = inventory.Items[slotA];
            ItemInstance itemInstanceB = inventory.Items[slotB];

            if (itemInstanceA == null || itemInstanceB == null)
            {
                LogWarning("One or both slots are empty!");
                return false;
            }

            return TryCombineItems(itemInstanceA.itemData, itemInstanceB.itemData, slotA, slotB);
        }

        public bool TryCombineItems(ItemData itemA, ItemData itemB, int slotA = -1, int slotB = -1)
        {
            if (itemA == null || itemB == null)
            {
                LogWarning("Cannot combine null items!");
                return false;
            }

            if (itemA == itemB)
            {
                LogWarning("Cannot combine an item with itself!");
                OnCombinationFailed?.Invoke(itemA, itemB);
                return false;
            }

            CombinationRecipe recipe = FindRecipe(itemA, itemB);

            if (recipe == null)
            {
                Log($"No recipe found for {itemA.ItemName} + {itemB.ItemName}");
                OnCombinationFailed?.Invoke(itemA, itemB);
                return false;
            }

            if (!HasRequiredQuantities(itemA, itemB, recipe))
            {
                LogWarning($"Not enough items to combine! Need {recipe.ConsumeAmountA}x {itemA.ItemName} and {recipe.ConsumeAmountB}x {itemB.ItemName}");
                OnCombinationFailed?.Invoke(itemA, itemB);
                return false;
            }

            ExecuteCombination(recipe, slotA, slotB);
            return true;
        }

        private void ExecuteCombination(CombinationRecipe recipe, int slotA, int slotB)
        {
            ItemData itemA = recipe.ItemA;
            ItemData itemB = recipe.ItemB;

            ConsumeItems(itemA, recipe.ConsumeAmountA);
            ConsumeItems(itemB, recipe.ConsumeAmountB);

            bool added = inventory.TryAddItem(recipe.ResultItem);

            if (added)
            {
                Log($"<color=green>Successfully combined {itemA.ItemName} + {itemB.ItemName} → {recipe.ResultItem.ItemName}</color>");
                
                if (recipe.CombinationSound != null && audioSource != null)
                {
                    audioSource.PlayOneShot(recipe.CombinationSound);
                }

                OnCombinationSuccess?.Invoke(itemA, itemB, recipe.ResultItem);
            }
            else
            {
                LogWarning("Failed to add result item to inventory!");
                
                inventory.TryAddItem(itemA);
                inventory.TryAddItem(itemB);
            }

            UpdateAvailableCombinations();
        }

        private bool HasRequiredQuantities(ItemData itemA, ItemData itemB, CombinationRecipe recipe)
        {
            int countA = CountItemInInventory(itemA);
            int countB = CountItemInInventory(itemB);

            return countA >= recipe.ConsumeAmountA && countB >= recipe.ConsumeAmountB;
        }

        private void ConsumeItems(ItemData itemData, int amount)
        {
            int remaining = amount;

            for (int i = 0; i < inventory.Items.Length && remaining > 0; i++)
            {
                ItemInstance item = inventory.Items[i];
                
                if (item != null && item.itemData == itemData)
                {
                    int toRemove = Mathf.Min(remaining, item.quantity);
                    inventory.RemoveItem(i, toRemove);
                    remaining -= toRemove;
                }
            }
        }

        private int CountItemInInventory(ItemData itemData)
        {
            int count = 0;

            foreach (ItemInstance item in inventory.Items)
            {
                if (item != null && item.itemData == itemData)
                {
                    count += item.quantity;
                }
            }

            return count;
        }

        public CombinationRecipe FindRecipe(ItemData itemA, ItemData itemB)
        {
            foreach (CombinationRecipe recipe in allRecipes)
            {
                if (recipe == null)
                    continue;

                if (recipe.IsValidRecipe() && recipe.CanCombine(itemA, itemB))
                {
                    return recipe;
                }
            }

            return null;
        }

        public List<CombinationRecipe> GetAvailableCombinations()
        {
            List<CombinationRecipe> available = new List<CombinationRecipe>();

            if (inventory == null)
                return available;

            foreach (CombinationRecipe recipe in allRecipes)
            {
                if (recipe == null)
                    continue;

                if (!recipe.IsValidRecipe())
                    continue;

                int countA = CountItemInInventory(recipe.ItemA);
                int countB = CountItemInInventory(recipe.ItemB);

                if (countA >= recipe.ConsumeAmountA && countB >= recipe.ConsumeAmountB)
                {
                    available.Add(recipe);
                }
            }

            return available;
        }

        public List<ItemData> GetCombinableItemsFor(ItemData sourceItem)
        {
            List<ItemData> combinableItems = new List<ItemData>();

            if (sourceItem == null)
                return combinableItems;

            foreach (CombinationRecipe recipe in allRecipes)
            {
                if (recipe == null)
                    continue;

                if (!recipe.IsValidRecipe())
                    continue;

                if (recipe.ItemA == sourceItem)
                {
                    if (!combinableItems.Contains(recipe.ItemB))
                        combinableItems.Add(recipe.ItemB);
                }
                else if (recipe.Bidirectional && recipe.ItemB == sourceItem)
                {
                    if (!combinableItems.Contains(recipe.ItemA))
                        combinableItems.Add(recipe.ItemA);
                }
            }

            return combinableItems;
        }

        public bool CanCombineWith(ItemData itemA, ItemData itemB)
        {
            return FindRecipe(itemA, itemB) != null;
        }

        private void UpdateAvailableCombinations()
        {
            List<CombinationRecipe> available = GetAvailableCombinations();
            OnAvailableCombinationsChanged?.Invoke(available);
        }

        private void ValidateRecipes()
        {
            List<CombinationRecipe> invalidRecipes = new List<CombinationRecipe>();

            foreach (CombinationRecipe recipe in allRecipes)
            {
                if (recipe == null)
                {
                    LogWarning("Found null recipe in allRecipes list!");
                    continue;
                }

                if (!recipe.IsValidRecipe())
                {
                    invalidRecipes.Add(recipe);
                }
            }

            if (invalidRecipes.Count > 0)
            {
                LogWarning($"Found {invalidRecipes.Count} invalid recipes:");
                foreach (CombinationRecipe recipe in invalidRecipes)
                {
                    LogWarning($"  - {recipe.RecipeName}");
                }
            }
        }

        public void AddRecipe(CombinationRecipe recipe)
        {
            if (recipe != null && !allRecipes.Contains(recipe))
            {
                allRecipes.Add(recipe);
                Log($"Added recipe: {recipe.RecipeName}");
                UpdateAvailableCombinations();
            }
        }

        public void RemoveRecipe(CombinationRecipe recipe)
        {
            if (allRecipes.Contains(recipe))
            {
                allRecipes.Remove(recipe);
                Log($"Removed recipe: {recipe.RecipeName}");
                UpdateAvailableCombinations();
            }
        }

        private void Log(string message)
        {
            if (showDebugLogs)
            {
                Debug.Log($"<color=cyan>[COMBINATION] {message}</color>");
            }
        }

        private void LogWarning(string message)
        {
            if (showDebugLogs)
            {
                Debug.LogWarning($"<color=yellow>[COMBINATION] {message}</color>");
            }
        }

        [ContextMenu("List All Recipes")]
        private void ListAllRecipes()
        {
            Debug.Log($"<color=cyan>=== ALL RECIPES ({allRecipes.Count}) ===</color>");
            
            foreach (CombinationRecipe recipe in allRecipes)
            {
                if (recipe == null)
                {
                    Debug.Log("✗ [NULL RECIPE]");
                    continue;
                }

                string status = recipe.IsValidRecipe() ? "✓" : "✗";
                Debug.Log($"{status} {recipe.RecipeName}: {recipe.ItemA?.ItemName} + {recipe.ItemB?.ItemName} → {recipe.ResultItem?.ItemName}");
            }
        }

        [ContextMenu("List Available Combinations")]
        private void ListAvailableCombinations()
        {
            List<CombinationRecipe> available = GetAvailableCombinations();
            
            Debug.Log($"<color=green>=== AVAILABLE COMBINATIONS ({available.Count}) ===</color>");
            
            foreach (CombinationRecipe recipe in available)
            {
                Debug.Log($"✓ {recipe.RecipeName}");
            }
        }
    }
}
