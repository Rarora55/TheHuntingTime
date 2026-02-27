using UnityEngine;
using TheHunt.Environment;
using TheHunt.UI;

namespace TheHunt.Interaction.Rope
{
    /// <summary>
    /// Extends ClimbSpawnPoint with rope-specific dialog logic.
    /// Add this component alongside ClimbSpawnPoint on rope-generated spawn points.
    /// Controls when dialogs appear, what they say, and what each button does.
    /// </summary>
    [RequireComponent(typeof(ClimbSpawnPoint))]
    public class RopeClimbSpawnPoint : MonoBehaviour, IRopeSpawnDialogAction
    {
        [Header("Config")]
        [Tooltip("ScriptableObject with dialog and usage configuration.")]
        [SerializeField] private RopeSpawnConfig config;

        [Header("Anchor Reference")]
        [Tooltip("The RopeAnchorPassiveItem that created this spawn point.")]
        [SerializeField] private RopeAnchorPassiveItem ownerAnchor;

        private ClimbSpawnPoint climbSpawnPoint;
        private DialogService dialogService;
        private int useCount = 0;

        private void Awake()
        {
            climbSpawnPoint = GetComponent<ClimbSpawnPoint>();
            dialogService = FindFirstObjectByType<DialogService>();

            if (dialogService == null)
                Debug.LogWarning($"<color=yellow>[ROPE SPAWN] {gameObject.name}: DialogService not found!</color>");

            if (config == null)
                Debug.LogWarning($"<color=yellow>[ROPE SPAWN] {gameObject.name}: RopeSpawnConfig not assigned!</color>");

            Debug.Log($"<color=cyan>[ROPE SPAWN] {gameObject.name} initialized. MaxUses: {config?.maxUses}</color>");
        }

        private void OnEnable()
        {
            climbSpawnPoint = GetComponent<ClimbSpawnPoint>();
            climbSpawnPoint.OnInteractRequested += HandleInteractRequested;
        }

        private void OnDisable()
        {
            if (climbSpawnPoint != null)
                climbSpawnPoint.OnInteractRequested -= HandleInteractRequested;
        }

        /// <summary>
        /// Sets the anchor that owns this spawn point, called by RopeAnchorPassiveItem when deploying.
        /// </summary>
        public void Initialize(RopeAnchorPassiveItem anchor, RopeSpawnConfig spawnConfig)
        {
            ownerAnchor = anchor;
            config = spawnConfig;
            useCount = 0;
            Debug.Log($"<color=cyan>[ROPE SPAWN] {gameObject.name} initialized by anchor. Config: {spawnConfig?.name}</color>");
        }

        /// <summary>
        /// Called by ClimbSpawnPoint before executing the teleport.
        /// Returns true to allow teleport, false to block it (dialog is shown instead).
        /// </summary>
        private void HandleInteractRequested(GameObject interactor, System.Action proceedCallback)
        {
            if (config == null)
            {
                proceedCallback?.Invoke();
                return;
            }

            if (config.IsExhausted(useCount))
            {
                Debug.Log($"<color=red>[ROPE SPAWN] Rope exhausted after {useCount} uses.</color>");
                PickupRope();
                return;
            }

            RopeDialogEntry entry = config.GetDialogForUse(useCount + 1);

            if (entry != null)
            {
                Debug.Log($"<color=cyan>[ROPE SPAWN] Showing dialog for use #{useCount + 1}</color>");
                ShowDialog(entry, proceedCallback);
            }
            else
            {
                // No dialog configured for this use, proceed directly
                useCount++;
                Debug.Log($"<color=green>[ROPE SPAWN] Direct use #{useCount} - no dialog</color>");
                proceedCallback?.Invoke();
            }
        }

        private void ShowDialog(RopeDialogEntry entry, System.Action proceedCallback)
        {
            if (dialogService == null)
            {
                Debug.LogError("[ROPE SPAWN] Cannot show dialog: DialogService is null.");
                proceedCallback?.Invoke();
                return;
            }

            dialogService.ShowConfirmation(
                entry.title,
                entry.message,
                onConfirm: () => ExecuteDialogAction(entry.yesAction, proceedCallback),
                onCancel: () => ExecuteDialogAction(entry.noAction, proceedCallback)
            );
        }

        private void ExecuteDialogAction(RopeDialogActionType actionType, System.Action proceedCallback)
        {
            switch (actionType)
            {
                case RopeDialogActionType.UseRespawn:
                    useCount++;
                    Debug.Log($"<color=green>[ROPE SPAWN] Use respawn selected. Use #{useCount}</color>");
                    proceedCallback?.Invoke();
                    break;

                case RopeDialogActionType.PickupRope:
                    Debug.Log("<color=orange>[ROPE SPAWN] Pick up rope selected.</color>");
                    PickupRope();
                    break;

                case RopeDialogActionType.DoNothing:
                    Debug.Log("<color=grey>[ROPE SPAWN] DoNothing selected, closing dialog.</color>");
                    break;
            }
        }

        private void PickupRope()
        {
            if (ownerAnchor != null)
            {
                ownerAnchor.RetractRope();
                Debug.Log("<color=orange>[ROPE SPAWN] Rope retracted via anchor.</color>");
            }
            else
            {
                Debug.LogWarning("[ROPE SPAWN] ownerAnchor is null, cannot retract rope.");
            }
        }

        /// <summary>IRopeSpawnDialogAction implementation (direct external call).</summary>
        public void Execute(RopeClimbSpawnPoint spawnPoint)
        {
            Debug.Log($"[ROPE SPAWN] External Execute called on {gameObject.name}");
        }
    }
}
