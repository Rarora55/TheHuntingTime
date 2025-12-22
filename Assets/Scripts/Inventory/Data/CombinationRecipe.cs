using UnityEngine;
using System.Collections.Generic;

namespace TheHunt.Inventory
{
    [CreateAssetMenu(fileName = "CombinationRecipe", menuName = "Inventory/Combination Recipe")]
    public class CombinationRecipe : ScriptableObject
    {
        [Header("Recipe Info")]
        [SerializeField] private string recipeName;
        [SerializeField] [TextArea(2, 4)] private string recipeDescription;

        [Header("Required Items")]
        [Tooltip("First item required for combination")]
        [SerializeField] private ItemData itemA;
        
        [Tooltip("Second item required for combination")]
        [SerializeField] private ItemData itemB;
        
        [Tooltip("Allow combination in any order (A+B or B+A)")]
        [SerializeField] private bool bidirectional = true;

        [Header("Result")]
        [Tooltip("Item created after successful combination")]
        [SerializeField] private ItemData resultItem;
        
        [Tooltip("Quantity of the result item")]
        [SerializeField] private int resultQuantity = 1;

        [Header("Consumption")]
        [Tooltip("Amount of Item A consumed")]
        [SerializeField] private int consumeAmountA = 1;
        
        [Tooltip("Amount of Item B consumed")]
        [SerializeField] private int consumeAmountB = 1;

        [Header("Feedback")]
        [SerializeField] private string successMessage = "Items combined successfully!";
        [SerializeField] private string failMessage = "These items cannot be combined.";
        [SerializeField] private AudioClip combinationSound;

        public string RecipeName => recipeName;
        public string RecipeDescription => recipeDescription;
        public ItemData ItemA => itemA;
        public ItemData ItemB => itemB;
        public bool Bidirectional => bidirectional;
        public ItemData ResultItem => resultItem;
        public int ResultQuantity => resultQuantity;
        public int ConsumeAmountA => consumeAmountA;
        public int ConsumeAmountB => consumeAmountB;
        public string SuccessMessage => successMessage;
        public string FailMessage => failMessage;
        public AudioClip CombinationSound => combinationSound;

        public bool CanCombine(ItemData first, ItemData second)
        {
            if (first == null || second == null)
                return false;

            bool matchDirect = (first == itemA && second == itemB);
            bool matchReverse = bidirectional && (first == itemB && second == itemA);

            return matchDirect || matchReverse;
        }

        public bool IsValidRecipe()
        {
            if (itemA == null || itemB == null || resultItem == null)
                return false;

            if (itemA == itemB)
            {
                Debug.LogWarning($"<color=yellow>[RECIPE] {recipeName}: ItemA and ItemB cannot be the same!</color>");
                return false;
            }

            if (consumeAmountA <= 0 || consumeAmountB <= 0)
            {
                Debug.LogWarning($"<color=yellow>[RECIPE] {recipeName}: Consume amounts must be greater than 0!</color>");
                return false;
            }

            if (resultQuantity <= 0)
            {
                Debug.LogWarning($"<color=yellow>[RECIPE] {recipeName}: Result quantity must be greater than 0!</color>");
                return false;
            }

            return true;
        }

        private void OnValidate()
        {
            if (string.IsNullOrEmpty(recipeName))
            {
                if (itemA != null && itemB != null)
                {
                    recipeName = $"{itemA.ItemName} + {itemB.ItemName}";
                }
            }

            consumeAmountA = Mathf.Max(1, consumeAmountA);
            consumeAmountB = Mathf.Max(1, consumeAmountB);
            resultQuantity = Mathf.Max(1, resultQuantity);
        }
    }
}
