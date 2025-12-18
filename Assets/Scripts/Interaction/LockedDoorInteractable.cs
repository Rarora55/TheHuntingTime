using UnityEngine;

namespace TheHunt.Interaction
{
    [RequireComponent(typeof(Collider2D))]
    public class LockedDoorInteractable : InteractableObject
    {
        [Header("Door Settings")]
        [SerializeField] private string requiredKeyID = "rusty_key";
        [SerializeField] private string doorName = "Door";
        [SerializeField] private bool consumeKeyOnUnlock = true;
        [SerializeField] private bool isLocked = true;

        [Header("Visuals")]
        [SerializeField] private SpriteRenderer spriteRenderer;
        [SerializeField] private Sprite lockedSprite;
        [SerializeField] private Sprite unlockedSprite;
        [SerializeField] private Animator doorAnimator;

        [Header("Feedback")]
        [SerializeField] private AudioClip unlockSound;
        [SerializeField] private AudioClip openSound;
        [SerializeField] private AudioClip lockedSound;
        [SerializeField] private GameObject unlockVFX;

        private Collider2D doorCollider;

        void Awake()
        {
            doorCollider = GetComponent<Collider2D>();

            if (doorCollider != null)
            {
                doorCollider.isTrigger = true;
            }

            if (spriteRenderer == null)
            {
                spriteRenderer = GetComponent<SpriteRenderer>();
            }

            UpdatePrompt();
        }

        void UpdatePrompt()
        {
            if (!isLocked)
            {
                interactionPrompt = $"Press E to open {doorName}";
            }
            else
            {
                GameObject player = GameObject.FindGameObjectWithTag("Player");

                if (player != null && HasRequiredKey(player))
                {
                    interactionPrompt = $"Press E to unlock {doorName}";
                }
                else
                {
                    interactionPrompt = $"Locked. Requires: {doorName} Key";
                }
            }
        }

        public override bool CanInteract(GameObject interactor)
        {
            if (!base.CanInteract(interactor))
                return false;

            if (!isLocked)
            {
                return true;
            }

            return HasRequiredKey(interactor);
        }

        bool HasRequiredKey(GameObject interactor)
        {
            Inventory.KeyInventoryManager keyManager = interactor.GetComponent<Inventory.KeyInventoryManager>();

            if (keyManager == null)
            {
                return false;
            }

            return keyManager.HasKeyItem(requiredKeyID);
        }

        protected override void OnInteract(GameObject interactor)
        {
            if (!isLocked)
            {
                OpenDoor();
                return;
            }

            Inventory.KeyInventoryManager keyManager = interactor.GetComponent<Inventory.KeyInventoryManager>();

            if (keyManager == null || !keyManager.HasKeyItem(requiredKeyID))
            {
                PlayLockedSound();
                Debug.Log($"<color=yellow>[DOOR] {doorName} is locked!</color>");
                return;
            }

            UnlockDoor();

            if (consumeKeyOnUnlock)
            {
                keyManager.ConsumeKeyItem(requiredKeyID);
            }

            OpenDoor();
        }

        void UnlockDoor()
        {
            isLocked = false;

            if (spriteRenderer != null && unlockedSprite != null)
            {
                spriteRenderer.sprite = unlockedSprite;
            }

            if (unlockSound != null)
            {
                AudioSource.PlayClipAtPoint(unlockSound, transform.position);
            }

            if (unlockVFX != null)
            {
                Instantiate(unlockVFX, transform.position, Quaternion.identity);
            }

            Debug.Log($"<color=green>[DOOR] {doorName} unlocked!</color>");

            UpdatePrompt();
        }

        void OpenDoor()
        {
            if (doorAnimator != null)
            {
                doorAnimator.SetTrigger("Open");
            }

            if (doorCollider != null)
            {
                doorCollider.enabled = false;
            }

            if (openSound != null)
            {
                AudioSource.PlayClipAtPoint(openSound, transform.position);
            }

            Debug.Log($"<color=cyan>[DOOR] {doorName} opened!</color>");

            SetInteractable(false);
        }

        void PlayLockedSound()
        {
            if (lockedSound != null)
            {
                AudioSource.PlayClipAtPoint(lockedSound, transform.position);
            }
        }

        void OnDrawGizmosSelected()
        {
            Gizmos.color = isLocked ? Color.red : Color.green;
            Gizmos.DrawWireCube(transform.position, GetComponent<Collider2D>().bounds.size);

            if (isLocked)
            {
                UnityEditor.Handles.Label(transform.position + Vector3.up * 2f, $"Requires: {requiredKeyID}");
            }
        }
    }
}
