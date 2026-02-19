using UnityEngine;
using TheHunt.Inventory;
using TheHunt.Interaction;

namespace TheHunt.Items
{
    [RequireComponent(typeof(SpriteRenderer))]
    [RequireComponent(typeof(Collider2D))]
    public class RadioPickup : MonoBehaviour, IInteractable
    {
        [Header("Item Data")]
        [SerializeField] private RadioItemData radioData;

        [Header("Visual Settings")]
        [SerializeField] private bool useItemIcon = true;
        [SerializeField] private Sprite customSprite;
        [SerializeField] private Vector2 spriteSize = new Vector2(1f, 1f);

        [Header("Interaction Settings")]
        [SerializeField] private float interactionRadius = 2f;
        [SerializeField] private string pickupMessage = "Press E to pick up";

        [Header("Animation")]
        [SerializeField] private bool enableFloating = true;
        [SerializeField] private float floatSpeed = 1f;
        [SerializeField] private float floatHeight = 0.3f;
        
        [Header("Debug")]
        [SerializeField] private bool enableDebugLogs = true;

        private SpriteRenderer spriteRenderer;
        private Vector3 startPosition;
        private bool wasPickedUp = false;

        public bool IsInteractable => !wasPickedUp && radioData != null;
        public string InteractionPrompt => GetPickupMessage();

        public bool CanInteract(GameObject interactor)
        {
            bool canInteract = !wasPickedUp && radioData != null && interactor.GetComponent<InventorySystem>() != null;
            
            if (enableDebugLogs)
            {
                Debug.Log($"<color=cyan>[RADIO PICKUP] CanInteract called by {interactor.name} - Result: {canInteract}</color>");
            }
            
            return canInteract;
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
            
            if (radioData == null)
            {
                Debug.LogWarning($"<color=yellow>[RADIO PICKUP] {gameObject.name} has no RadioItemData assigned!</color>");
            }
            else if (enableDebugLogs)
            {
                Debug.Log($"<color=green>[RADIO PICKUP] {gameObject.name} initialized - Layer: {LayerMask.LayerToName(gameObject.layer)}, IsInteractable: {IsInteractable}</color>");
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

            if (radioData != null && useItemIcon)
            {
                spriteRenderer.sprite = radioData.ItemIcon;
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
        }

        private string GetPickupMessage()
        {
            if (radioData == null)
                return "???";

            return $"{pickupMessage} {radioData.ItemName}";
        }

        public void Interact(GameObject interactor)
        {
            if (enableDebugLogs)
            {
                Debug.Log($"<color=magenta>[RADIO PICKUP] Interact called by {interactor.name}</color>");
            }
            
            if (wasPickedUp || radioData == null)
            {
                if (enableDebugLogs)
                    Debug.LogWarning($"<color=yellow>[RADIO PICKUP] Cannot interact - WasPickedUp: {wasPickedUp}, RadioData: {(radioData != null ? "OK" : "NULL")}</color>");
                return;
            }

            InventorySystem inventory = interactor.GetComponent<InventorySystem>();
            
            if (inventory == null)
            {
                Debug.LogWarning($"<color=yellow>[RADIO PICKUP] {interactor.name} has no InventorySystem!</color>");
                return;
            }

            bool success = inventory.TryAddItem(radioData);
            
            if (success)
            {
                OnPickedUp(interactor);
            }
            else
            {
                Debug.Log($"<color=yellow>[RADIO PICKUP] Inventory full! Cannot pick up {radioData.ItemName}</color>");
            }
        }

        private void OnPickedUp(GameObject picker)
        {
            wasPickedUp = true;
            
            Debug.Log($"<color=green>[RADIO PICKUP] {picker.name} picked up {radioData.ItemName}. Open inventory to equip it!</color>");
            
            Destroy(gameObject);
        }

        private void OnDrawGizmosSelected()
        {
            Gizmos.color = Color.cyan;
            Gizmos.DrawWireSphere(transform.position, interactionRadius);
        }
    }
}
