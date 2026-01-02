using UnityEngine;
using TheHunt.Inventory;

public class RopeAnchorPoint : MonoBehaviour
{
    [Header("Anchor Settings")]
    [SerializeField] private Transform ropeSpawnPoint;
    [SerializeField] private float ropeLength = 5f;
    [SerializeField] private GameObject ropePrefab;
    
    [Header("Visual Feedback")]
    [SerializeField] private SpriteRenderer anchorVisual;
    [SerializeField] private Color availableColor = Color.green;
    [SerializeField] private Color usedColor = Color.gray;
    
    private bool isRopeDeployed;
    private GameObject deployedRope;
    private Collider2D ropeCollider;
    
    public bool IsRopeDeployed => isRopeDeployed;
    public float RopeLength => ropeLength;
    
    void Start()
    {
        if (ropeSpawnPoint == null)
        {
            ropeSpawnPoint = transform;
        }
        
        UpdateVisual();
    }
    
    public bool CanDeployRope()
    {
        return !isRopeDeployed;
    }
    
    public bool DeployRope(global::Player player)
    {
        if (isRopeDeployed || ropePrefab == null)
        {
            return false;
        }
        
        WeaponInventoryManager weaponManager = player.GetComponent<WeaponInventoryManager>();
        if (weaponManager == null)
        {
            Debug.LogWarning($"<color=yellow>[ROPE ANCHOR] Player doesn't have WeaponInventoryManager</color>");
            return false;
        }
        
        WeaponItemData secondaryWeapon = weaponManager.SecondaryWeapon;
        if (secondaryWeapon == null || 
            secondaryWeapon.WeaponType != WeaponType.Tool || 
            secondaryWeapon.ToolType != ToolType.Rope)
        {
            Debug.Log($"<color=yellow>[ROPE ANCHOR] No rope equipped in secondary slot</color>");
            return false;
        }
        
        deployedRope = Instantiate(ropePrefab, ropeSpawnPoint.position, Quaternion.identity, transform);
        
        RopeClimbable ropeClimbable = deployedRope.GetComponent<RopeClimbable>();
        if (ropeClimbable != null)
        {
            ropeClimbable.Initialize(this, ropeLength);
        }
        
        ropeCollider = deployedRope.GetComponent<Collider2D>();
        
        isRopeDeployed = true;
        UpdateVisual();
        
        Debug.Log($"<color=green>[ROPE ANCHOR] Rope deployed successfully with length {ropeLength}</color>");
        
        return true;
    }
    
    public void RemoveRope()
    {
        if (deployedRope != null)
        {
            Destroy(deployedRope);
            deployedRope = null;
            ropeCollider = null;
        }
        
        isRopeDeployed = false;
        UpdateVisual();
        
        Debug.Log($"<color=orange>[ROPE ANCHOR] Rope removed</color>");
    }
    
    void UpdateVisual()
    {
        if (anchorVisual != null)
        {
            anchorVisual.color = isRopeDeployed ? usedColor : availableColor;
        }
    }
    
    void OnDrawGizmos()
    {
        if (ropeSpawnPoint == null)
            ropeSpawnPoint = transform;
        
        Gizmos.color = isRopeDeployed ? Color.gray : Color.green;
        Gizmos.DrawWireSphere(ropeSpawnPoint.position, 0.3f);
        
        Gizmos.color = Color.yellow;
        Vector3 ropeEnd = ropeSpawnPoint.position + Vector3.down * ropeLength;
        Gizmos.DrawLine(ropeSpawnPoint.position, ropeEnd);
        Gizmos.DrawWireSphere(ropeEnd, 0.2f);
    }
}
