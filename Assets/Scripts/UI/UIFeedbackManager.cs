using UnityEngine;
using TheHunt.Inventory;

namespace TheHunt.UI
{
    public class UIFeedbackManager : MonoBehaviour
    {
        [Header("Components")]
        [SerializeField] private InventoryFullDialog inventoryFullDialog;
        
        void Awake()
        {
            if (inventoryFullDialog == null)
            {
                inventoryFullDialog = GetComponent<InventoryFullDialog>();
            }
            
            if (inventoryFullDialog == null)
            {
                Debug.LogWarning("[UI FEEDBACK] No InventoryFullDialog found. Adding it automatically.");
                inventoryFullDialog = gameObject.AddComponent<InventoryFullDialog>();
            }
        }
    }
}
