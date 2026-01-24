using UnityEngine;
using TheHunt.Inventory;

namespace TheHunt.Environment
{
    public class RopeDeploymentSystem : MonoBehaviour
    {
        [Header("Rope Anchor Settings")]
        [SerializeField] private Transform ropeSpawnPoint;
        [SerializeField] private GameObject ropePrefab;
        [SerializeField] private float ropeLength = 5f;
        [SerializeField] private float fadeDuration = 0.5f;

        [Header("Visual Feedback")]
        [SerializeField] private SpriteRenderer anchorSprite;
        [SerializeField] private Color deployedColor = Color.gray;
        
        private GameObject deployedRope;
        private Transform ropeExitPoint;
        private bool isDeployed = false;
        private Color originalColor;

        private void Awake()
        {
            if (anchorSprite != null)
            {
                originalColor = anchorSprite.color;
            }

            if (ropeSpawnPoint == null)
            {
                ropeSpawnPoint = transform;
            }
        }

        public bool TryDeployRope(global::Player player)
        {
            if (isDeployed)
            {
                Debug.Log("<color=yellow>[ROPE DEPLOYMENT] Rope already deployed</color>");
                return false;
            }

            if (!HasRopeInInventory(player))
            {
                Debug.Log("<color=yellow>[ROPE DEPLOYMENT] Player doesn't have rope in inventory</color>");
                return false;
            }

            DeployRope();
            return true;
        }

        private bool HasRopeInInventory(global::Player player)
        {
            if (player == null) return false;

            WeaponInventoryManager weaponManager = player.GetComponent<WeaponInventoryManager>();
            if (weaponManager == null) return false;

            WeaponItemData primaryWeapon = weaponManager.PrimaryWeapon;
            WeaponItemData secondaryWeapon = weaponManager.SecondaryWeapon;

            bool hasPrimaryRope = primaryWeapon != null && 
                                  primaryWeapon.WeaponType == WeaponType.Tool && 
                                  primaryWeapon.ToolType == ToolType.Rope;
            
            bool hasSecondaryRope = secondaryWeapon != null && 
                                    secondaryWeapon.WeaponType == WeaponType.Tool && 
                                    secondaryWeapon.ToolType == ToolType.Rope;

            return hasPrimaryRope || hasSecondaryRope;
        }

        private void DeployRope()
        {
            if (ropePrefab == null)
            {
                Debug.LogError("<color=red>[ROPE DEPLOYMENT] No rope prefab assigned!</color>");
                return;
            }

            Vector3 spawnPosition = ropeSpawnPoint.position;
            deployedRope = Instantiate(ropePrefab, spawnPosition, Quaternion.identity, transform);

            SetupRopeClimbable();

            isDeployed = true;

            if (anchorSprite != null)
            {
                anchorSprite.color = deployedColor;
            }

            Debug.Log("<color=green>[ROPE DEPLOYMENT] Rope deployed successfully!</color>");
        }

        private void SetupRopeClimbable()
        {
            if (deployedRope == null) return;

            ClimbableWithTeleport climbable = deployedRope.GetComponent<ClimbableWithTeleport>();
            if (climbable == null)
            {
                climbable = deployedRope.AddComponent<ClimbableWithTeleport>();
            }

            if (ropeExitPoint == null)
            {
                GameObject exitGO = new GameObject("RopeExitPoint");
                exitGO.transform.SetParent(transform);
                ropeExitPoint = exitGO.transform;
                ropeExitPoint.position = ropeSpawnPoint.position + Vector3.down * ropeLength;
            }

            typeof(ClimbableWithTeleport)
                .GetField("exitPoint", System.Reflection.BindingFlags.NonPublic | System.Reflection.BindingFlags.Instance)
                ?.SetValue(climbable, ropeExitPoint);
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

            Debug.Log("<color=cyan>[ROPE DEPLOYMENT] Rope retracted</color>");
        }

        private void OnDrawGizmosSelected()
        {
            if (ropeSpawnPoint == null)
                ropeSpawnPoint = transform;

            Gizmos.color = Color.yellow;
            Gizmos.DrawWireSphere(ropeSpawnPoint.position, 0.2f);

            Gizmos.color = Color.cyan;
            Vector3 ropeEnd = ropeSpawnPoint.position + Vector3.down * ropeLength;
            Gizmos.DrawLine(ropeSpawnPoint.position, ropeEnd);
            Gizmos.DrawWireSphere(ropeEnd, 0.3f);

#if UNITY_EDITOR
            UnityEditor.Handles.Label(ropeEnd + Vector3.down * 0.5f, "ROPE EXIT");
#endif
        }
    }
}
