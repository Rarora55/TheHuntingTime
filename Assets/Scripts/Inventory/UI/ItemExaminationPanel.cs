using UnityEngine;
using UnityEngine.UI;
using TMPro;

namespace TheHunt.Inventory
{
    public class ItemExaminationPanel : MonoBehaviour
    {
        [Header("Panel References")]
        [SerializeField] private CanvasGroup canvasGroup;
        [SerializeField] private GameObject panelBackground;

        [Header("Item Display")]
        [SerializeField] private Image itemImage;
        [SerializeField] private TextMeshProUGUI itemNameText;
        [SerializeField] private TextMeshProUGUI itemDescriptionText;
        [SerializeField] private TextMeshProUGUI examinationText;

        [Header("Optional Details")]
        [SerializeField] private GameObject detailsContainer;
        [SerializeField] private TextMeshProUGUI itemTypeText;
        [SerializeField] private TextMeshProUGUI stackInfoText;

        [Header("Animation")]
        [SerializeField] private float fadeSpeed = 5f;

        private ItemData currentItem;
        private bool isVisible = false;

        private void Awake()
        {
            if (canvasGroup == null)
                canvasGroup = GetComponent<CanvasGroup>();

            if (canvasGroup == null)
                canvasGroup = gameObject.AddComponent<CanvasGroup>();

            Hide(true);
        }

        public void ShowItem(ItemData itemData)
        {
            if (itemData == null)
            {
                Debug.LogWarning("<color=yellow>[EXAMINE PANEL] Cannot show null item!</color>");
                return;
            }

            if (!itemData.CanBeExamined)
            {
                Debug.LogWarning($"<color=yellow>[EXAMINE PANEL] {itemData.ItemName} cannot be examined!</color>");
                return;
            }

            currentItem = itemData;
            UpdateDisplay();
            Show();

            Debug.Log($"<color=cyan>[EXAMINE PANEL] Showing {itemData.ItemName}</color>");
        }

        public void Hide(bool immediate = false)
        {
            isVisible = false;

            if (immediate)
            {
                canvasGroup.alpha = 0f;
                canvasGroup.interactable = false;
                canvasGroup.blocksRaycasts = false;
            }
            else
            {
                StartCoroutine(FadeOut());
            }

            currentItem = null;
            Debug.Log("<color=cyan>[EXAMINE PANEL] Hidden</color>");
        }

        private void Show()
        {
            isVisible = true;
            StartCoroutine(FadeIn());
        }

        private void UpdateDisplay()
        {
            if (currentItem == null)
                return;

            if (itemNameText != null)
                itemNameText.text = currentItem.ItemName;

            if (itemDescriptionText != null)
            {
                string description = !string.IsNullOrEmpty(currentItem.Description) 
                    ? currentItem.Description 
                    : "No description available.";
                itemDescriptionText.text = description;
            }

            if (examinationText != null)
            {
                string examination = !string.IsNullOrEmpty(currentItem.ExaminationText) 
                    ? currentItem.ExaminationText 
                    : "Nothing special to note.";
                examinationText.text = examination;
            }

            if (itemImage != null)
            {
                Sprite imageToShow = currentItem.ExaminationImage != null 
                    ? currentItem.ExaminationImage 
                    : currentItem.ItemIcon;

                if (imageToShow != null)
                {
                    itemImage.sprite = imageToShow;
                    itemImage.enabled = true;
                }
                else
                {
                    itemImage.enabled = false;
                }
            }

            if (itemTypeText != null)
            {
                itemTypeText.text = $"Type: {GetItemTypeDisplayName(currentItem.ItemType)}";
            }

            if (stackInfoText != null)
            {
                if (currentItem.IsStackable)
                {
                    stackInfoText.text = "Stackable";
                    stackInfoText.gameObject.SetActive(true);
                }
                else
                {
                    stackInfoText.gameObject.SetActive(false);
                }
            }
        }

        private string GetItemTypeDisplayName(ItemType itemType)
        {
            switch (itemType)
            {
                case ItemType.Consumable: return "Consumable";
                case ItemType.KeyItem: return "Key Item";
                case ItemType.Weapon: return "Weapon";
                case ItemType.Ammo: return "Ammunition";
                case ItemType.Examinable: return "Examinable";
                default: return itemType.ToString();
            }
        }

        private System.Collections.IEnumerator FadeIn()
        {
            canvasGroup.interactable = true;
            canvasGroup.blocksRaycasts = true;

            while (canvasGroup.alpha < 1f)
            {
                canvasGroup.alpha += Time.unscaledDeltaTime * fadeSpeed;
                yield return null;
            }

            canvasGroup.alpha = 1f;
        }

        private System.Collections.IEnumerator FadeOut()
        {
            canvasGroup.interactable = false;
            canvasGroup.blocksRaycasts = false;

            while (canvasGroup.alpha > 0f)
            {
                canvasGroup.alpha -= Time.unscaledDeltaTime * fadeSpeed;
                yield return null;
            }

            canvasGroup.alpha = 0f;
        }

        public bool IsVisible => isVisible;
        public ItemData CurrentItem => currentItem;
    }
}
