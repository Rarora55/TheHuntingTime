using UnityEngine;
using TheHunt.Interaction;

namespace TheHunt.Inventory
{
    [RequireComponent(typeof(SpriteRenderer))]
    [RequireComponent(typeof(Collider2D))]
    public class WorldItem : MonoBehaviour, IInteractable
    {
        [Header("Item Data")]
        [SerializeField] private ItemData itemData;
        [SerializeField] private int quantity = 1;

        [Header("Visual Settings")]
        [SerializeField] private bool useItemIcon = true;
        [SerializeField] private Sprite customSprite;
        [SerializeField] private Vector2 spriteSize = new Vector2(1f, 1f);

        [Header("Interaction Settings")]
        [SerializeField] private float interactionRadius = 2f;
        [SerializeField] private string pickupMessage = "Press E to pick up";
        [SerializeField] private bool autoPickup = false;

        [Header("Animation")]
        [SerializeField] private bool enableFloating = true;
        [SerializeField] private float floatSpeed = 1f;
        [SerializeField] private float floatHeight = 0.3f;
        [SerializeField] private bool enableRotation = false;
        [SerializeField] private float rotationSpeed = 50f;

        private SpriteRenderer spriteRenderer;
        private Vector3 startPosition;
        private bool wasPickedUp = false;

        public ItemData ItemData => itemData;
        public int Quantity => quantity;
        public bool IsInteractable => !wasPickedUp && itemData != null;
        public string InteractionPrompt => GetPickupMessage();

        public bool CanInteract(GameObject interactor)
        {
            if (wasPickedUp || itemData == null)
                return false;

            InventorySystem inventory = interactor.GetComponent<InventorySystem>();
            return inventory != null;
        }

        private void Awake()
        {
            spriteRenderer = GetComponent<SpriteRenderer>();
            SetupVisuals();
            SetupCollider();
        }

        private void Start()
        {
            startPosition = transform.position;
            
            if (itemData == null)
            {
                Debug.LogWarning($"<color=yellow>[WORLD ITEM] {gameObject.name} has no ItemData assigned!</color>");
            }
        }

        private void Update()
        {
            if (wasPickedUp)
                return;

            AnimateItem();
        }

        private void SetupVisuals()
        {
            if (spriteRenderer == null)
                return;

            if (itemData != null && useItemIcon)
            {
                spriteRenderer.sprite = itemData.ItemIcon;
            }
            else if (customSprite != null)
            {
                spriteRenderer.sprite = customSprite;
            }

            transform.localScale = new Vector3(spriteSize.x, spriteSize.y, 1f);
        }

        private void SetupCollider()
        {
            Collider2D collider = GetComponent<Collider2D>();
            
            if (collider != null)
            {
                collider.isTrigger = true;
            }
        }

        private void AnimateItem()
        {
            if (enableFloating)
            {
                float newY = startPosition.y + Mathf.Sin(Time.time * floatSpeed) * floatHeight;
                transform.position = new Vector3(transform.position.x, newY, transform.position.z);
            }

            if (enableRotation)
            {
                transform.Rotate(Vector3.forward, rotationSpeed * Time.deltaTime);
            }
        }

        private string GetPickupMessage()
        {
            if (itemData == null)
                return "???";

            if (quantity > 1)
            {
                return $"{pickupMessage} {itemData.ItemName} x{quantity}";
            }

            return $"{pickupMessage} {itemData.ItemName}";
        }

        public void Interact(GameObject interactor)
        {
            if (wasPickedUp || itemData == null)
                return;

            InventorySystem inventory = interactor.GetComponent<InventorySystem>();
            
            if (inventory == null)
            {
                Debug.LogWarning($"<color=yellow>[WORLD ITEM] {interactor.name} has no InventorySystem!</color>");
                return;
            }

            bool success = inventory.TryAddItem(itemData);
            
            if (success)
            {
                OnPickedUp(interactor);
            }
            else
            {
                Debug.Log($"<color=yellow>[WORLD ITEM] Inventory full! Cannot pick up {itemData.ItemName}</color>");
            }
        }

        private void OnPickedUp(GameObject picker)
        {
            wasPickedUp = true;
            
            Debug.Log($"<color=green>[WORLD ITEM] {picker.name} picked up {itemData.ItemName} x{quantity}</color>");
            
            StartCoroutine(PickupAnimation());
        }

        private System.Collections.IEnumerator PickupAnimation()
        {
            float duration = 0.3f;
            float elapsed = 0f;
            
            Vector3 startPos = transform.position;
            Vector3 endPos = startPos + Vector3.up * 1f;
            Vector3 startScale = transform.localScale;
            
            while (elapsed < duration)
            {
                elapsed += Time.deltaTime;
                float t = elapsed / duration;
                
                transform.position = Vector3.Lerp(startPos, endPos, t);
                transform.localScale = Vector3.Lerp(startScale, Vector3.zero, t);
                
                if (spriteRenderer != null)
                {
                    Color color = spriteRenderer.color;
                    color.a = 1f - t;
                    spriteRenderer.color = color;
                }
                
                yield return null;
            }
            
            Destroy(gameObject);
        }

        private void OnTriggerEnter2D(Collider2D other)
        {
            if (autoPickup && !wasPickedUp)
            {
                if (other.CompareTag("Player"))
                {
                    Interact(other.gameObject);
                }
            }
        }

        private void OnDrawGizmosSelected()
        {
            Gizmos.color = Color.yellow;
            Gizmos.DrawWireSphere(transform.position, interactionRadius);
        }

        public void SetItemData(ItemData data, int qty = 1)
        {
            itemData = data;
            quantity = qty;
            
            if (Application.isPlaying && spriteRenderer != null)
            {
                SetupVisuals();
            }
        }

        [ContextMenu("Refresh Visuals")]
        private void RefreshVisuals()
        {
            if (spriteRenderer == null)
                spriteRenderer = GetComponent<SpriteRenderer>();
                
            SetupVisuals();
        }
    }
}
