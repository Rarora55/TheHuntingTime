using UnityEngine;
using TheHunt.Inventory;

namespace TheHunt.Interaction
{
    public class RopeAnchorPassiveItem : PassiveItem
    {
        [Header("Rope Settings")]
        [SerializeField] private float ropeLength = 5f;
        [SerializeField] private GameObject ropePrefab;
        [SerializeField] private Transform spawnPoint;
        
        [Header("Visual Feedback")]
        [SerializeField] private SpriteRenderer anchorSprite;
        [SerializeField] private Color deployedColor = Color.gray;
        
        private GameObject deployedRope;
        private bool isDeployed = false;
        private Color originalColor;
        
        private void Awake()
        {
            if (anchorSprite != null)
            {
                originalColor = anchorSprite.color;
            }
            
            if (spawnPoint == null)
            {
                spawnPoint = transform;
            }
            
            interactionPrompt = "Deploy Rope";
        }
        
        protected override bool CanExecuteAction(GameObject interactor)
        {
            if (isDeployed)
            {
                return false;
            }
            
            global::Player player = interactor.GetComponent<global::Player>();
            if (player == null)
            {
                return false;
            }
            
            WeaponInventoryManager weaponManager = player.GetComponent<WeaponInventoryManager>();
            if (weaponManager == null)
            {
                return false;
            }
            
            WeaponItemData secondaryWeapon = weaponManager.SecondaryWeapon;
            
            bool hasRope = secondaryWeapon != null &&
                          secondaryWeapon.WeaponType == WeaponType.Tool &&
                          secondaryWeapon.ToolType == ToolType.Rope;
            
            return hasRope;
        }
        
        protected override void ExecutePassiveAction(GameObject interactor)
        {
            if (ropePrefab == null)
            {
                Debug.LogError($"<color=red>[ROPE ANCHOR] No rope prefab assigned!</color>");
                return;
            }
            
            Vector3 spawnPosition = spawnPoint.position;
            deployedRope = Instantiate(ropePrefab, spawnPosition, Quaternion.identity, transform);
            
            RopeClimbable ropeClimbable = deployedRope.GetComponent<RopeClimbable>();
            if (ropeClimbable != null)
            {
                ropeClimbable.Initialize(this, ropeLength);
            }
            
            isDeployed = true;
            
            if (anchorSprite != null)
            {
                anchorSprite.color = deployedColor;
            }
            
            SetInteractable(false);
            
            Debug.Log($"<color=green>[ROPE ANCHOR] Rope deployed successfully! Length: {ropeLength}</color>");
        }
        
        public void RetractRope()
        {
            if (deployedRope != null)
            {
                Destroy(deployedRope);
            }
            
            isDeployed = false;
            
            if (anchorSprite != null)
            {
                anchorSprite.color = originalColor;
            }
            
            SetInteractable(true);
            
            Debug.Log($"<color=cyan>[ROPE ANCHOR] Rope retracted</color>");
        }
        
        private void OnDrawGizmos()
        {
            if (spawnPoint == null)
            {
                spawnPoint = transform;
            }
            
            Gizmos.color = Color.yellow;
            Gizmos.DrawWireSphere(spawnPoint.position, 0.2f);
            
            Gizmos.color = Color.cyan;
            Vector3 ropeEnd = spawnPoint.position + Vector3.down * ropeLength;
            Gizmos.DrawLine(spawnPoint.position, ropeEnd);
        }
    }
}
