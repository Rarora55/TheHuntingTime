using UnityEngine;
using TMPro;
using System.Collections.Generic;

namespace TheHunt.Inventory
{
    public class ContextMenuUI : MonoBehaviour
    {
        [Header("References")]
        [SerializeField] private InventoryUIController uiController;

        [Header("UI Elements")]
        [SerializeField] private Transform optionsContainer;
        [SerializeField] private GameObject optionPrefab;

        [Header("Visual Settings")]
        [SerializeField] private Color normalColor = Color.white;
        [SerializeField] private Color selectedColor = Color.yellow;

        private List<TextMeshProUGUI> optionTexts = new List<TextMeshProUGUI>();
        private int currentSelection = 0;

        private void Awake()
        {
            if (uiController == null)
                uiController = FindFirstObjectByType<InventoryUIController>();
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
            ClearOptions();

            foreach (ItemContextAction action in actions)
            {
                CreateOption(action);
            }

            UpdateSelectionVisual(0);
        }

        private void OnContextMenuClosed()
        {
            ClearOptions();
        }

        private void OnSelectionChanged(int newIndex)
        {
            UpdateSelectionVisual(newIndex);
        }

        private void CreateOption(ItemContextAction action)
        {
            if (optionsContainer == null || optionPrefab == null)
                return;

            GameObject optionObj = Instantiate(optionPrefab, optionsContainer);
            TextMeshProUGUI textComponent = optionObj.GetComponent<TextMeshProUGUI>();

            if (textComponent != null)
            {
                textComponent.text = uiController.GetContextActionDisplayName(action);
                textComponent.color = normalColor;
                optionTexts.Add(textComponent);
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
            currentSelection = 0;
        }

        private void UpdateSelectionVisual(int selectedIndex)
        {
            currentSelection = selectedIndex;

            for (int i = 0; i < optionTexts.Count; i++)
            {
                if (i == selectedIndex)
                {
                    optionTexts[i].color = selectedColor;
                    optionTexts[i].fontSize = optionTexts[i].fontSize * 1.1f;
                }
                else
                {
                    optionTexts[i].color = normalColor;
                    optionTexts[i].fontSize = optionTexts[i].fontSize / 1.1f;
                }
            }
        }
    }
}
