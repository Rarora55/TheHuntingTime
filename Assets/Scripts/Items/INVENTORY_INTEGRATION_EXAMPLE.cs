using UnityEngine;

public class INVENTORY_INTEGRATION_EXAMPLE : MonoBehaviour
{
    void Update()
    {
        if (Input.GetKeyDown(KeyCode.Alpha1))
        {
            UseItemExample();
        }
    }
    
    void UseItemExample()
    {
        Player player = FindObjectOfType<Player>();
        if (player == null) return;
        
        ConsumableItemData medkit = Resources.Load<ConsumableItemData>("Items/Medkit");
        if (medkit == null)
        {
            Debug.LogWarning("Medkit not found! Create one at Resources/Items/Medkit");
            return;
        }
        
        if (medkit.CanUse(player.gameObject))
        {
            medkit.Use(player.gameObject);
            Debug.Log($"<color=green>✅ Used {medkit.ItemName} from inventory</color>");
        }
        else
        {
            Debug.Log($"<color=yellow>⚠️ Cannot use {medkit.ItemName} (health already full?)</color>");
        }
    }
}
