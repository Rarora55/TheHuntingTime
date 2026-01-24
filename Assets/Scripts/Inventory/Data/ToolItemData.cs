using UnityEngine;

namespace TheHunt.Inventory
{
    [CreateAssetMenu(fileName = "New Tool", menuName = "Inventory/Tool Item")]
    public class ToolItemData : ItemData
    {
        [Header("Tool Settings")]
        [SerializeField] private ToolType toolType = ToolType.None;
        
        public ToolType ToolType => toolType;
        
        public override void Use(GameObject user)
        {
            Debug.Log($"<color=yellow>[TOOL] {ItemName} cannot be used directly. Interact with appropriate objects to use this tool.</color>");
        }
    }
}
